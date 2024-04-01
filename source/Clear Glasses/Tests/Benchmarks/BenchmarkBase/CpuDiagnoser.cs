/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using JetBrains.Annotations;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Benchmarks.BenchmarkBase;

[PublicAPI]
[AttributeUsage(AttributeTargets.Class)]
public class CpuDiagnoserAttribute : Attribute, IConfigSource
{
	public IConfig Config { get; }

	public CpuDiagnoserAttribute()
	{
		Config = ManualConfig.CreateEmpty().AddDiagnoser(new CpuDiagnoser());
	}
}

[PublicAPI]
public class CpuDiagnoser : IDiagnoser {
	private const string DiagnoserId = nameof(CpuDiagnoser);
	public static readonly CpuDiagnoser Default = new();

	private readonly Process Process;

	private struct TimeSpanPair {
		internal TimeSpan Start;
		internal TimeSpan End;

		internal TimeSpan Duration => End - Start;
	}

	private readonly Stopwatch RealStopwatch = new();
	private TimeSpanPair Real;
	private TimeSpanPair User;
	private TimeSpanPair Privileged;
	private TimeSpanPair Total;

	public CpuDiagnoser()
	{
		Process = Process.GetCurrentProcess();
	}

	public IEnumerable<string> Ids => new[] { nameof(CpuDiagnoser) };

	public IEnumerable<IExporter> Exporters => Array.Empty<IExporter>();

	public IEnumerable<IAnalyser> Analysers => Array.Empty<IAnalyser>();

	public void DisplayResults(ILogger logger)
	{
	}

	public RunMode GetRunMode(BenchmarkCase benchmarkCase) => RunMode.NoOverhead;

	public void Handle(HostSignal signal, DiagnoserActionParameters parameters) {
		switch (signal)
		{
			case HostSignal.BeforeActualRun:
				RealStopwatch.Start();
				Real.Start = RealStopwatch.Elapsed;
				User.Start = Process.UserProcessorTime;
				Privileged.Start = Process.PrivilegedProcessorTime;
				Total.Start = Process.TotalProcessorTime;
				break;
			case HostSignal.AfterActualRun:
				Real.End = RealStopwatch.Elapsed;
				User.End = Process.UserProcessorTime;
				Privileged.End = Process.PrivilegedProcessorTime;
				Total.End = Process.TotalProcessorTime;
				break;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static double GetMetricValue(DiagnoserResults results, in TimeSpan duration) {
		return (duration.Ticks * 100.0) / results.TotalOperations;
	}

	public IEnumerable<Metric> ProcessResults(DiagnoserResults results) {
		yield return new(RealMetricDescriptor.Instance, GetMetricValue(results, Real.Duration));
		yield return new(UserMetricDescriptor.Instance, GetMetricValue(results, User.Duration));
		yield return new(PrivilegedMetricDescriptor.Instance, GetMetricValue(results, Privileged.Duration));
		yield return new(TotalMetricDescriptor.Instance, GetMetricValue(results, Total.Duration));
		yield return new(UsageMetricDescriptor.Instance, (Total.Duration / Real.Duration) * 100.0);
	}

	public IEnumerable<ValidationError> Validate(ValidationParameters validationParameters) =>
		Array.Empty<ValidationError>();

	private abstract class DurationMetricDescriptor<TMetricDescriptor> : IMetricDescriptor where TMetricDescriptor : DurationMetricDescriptor<TMetricDescriptor>, new() {
		internal static readonly TMetricDescriptor Instance = new();

		public string Id { get; }
		public string DisplayName => Id;
		public string Legend => Id;
		public string NumberFormat => "0.##";
		public UnitType UnitType => UnitType.Time;
		public string Unit => "ns";
		public bool TheGreaterTheBetter => false;
		public int PriorityInCategory => 1;

		protected DurationMetricDescriptor(string id) => Id = id;
	}

	private class RealMetricDescriptor : DurationMetricDescriptor<RealMetricDescriptor> {
		public RealMetricDescriptor() : base("CPU Real Time") { }
	}

	private class UserMetricDescriptor : DurationMetricDescriptor<UserMetricDescriptor> {
		public UserMetricDescriptor() : base("CPU User Time") { }
	}

	private class PrivilegedMetricDescriptor : DurationMetricDescriptor<PrivilegedMetricDescriptor> {
		public PrivilegedMetricDescriptor() : base("CPU Privileged Time") { }
	}

	private class TotalMetricDescriptor : DurationMetricDescriptor<TotalMetricDescriptor> {
		public TotalMetricDescriptor() : base("CPU Total Time") { }
	}

	private abstract class PercentageMetricDescriptor<TMetricDescriptor> : IMetricDescriptor where TMetricDescriptor : PercentageMetricDescriptor<TMetricDescriptor>, new() {
		internal static readonly TMetricDescriptor Instance = new();

		public string Id { get; }
		public string DisplayName => Id;
		public string Legend => Id;
		public string NumberFormat => "0.##";
		public UnitType UnitType => UnitType.Dimensionless;
		public string Unit => "%";
		public bool TheGreaterTheBetter => false;
		public int PriorityInCategory => 1;

		protected PercentageMetricDescriptor(string id) => Id = id;
	}

	private class UsageMetricDescriptor : PercentageMetricDescriptor<UsageMetricDescriptor> {
		public UsageMetricDescriptor() : base("CPU Usage") { }
	}
}