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
using Benchmarks.BenchmarkBase.Benchmarks;
using LinqFasterer;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System.Numerics;
using static Benchmarks.Hashing.Benchmarks.Sprites;

namespace Benchmarks.Hashing.Benchmarks;
public class Sprites : BenchmarkBase<SpriteDataSet, ReadOnlyMemory2D<byte>> {
	private const int RandSeed = 0x13377113;

	private static SpriteDataSet MakeAlignedSprite(Random random, Vector2I size) {
		var data = GC.AllocateUninitializedArray<byte>(size.Area * 4);
		var dataBytes = data.AsSpan().AsBytes();
		random.NextBytes(dataBytes);
		return new(
			true,
			new(
				data,
				height: size.Height,
				width: size.Width * 4
			)
		);
	}

	private static SpriteDataSet MakeUnalignedSprite(Random random, Vector2I size, Vector2I innerSize) {
		Vector2I offset = (size - innerSize) / 2;
		int offsetIndex = (offset.Height * size.Width) + offset.Width;

		var data = GC.AllocateUninitializedArray<byte>(size.Area * 4);
		var dataBytes = data.AsSpan().AsBytes();
		random.NextBytes(dataBytes);
		return new(
			false,
			new(
				data,
				width: innerSize.Width * 4,
				height: innerSize.Height,
				offset: offsetIndex,
				pitch: (size.Width - innerSize.Width) * 4
			)
		);
	}

	public readonly struct SpriteDataSet : IDataSet<ReadOnlyMemory2D<byte>> {
		public readonly ReadOnlyMemory2D<byte> Data { get; }
		internal readonly ReadOnlySpan2D<byte> Span => Data.Span;
		internal readonly byte[] Reference; 
		internal readonly bool Aligned;

		private readonly uint Index => (Data.Length == 0) ? 0u : (uint)BitOperations.Log2((uint)Data.Length) + 1u;

		internal SpriteDataSet(bool aligned, ReadOnlyMemory2D<byte> data) {
			Data = data;
			Reference = GC.AllocateUninitializedArray<byte>((int)Data.Length);
			Data.CopyTo(Reference);
			Aligned = aligned;
		}

		public override readonly string ToString() => $"{Data.Width / 4}x{Data.Height} {(Aligned ? "Aligned" : "Unaligned")}";
	}

	private static readonly Vector2I[] CommonSizes = {
		//(2048, 1),
		(1, 2048),
		(2048, 2048),
		//(1, 1),
		//(4, 4),
		//(4, 128),
		//(128, 4),
		//(48, 32),
		//(16, 16),
		//(16, 32),
		//(32, 16),
		//(32, 32),
		//(48, 32),
		//(16, 64),
		//(64, 16),
		//(64, 64),
		//(16 * 3, 16 * 5)
	};


	private static void AddErrorCases(Random random) {
		{
			// error case
			Vector2I outerDimensions = (704, 2256);
			uint[] data = GC.AllocateUninitializedArray<uint>(outerDimensions.Area);
			random.NextBytes(data.AsSpan().AsBytes());
			Bounds innerBounds = new((703, 1912), (1, 264));

			ReadOnlySpan<byte> dataBytes = data.AsSpan().AsBytes();

			var format = SurfaceFormat.Color;
			var actualWidth = format.SizeBytes(innerBounds.Extent.X);
			var rawStride = format.SizeBytes(outerDimensions.Width);
			var rawOffset = (rawStride * innerBounds.Top) + format.SizeBytes(innerBounds.Left);

			SpriteDataSet set = new(
				false,
				new(
					array: dataBytes.ToArray(),
					offset: rawOffset,
					width: actualWidth,
					height: innerBounds.Extent.Y,
					pitch: rawStride - actualWidth
				)
			);
			DefaultDataSetsStatic.Add(set);
		}
	}

