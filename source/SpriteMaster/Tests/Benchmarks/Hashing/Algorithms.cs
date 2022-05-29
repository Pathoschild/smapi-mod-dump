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
using BenchmarkDotNet.Order;
using SpriteMaster.Hashing.Algorithms;
using System.Data.HashFunction.xxHash;

namespace Hashing;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.Declared, MethodOrderPolicy.Declared)]
[CsvExporter]
//[InliningDiagnoser(true, true)]
//[TailCallDiagnoser]
//[EtwProfiler]
//[SimpleJob(RuntimeMoniker.CoreRt50)]
public class Algorithms {
	private const int RandSeed = 0x13377113;
	private const int MinSize = 0;
	private const int MaxSize = 2048;

	public readonly struct DataSet<T> where T : unmanaged {
		public readonly T[] Data;

		public readonly uint Index => (Data.Length == 0) ? 0u : (uint)Math.Round(Math.Log2(Data.Length)) + 1u;

		public DataSet(T[] data) => Data = data;

		public override string ToString() => $"({Index:D2}) {Data.Length}";
	}

	public static List<DataSet<byte>> DataSets { get; }

	private readonly record struct ValidationSettings(bool UseAVX2, bool UseSSE2, bool UsePrefetch, uint UnrollCount) {
		internal readonly void Apply() {
			XxHash3.UseAVX2 = UseAVX2;
			XxHash3.UseSSE2 = UseSSE2;
			XxHash3.UsePrefetch = UsePrefetch;
			XxHash3.UnrollCount = UnrollCount;

			XxHash3Test.UseAVX2 = UseAVX2;
			XxHash3Test.UseSSE2 = UseSSE2;
			XxHash3Test.UsePrefetch = UsePrefetch;
			XxHash3Test.UnrollCount = UnrollCount;
		}

		public override readonly string ToString() {
			var options = new List<string>(4);
			if (UseAVX2) {
				options.Add("AVX2");
			}
			if (UseSSE2) {
				options.Add("SSE2");
			}
			if (UsePrefetch) {
				options.Add("Prefetch");
			}
			if (UnrollCount != 0u) {
				options.Add($"U{UnrollCount}");
			}

			if (options.Count == 0) {
				return "Baseline";
			}

			return string.Join(' ', options);
		}
	}

	private static void Validate(byte[] data) {
		return;

		ValidationSettings baselineSettings = new(false, false, false, 0u);

		baselineSettings.Apply();
		var baselineHash = XxHash3.Hash64(data);

		var configs = new ValidationSettings[] {
			new(false, false, false, 0U),

			new(true, false, false, 0U),
			new(false, true, false, 0U),
			new(false, false, true, 0U),

			new(true, false, false, 2U),
			new(false, true, false, 2U),

			new(true, false, false, 4U),
			new(false, true, false, 4U),
		};

		foreach (var config in configs) {
			config.Apply();
			var hash = XxHash3.Hash64(data);
			if (baselineHash != hash) {
				throw new Exception($"Hash Mismatch: {config} {data.Length}");
			}
			//Console.Out.WriteLine($"Validated: {config} {data.Length}");
		}

		foreach (var config in configs) {
			config.Apply();
			var hash = XxHash3Test.Hash64(data);
			if (baselineHash != hash) {
				throw new Exception($"Hash Mismatch: Test {config} {data.Length}");
			}
			//Console.Out.WriteLine($"Validated: Test {config} {data.Length}");
		}

		XxHash3.UseAVX2 = true;
		XxHash3.UseSSE2 = true;
		XxHash3.UsePrefetch = true;
		XxHash3.UnrollCount = 4u;

		XxHash3Test.UseAVX2 = true;
		XxHash3Test.UseSSE2 = true;
		XxHash3Test.UsePrefetch = true;
		XxHash3Test.UnrollCount = 4u;
	}

	static Algorithms() {
		DataSets = new();

		var random = new Random(RandSeed);

		int start = MinSize;

		DataSet<byte> MakeDataSet(int length) {
			var data = GC.AllocateUninitializedArray<byte>(length);
			random.NextBytes(data);

			Validate(data);

			return new(data);
		}

		if (start == 0) {
			var dataSet = MakeDataSet(start);

			DataSets.Add(dataSet);

			start = 1;
		}

		for (int i = start; i <= MaxSize; i *= 2) {
			var dataSet = MakeDataSet(i);

			DataSets.Add(dataSet);
		}
	}

	[GlobalSetup(Target = nameof(xxHash3))]
	public void xxHash3() {
		XxHash3.UseAVX2 = true;
		XxHash3.UseSSE2 = true;
		XxHash3.UsePrefetch = true;
		XxHash3.UnrollCount = 4;
	}

	[Benchmark(Description = "xxHash3", Baseline = true)]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong xxHash3(DataSet<byte> dataSet) {
		return XxHash3.Hash64(dataSet.Data);
	}

	[GlobalSetup(Target = nameof(xxHash3Test))]
	public void xxHash3Test() {
		XxHash3Test.UseAVX2 = true;
		XxHash3Test.UseSSE2 = true;
		XxHash3Test.UsePrefetch = true;
		XxHash3Test.UnrollCount = 4;
	}

	[Benchmark(Description = "xxHash3 (New Test)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong xxHash3Test(DataSet<byte> dataSet) {
		return XxHash3Test.Hash64(dataSet.Data);
	}

	[Benchmark(Description = "xxHash3 (Old)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong xxHash3Old(DataSet<byte> dataSet) {
		return XxHash3Old.Hash64(dataSet.Data);
	}

	[Benchmark(Description = "FNV1a")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong FNV1a(in DataSet<byte> dataSet) {
		return Functions.FNV1a(dataSet.Data);
	}

	/*
	[Benchmark(Description = "CombHash")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong CombHash(in DataSet<byte> dataSet) {
		return Functions.CombHash(dataSet.Data);
	}
	*/
}
