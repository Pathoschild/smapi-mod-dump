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
using Microsoft.Toolkit.HighPerformance;
using SpriteMaster.Hashing.Algorithms;
using System.Net.Sockets;
using System.Numerics;
using System.Text;

namespace Hashing;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.Default, MethodOrderPolicy.Alphabetical)]
//[InliningDiagnoser(true, true)]
//[TailCallDiagnoser]
//[EtwProfiler]
//[SimpleJob(RuntimeMoniker.CoreRt50)]
public class AlgorithmsString {
	private const int RandSeed = 0x13377113;
	private const int MinSize = 1;
	private const int MaxSize = 65_536;

	public readonly struct DataSet {
		public readonly string Data;

		public DataSet(string data) => Data = data;

		public override string ToString() => Data.Length.ToString();
	}

	public List<DataSet> DataSets { get; init; } = new();

	public int currentDataSet;

	private static readonly char[] Chars =
		"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();


	public AlgorithmsString() {
		var random = new Random(RandSeed);

		{
			var sb = new StringBuilder();
			for (int j = 0; j < 0; ++j) {
				sb.Append(Chars[random.Next(0, Chars.Length)]);
			}

			DataSets.Add(new(sb.ToString()));
		}

		for (int i = MinSize; i <= MaxSize; i *= 2) {
			var sb = new StringBuilder();
			for (int j = 0; j < i; ++j) {
				sb.Append(Chars[random.Next(0, Chars.Length)]);
			}

			DataSets.Add(new(sb.ToString()));
		}

		if (DataSets.Last().Data.Length != MaxSize) {
			var sb = new StringBuilder();
			for (int j = 0; j < MaxSize; ++j) {
				sb.Append(Chars[random.Next(0, Chars.Length)]);
			}

			DataSets.Add(new(sb.ToString()));
		}
	}

	/*
	[Benchmark(Description = "xxHash3", Baseline = true)]
	[ArgumentsSource(nameof(DataSets))]
	public ulong xxHash3(in DataSet dataSet) {
		return SpriteMaster.Hashing.XXHash3.Hash64(dataSet.Data);
	}
	*/

