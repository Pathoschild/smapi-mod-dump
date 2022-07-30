/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using JetBrains.Annotations;
using LinqFasterer;
using SpriteMaster.Extensions;
using SpriteMaster.Extensions.Reflection;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using ZstdNet;

namespace Benchmarks.BenchmarkBase;

[PublicAPI]
public abstract class ProgramBase<TOptions> where TOptions : Options, new() {
	private const string ArgsEnvVar = @"BENCHIE_ARGS";
	private static TOptions? CurrentOptionsImpl = null!;

	private static readonly Lazy<byte[]> ZStdDictionary = new(MakeDictionary);
	private static byte[] MakeDictionary() {
		var samples = new List<string>();

		void AddSample(string sample, bool enquote = true) {
			if (enquote) {
				samples.Add($"\"{sample.ToLowerInvariant()}\"");
				samples.Add($"\"{sample.ToLowerInvariant()}\",");
			}
			else {
				samples.Add($"{sample.ToLowerInvariant()}");
			}
		}

		void AddOption(string sample, bool shortOpt) {
			foreach (var prefix in shortOpt ? new[] {"-", "/"} : new[] {"--", "/"}) {
				AddSample($"{prefix}{sample}");
				AddSample($"\"{prefix}{sample}=", enquote: false);
			}
		}

		AddSample("--");
		foreach (var member in typeof(TOptions).GetFields().Cast<MemberInfo>().Concat(typeof(TOptions).GetProperties().Cast<MemberInfo>())) {
			if (member.GetCustomAttribute<OptionAttribute>() is not { } option) {
				continue;
			}

			AddOption(option.LongOpt, false);
			if (option.ShortOpt.HasValue) {
				AddOption($"{option.ShortOpt}", true);
			}
		}

		var assembly = typeof(TOptions).Assembly;

		foreach (var type in assembly.GetTypes()
							.WhereF(
								type =>
									!type.IsAbstract &&
									type.IsAssignableTo(typeof(Benchmarks.BenchmarkBase))
							)
						) {
			AddSample(type.Name);
		}

		foreach (var type in assembly.GetTypes().Where(type => type.IsAssignableTo(typeof(Benchmarks.BenchmarkBase)))) {
			foreach (var method in type.GetMethods()) {
				if (!method.HasAttribute<BenchmarkAttribute>()) {
					continue;
				}

				AddSample(method.Name, enquote: true);
			}
		}

		var byteSamples = new List<byte[]>(samples.Count);
		foreach (var sample in samples) {
			var sampleBytes = System.Text.Encoding.UTF8.GetBytes(sample);
			byteSamples.Add(sampleBytes);
		}

		return ZstdNet.DictBuilder.TrainFromBuffer(byteSamples);
	}

	private static string[] ParsePackedArgs(string? source) {
		if (source is null) {
			return Array.Empty<string>();
		}

		var argBytes = System.Convert.FromBase64String(source);
		using var decompressor = new ZstdNet.Decompressor(new(ZStdDictionary.Value));
		var decompressedBytes = decompressor.Unwrap(argBytes);
		var argsJson = System.Text.Encoding.UTF8.GetString(decompressedBytes);

		if (System.Text.Json.JsonSerializer.Deserialize<string[]>(argsJson) is { } newArgs) {
			return newArgs;
		}

		return Array.Empty<string>();
	}

	private static string MakePackedArgs(string[] args) {
		var argsJson = System.Text.Json.JsonSerializer.Serialize(args.SelectF(s => s.ToLowerInvariant()).ToArrayF());
		var jsonBytes = System.Text.Encoding.UTF8.GetBytes(argsJson);
		using var compressor = new ZstdNet.Compressor(new(ZStdDictionary.Value, compressionLevel: CompressionOptions.MaxCompressionLevel));
		var compressedBytes = compressor.Wrap(jsonBytes);
		return System.Convert.ToBase64String(compressedBytes);
	}

	public static TOptions CurrentOptions {
		get {
			if (CurrentOptionsImpl is not {} options) {
				string[] args = ParsePackedArgs(Environment.GetEnvironmentVariable(ArgsEnvVar));

				options = CurrentOptionsImpl = Options.From<TOptions>(args);
			}

			return options;
		}
	}

	private static Regex[] CreatePatterns<TEnumerable>(TEnumerable patterns) where TEnumerable : IEnumerable<string> {
		return patterns.Select(Options.CreatePattern).Distinct().ToArray();
	}

	private readonly record struct RunResult(Summary Summary, IConfig Config, Job Job);

	private static IEnumerable<RunResult> ConditionalRun(Type benchmarkType, string[] args, Options options, OptionPermutation optionPermutation) {
		if (CurrentOptions.Cold) {
			yield return ConditionalRun(benchmarkType, args, options, optionPermutation, coldStart: true);
		}

		yield return ConditionalRun(benchmarkType, args, options, optionPermutation, coldStart: false);
	}

