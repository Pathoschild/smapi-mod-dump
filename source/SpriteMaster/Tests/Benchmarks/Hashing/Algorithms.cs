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
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;

namespace Hashing;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.Declared, MethodOrderPolicy.Declared)]
//[InliningDiagnoser(true, true)]
//[TailCallDiagnoser]
//[EtwProfiler]
//[SimpleJob(RuntimeMoniker.CoreRt50)]
public class Algorithms {
	private const int RandSeed = 0x13377113;
	private const int MinSize = 1;
	private const int MaxSize = 0x100_0000;

	public readonly struct DataSet<T> where T : unmanaged {
		public readonly T[] Data;

		public DataSet(T[] data) => Data = data;

		public override string ToString() => Data.Length.ToString();
	}

	public List<DataSet<byte>> DataSets { get; init; } = new();

	public int currentDataSet;

	public Algorithms() {
		var random = new Random(RandSeed);
		for (int i = MinSize; i <= MaxSize; i *= 2) {
			var data = GC.AllocateUninitializedArray<byte>(i);
			random.NextBytes(data);

			DataSets.Add(new(data));
		}
	}

	/*
	[Benchmark(Description = "xxHash3", Baseline = true)]
	[ArgumentsSource(nameof(DataSets))]
	public ulong xxHash3(in DataSet<byte> dataSet) {
		return SpriteMaster.Hashing.XXHash3.Hash64(dataSet.Data);
	}
	*/

	[Benchmark(Description = "xxHash3")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong xxHash3Ptr(DataSet<byte> dataSet) {
		return SpriteMaster.Hashing.XXHash3.Hash64(dataSet.Data);
	}

	static readonly System.Data.HashFunction.xxHash.xxHashConfig xxHashConfig = new() { HashSizeInBits = 64 };
	[Benchmark(Description = "xxHash")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong xxHash(in DataSet<byte> dataSet) {
		return BitConverter.ToUInt64(System.Data.HashFunction.xxHash.xxHashFactory.Instance.Create(xxHashConfig).ComputeHash(dataSet.Data).Hash);
	}

	static readonly System.Data.HashFunction.CityHash.CityHashConfig cityHashConfig = new() { HashSizeInBits = 64 };
	[Benchmark(Description = "CityHash")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong CityHash(in DataSet<byte> dataSet) {
		return BitConverter.ToUInt64(System.Data.HashFunction.CityHash.CityHashFactory.Instance.Create(cityHashConfig).ComputeHash(dataSet.Data).Hash);
	}

	[Benchmark(Description = "SHA1")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong SHA1(in DataSet<byte> dataSet) {
		using (var sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider()) {
			return BitConverter.ToUInt64(sha1.ComputeHash(dataSet.Data));
		}
	}

	[Benchmark(Description = "MD5")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong MD5(in DataSet<byte> dataSet) {
		using (var md5 = new System.Security.Cryptography.HMACMD5()) {
			return BitConverter.ToUInt64(md5.ComputeHash(dataSet.Data));
		}
	}

	private static ulong FNV1a(byte[] data) {
		ulong hash = 0xcbf29ce484222325UL;
		foreach (var octet in data) {
			hash ^= octet;
			hash *= 0x00000100000001B3UL;
		}
		return hash;
	}

	private static ulong FNV1aSpan(ReadOnlySpan<byte> data) {
		ulong hash = 0xcbf29ce484222325UL;
		foreach (var octet in data) {
			hash ^= octet;
			hash *= 0x00000100000001B3UL;
		}
		return hash;
	}

	private static unsafe ulong FNV1aPtr(byte* data, int length) {
		ulong hash = 0xcbf29ce484222325UL;
		for (int i = 0; i < length; ++i) {
			var octet = data[i];
			hash ^= octet;
			hash *= 0x00000100000001B3UL;
		}
		return hash;
	}

	[Benchmark(Description = "FNV1a Array")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong FNV1aArray(in DataSet<byte> dataSet) {
		return FNV1a(dataSet.Data);
	}

	/*
	[Benchmark(Description = "FNV1a Span")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong FNV1aSpan(DataSet<byte> dataSet) {
		return FNV1aSpan(dataSet.Data);
	}

	[Benchmark(Description = "FNV1a Pointer")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public unsafe ulong FNV1aPtr(DataSet<byte> dataSet) {
		fixed (byte* ptr = dataSet.Data) {
			return FNV1aPtr(ptr, dataSet.Data.Length);
		}
	}
	*/
}