	// https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2
	private static int NextPower2(int value) {
		uint v = (uint)value; // compute the next highest power of 2 of 32-bit v

		v--;
		v |= v >> 1;
		v |= v >> 2;
		v |= v >> 4;
		v |= v >> 8;
		v |= v >> 16;
		v++;

		return (int)v;
	}

	private readonly struct SizeComparer : IComparer<Vector2I> {
		public readonly int Compare(Vector2I x, Vector2I y) {
			int result = x.Area.CompareTo(y.Area);
			if (result != 0) {
				return result;
			}

			result = x.X.CompareTo(y.X);
			if (result != 0) {
				return result;
			}

			result = x.Y.CompareTo(y.Y);
			if (result != 0) {
				return result;
			}

			return 0;
		}
	}

	static Sprites() {
		var random = new Random(RandSeed);

		//AddErrorCases(random);

		var testSizes = CommonSizes.SortF(new SizeComparer());

		foreach (var size in testSizes) {
			//DataSets.Add(MakeAlignedSprite(random, size));
		}

		Vector2I outerSize = (1024, 1024);
		foreach (var size in testSizes) {
			var tempOuterSize = outerSize;
			if (size.X >= tempOuterSize.X) {
				tempOuterSize.X = NextPower2(size.X + 1);
			}
			if (size.Y >= tempOuterSize.Y) {
				tempOuterSize.Y = NextPower2(size.Y + 1);
			}

			DefaultDataSetsStatic.Add(MakeUnalignedSprite(random, tempOuterSize, size));
		}


		if (Program.CurrentOptions.DoValidate) {
			bool error = false;
			Console.WriteLine("Performing Validation...");

			var referenceInstance = new Sprites();

			foreach (var dataSet in DefaultDataSetsStatic) {
				try {
					ulong baseline = referenceInstance.Baseline(dataSet);

					foreach (var hasher in new TestFunction[] {
						referenceInstance.Copy,
						referenceInstance.SegmentedSpan
					}) {
						ulong hash = hasher(dataSet);
						if (baseline != hash) {
							Console.Error.WriteLine($"{hasher.Method.Name} mismatch [{dataSet}]: {hash} != {baseline}");
							error = true;
						}
					}
				}
				catch (Exception ex) {
					Console.Error.WriteLine($"Exception validating {dataSet}: {ex.GetType().Name} {ex.Message}");
					error = true;
				}
			}

			if (error) {
				throw new Exception("Benchmark validation failed");
			}
		}
	}

	private delegate ulong TestFunction(in SpriteDataSet dataSet);

	//[Benchmark(Description = "Baseline", Baseline = true)]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong Baseline(in SpriteDataSet dataSet) {
		return SpriteMaster.Hashing.Algorithms.XxHash3.Hash64(dataSet.Reference);
	}

	//[Benchmark(Description = "Row by Row")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public ulong RowByRow(in SpriteDataSet dataSet) {
		var span = dataSet.Span;

		ulong hash = 0;
		for (int i = 0; i < span.Height; ++i) {
			var row = span.GetRowSpan(i);
			hash = SpriteMaster.Hashing.HashUtility.Accumulate(hash, SpriteMaster.Hashing.Algorithms.XxHash3.Hash64(row.AsBytes()));
		}

		return hash;
	}

	//[Benchmark(Description = "Copy")]
	[ArgumentsSource(nameof(DataSets), Priority = 1)]
	public ulong Copy(in SpriteDataSet dataSet) {
		var span = dataSet.Span;

		var copiedData = GC.AllocateUninitializedArray<byte>((int)span.Length);
		span.CopyTo(copiedData);

		return SpriteMaster.Hashing.Algorithms.XxHash3.Hash64(copiedData.AsSpan().AsBytes());
	}

	[Benchmark(Description = "SegmentedSpan")]
	[ArgumentsSource(nameof(DataSets), Priority = 2)]
	public ulong SegmentedSpan(in SpriteDataSet dataSet) {
		var span = dataSet.Span;

		return SpriteMaster.Hashing.Algorithms.XxHash3.Hash64(span);
	}
}