	private static RunResult ConditionalRun(Type benchmarkType, string[] args, Options options, in OptionPermutation optionPermutation, bool? coldStart) {
		var config = DefaultConfig.Instance.WithOptions(ConfigOptions.Default);
		//if (Debugger.IsAttached) {
			config = config.WithOptions(ConfigOptions.DisableOptimizationsValidator);
		//}

		var runners = options.Runners.ToList();
		if (options.Reverse) {
			if (runners.Count != 0) {
				runners = runners.OrderByF(runner => runner.Reversed()).ToListF();
			}
		}

		if (runners.Count != 0) {
			var patterns = CreatePatterns(runners);

			var filters = patterns.Select(runner =>
				new NameFilter(runner.IsMatch)
			).Cast<IFilter>();

			var disjunction = new DisjunctionFilter(filters.ToArray());

			config = config.AddFilter(disjunction);
		}

		if (options.DiagnoseCpu) {
			config = config.AddDiagnoser(CpuDiagnoser.Default);
		}

		if (options.DiagnoseMemory) {
			config = config.AddDiagnoser(MemoryDiagnoser.Default);
		}

		if (options.DiagnoseInlining) {
			config = config.AddDiagnoser(new InliningDiagnoser());
		}

		if (options.DiagnoseTailCall) {
			config = config.AddDiagnoser(new TailCallDiagnoser());
		}

		if (options.DiagnoseEtw) {
			config = config.AddDiagnoser(new EtwProfiler());
		}

		//config.AddJob(Job.InProcess);

		var packedArgs = MakePackedArgs(args);

		Job job;
		if (Debugger.IsAttached || CurrentOptions.InProcess) {
			job = Job.InProcess;
		}
		else {
			job = Job.Default;
		}

		job = job
			.WithGcServer(optionPermutation.Gc == GCType.Server)
			.WithRuntime(optionPermutation.Runtime)
			.WithEnvironmentVariables(
				new EnvironmentVariable("DOTNET_TieredCompilation", optionPermutation.Tiered ? "1" : "0"),
				new EnvironmentVariable(ArgsEnvVar, packedArgs)
			)
			.WithStrategy((coldStart ?? false) ? RunStrategy.ColdStart : RunStrategy.Throughput);

		//if (typeof(TBenchmark) == typeof(Benchmarks.Premultiply)) {
		//	job = job.WithMinIterationCount(60).WithMaxIterationCount(400);
		//}

		string name = $"{optionPermutation.Gc}.{optionPermutation.Runtime}";
		if (coldStart.HasValue) {
			name = $"{name}.{(coldStart.Value ? "cold" : "warm")}";
		}

		config = config.AddJob(job);
		config = config.WithArtifactsPath(Path.Combine(config.ArtifactsPath, name));

		{
			List<Type> baseTypes = new();
			Type? baseType = benchmarkType;
			while (baseType is not null) {
				baseTypes.Add(baseType);
				baseType = baseType.BaseType;
			}

			foreach (var type in baseTypes.ReverseF()) {
				System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
			}
		}

		GC.Collect(int.MaxValue, GCCollectionMode.Forced, blocking: true, compacting: true);

		var summary = BenchmarkRunner.Run(benchmarkType, config);

		return new(summary, config, job);
	}

	private readonly record struct OptionPermutation(GCType Gc, Runtime Runtime, bool Tiered);

	public static int MainBase(Type rootType, string[] args, Func<Regex, Action<Regex>?>? execCallback = null) {
		var options = Options.From<TOptions>(args);
		CurrentOptionsImpl = options;

		//options.GCTypes.Add(GCType.Workstation);
		//options.Runtimes.Remove(CoreRtRuntime.CoreRt50);


		if (options.GCTypes.Count == 0) {
			options.GCTypes.Add(GCType.Workstation);
		}

		if (options.Runtimes.Count == 0) {
			options.Runtimes.Add(CoreRuntime.Core50);
		}

		HashSet<OptionPermutation> optionPermutations = new();

		foreach (var gcType in options.GCTypes) {
			foreach (var runtime in options.Runtimes) {
				optionPermutations.Add(new(gcType, runtime, Tiered: false));
			}
		}

		var allBenchmarkTypes = rootType.Assembly.GetTypes()
			.WhereF(type =>
				!type.IsAbstract &&
				type.IsAssignableTo(typeof(Benchmarks.BenchmarkBase))
			).ToArrayF();

		var setPatterns = CreatePatterns(options.Set);

		var matchingBenchmarkTypes = allBenchmarkTypes
			.WhereF(type =>
				setPatterns.Any(pattern => pattern.IsMatch(type.Name))
			).ToArrayF();

		Dictionary<string, Action> externalSets = new();

		bool error = false;
		foreach (var setPattern in setPatterns) {
			if (!matchingBenchmarkTypes.AnyF(type => setPattern.IsMatch(type.Name))) {
				bool hasExternal = false;
				if (execCallback is not null) {
					if (execCallback(setPattern) is { } callback) {
						hasExternal = true;
						externalSets.Add(setPattern.ToString(), () => callback(setPattern));
					}
				}

				if (!hasExternal) {
					Console.Error.WriteLine($"No set matches '{setPattern}'");
					error = true;
				}
			}
		}

		if (error) {
			Console.Error.WriteLine("Valid Sets:");
			foreach (var set in allBenchmarkTypes) {
				Console.Error.WriteLine($"  {set.Name}");
			}
			Environment.Exit(-2);
		}

		if (CurrentOptions.Clear) {
			Console.Clear();
		}

		foreach (var externalSet in externalSets) {
			externalSet.Value.Invoke();
		}

		var results = new List<RunResult>(optionPermutations.Count * matchingBenchmarkTypes.Length);
		foreach (var benchmarkType in matchingBenchmarkTypes) {
			foreach (var optionPermutation in optionPermutations) {
				results.AddRange(ConditionalRun(benchmarkType, args, options, optionPermutation));
			}
		}

		foreach (var (summary, config, job) in results) {
			var s = summary;
		}

		return 0;
	}
}