	[Benchmark(Description = "xxHash3")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong xxHash3(DataSet dataSet) {
		return XxHash3.Hash64(dataSet.Data);
	}

	[Benchmark(Description = "DJB2")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public int DJB2(in DataSet dataSet) {
		return dataSet.Data.GetDjb2HashCode();
	}

	[Benchmark(Description = "GetHashCode")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public int HashCode(in DataSet dataSet) {
		return dataSet.Data.GetHashCode();
	}

	private static ulong FNV1aSpan64(ReadOnlySpan<byte> data) {
		ulong hash = 0xcbf29ce484222325UL;
		foreach (var octet in data) {
			hash ^= octet;
			hash *= 0x00000100000001B3UL;
		}
		return hash;
	}

	private static uint FNV1aSpan32(ReadOnlySpan<byte> data) {
		uint hash = 0x811c9dc5U;
		foreach (var octet in data) {
			hash ^= octet;
			hash *= 0x01000193U;
		}
		return hash;
	}

	[Benchmark(Description = "FNV1a-64")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong FNV1a64(in DataSet dataSet) {
		return FNV1aSpan64(dataSet.Data.AsSpan().AsBytes());
	}

	[Benchmark(Description = "FNV1a-32")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public uint FNV1a32(in DataSet dataSet) {
		return FNV1aSpan32(dataSet.Data.AsSpan().AsBytes());
	}

	private static uint SDBM32(ReadOnlySpan<byte> data) {
		uint hash = 0;
		foreach (var octet in data) {
			hash = octet + (hash << 6) + (hash << 16) - hash;
		}
		return hash;
	}

	[Benchmark(Description = "SDBM-32")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public uint SBDM32(in DataSet dataSet) {
		return SDBM32(dataSet.Data.AsSpan().AsBytes());
	}

	private static uint DJB2a32(ReadOnlySpan<byte> data) {
		uint hash = 5381U;
		foreach (var octet in data) {
			hash = hash * 33 ^ octet;
		}
		return hash;
	}

	[Benchmark(Description = "Manual DJBa-32")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public uint DJBa32(in DataSet dataSet) {
		return DJB2a32(dataSet.Data.AsSpan().AsBytes());
	}

	private static uint DJB32(ReadOnlySpan<byte> data) {
		uint hash = 5381U;
		foreach (var octet in data) {
			hash = hash * 33 + octet;
		}
		return hash;
	}

	[Benchmark(Description = "Manual DJB-32")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public uint DJB32(in DataSet dataSet) {
		return DJB32(dataSet.Data.AsSpan().AsBytes());
	}

	private static uint DJBb32(ReadOnlySpan<byte> data) {
		uint hash = 5381U;
		foreach (var octet in data) {
			hash = ((hash << 5) + hash) ^ octet;
		}
		return hash;
	}

	[Benchmark(Description = "Manual DJBb-32")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public uint DJBb32(in DataSet dataSet) {
		return DJBb32(dataSet.Data.AsSpan().AsBytes());
	}
	private static uint DJBb32Unrolled(ReadOnlySpan<byte> data) {
		uint hash = 5381U;
		int index = 0;
		while (data.Length - index >= 8) {
			var octet0 = data[index + 0];
			var octet1 = data[index + 1];
			var octet2 = data[index + 2];
			var octet3 = data[index + 3];
			var octet4 = data[index + 4];
			var octet5 = data[index + 5];
			var octet6 = data[index + 6];
			var octet7 = data[index + 7];
			hash = ((hash << 5) + hash) ^ octet0;
			hash = ((hash << 5) + hash) ^ octet1;
			hash = ((hash << 5) + hash) ^ octet2;
			hash = ((hash << 5) + hash) ^ octet3;
			hash = ((hash << 5) + hash) ^ octet4;
			hash = ((hash << 5) + hash) ^ octet5;
			hash = ((hash << 5) + hash) ^ octet6;
			hash = ((hash << 5) + hash) ^ octet7;
			index += 8;
		}
		while (data.Length - index > 0) {
			hash = ((hash << 5) + hash) ^ data[index++];
		}
		return hash;
	}

	[Benchmark(Description = "Manual DJBb-32 (Unrolled)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public uint DJBb32Unrolled(in DataSet dataSet) {
		return DJBb32Unrolled(dataSet.Data.AsSpan().AsBytes());
	}

	private static uint LoseLose(ReadOnlySpan<byte> data) {
		uint hash = 0U;
		foreach (var octet in data) {
			hash += octet;
		}
		return hash;
	}

	[Benchmark(Description = "LoseLose")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public uint LoseLose(in DataSet dataSet) {
		return LoseLose(dataSet.Data.AsSpan().AsBytes());
	}

	private static uint NormanHash(ReadOnlySpan<byte> data) {
		uint hash = 0U;
		foreach (var octet in data) {
			hash = BitOperations.RotateRight(hash, 4) + octet;
		}
		return hash;
	}

	[Benchmark(Description = "NormanHash")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public uint NormanHash(in DataSet dataSet) {
		return NormanHash(dataSet.Data.AsSpan().AsBytes());
	}

	private static uint NormanHashUnrolled(ReadOnlySpan<byte> data) {
		uint hash = 0U;

		int index = 0;
		while (data.Length - index >= 8) {
			var octet0 = data[index + 0];
			var octet1 = data[index + 1];
			var octet2 = data[index + 2];
			var octet3 = data[index + 3];
			var octet4 = data[index + 4];
			var octet5 = data[index + 5];
			var octet6 = data[index + 6];
			var octet7 = data[index + 7];
			hash = BitOperations.RotateRight(hash, 4) + octet0;
			hash = BitOperations.RotateRight(hash, 4) + octet1;
			hash = BitOperations.RotateRight(hash, 4) + octet2;
			hash = BitOperations.RotateRight(hash, 4) + octet3;
			hash = BitOperations.RotateRight(hash, 4) + octet4;
			hash = BitOperations.RotateRight(hash, 4) + octet5;
			hash = BitOperations.RotateRight(hash, 4) + octet6;
			hash = BitOperations.RotateRight(hash, 4) + octet7;
			index += 8;
		}
		while (data.Length - index > 0) {
			hash = BitOperations.RotateRight(hash, 4) + data[index++];
		}

		return hash;
	}

	[Benchmark(Description = "NormanHash (Unrolled)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public uint NormanHashUnrolled(in DataSet dataSet) {
		return NormanHashUnrolled(dataSet.Data.AsSpan().AsBytes());
	}

	/*
	[Benchmark(Description = "FNV1a Span")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong FNV1aSpan(DataSet dataSet) {
		return FNV1aSpan(dataSet.Data);
	}

	[Benchmark(Description = "FNV1a Pointer")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public unsafe ulong FNV1aPtr(DataSet dataSet) {
		fixed (byte* ptr = dataSet.Data) {
			return FNV1aPtr(ptr, dataSet.Data.Length);
		}
	}
	*/
}
