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
using Benchmarks.BenchmarkBase;
using Benchmarks.BenchmarkBase.Benchmarks;
using LinqFasterer;
using Microsoft.Toolkit.HighPerformance;
using SkiaSharp;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using SpriteMaster;
using SpriteMaster.Types.Fixed;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Benchmarks.Sprites.Benchmarks;
public abstract class Textures<TElement> : BenchmarkBase<Textures<TElement>.SpriteDataSet, Textures<TElement>.SpriteData[]> where TElement : unmanaged {
	private const int RandomSeed = 0x13371337;
	private const int ShortLength = 50;

	[GlobalSetup]
	public virtual void AlwaysRunBefore() {
	}

	private sealed class XorShiftRandom {
		private ulong State;

		internal XorShiftRandom(ulong seed) {
			if (seed == 0UL) {
				seed = 0x1337133773317331UL;
			}

			State = seed;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ulong Shift() {
			ulong x = State;
			x ^= x << 13;
			x ^= x >> 7;
			x ^= x << 17;
			return State = x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private double Sample() {
			return (double)Shift() / ulong.MaxValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int NextPow2(int difference) {
			var sample = Shift();
			return (int)sample & (difference - 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal int Next(int minValue, int maxValue) {
			int difference = maxValue - minValue;
			switch (difference) {
				case 1:
					return minValue;
				case 2:
					return NextPow2(2) + minValue;
				case 4:
					return NextPow2(4) + minValue;
				case 8:
					return NextPow2(8) + minValue;
				default:
					if (BitOperations.PopCount((uint)difference) == 1) {
						return NextPow2(difference) + minValue;
					}

					break;
			}
			var sample = Sample();
			return ((int)(sample * difference)) + minValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal int Next(int maxValue) {
			switch (maxValue) {
				case 1:
					return 0;
				case 2:
					return NextPow2(2);
				case 4:
					return NextPow2(4);
				case 8:
					return NextPow2(8);
				default:
					if (BitOperations.PopCount((uint)maxValue) == 1) {
						return NextPow2(maxValue);
					}

					break;
			}
			var sample = Sample();
			return (int)(sample * maxValue);
		}
	}

	private static readonly string[] Roots = {
		@"D:\Stardew\Reference\Content",
		@"D:\Stardew\root_mods"
	};

	public class SpriteData {
		internal readonly string Path;
		public Memory<TElement> Data { get; private set; }
		public Span<TElement> Span => Data.Span;
		internal object? TempReference = null;
		internal readonly TElement[] Reference;

		internal readonly Vector2I Size;

		public void Setup() {
			TempReference = null;
			Data = new(Reference.CloneFast());
		}

		public void Cleanup() {
			TempReference = null;
		}

		internal SpriteData(string path, ReadOnlyMemory<TElement> data, Vector2I size) {
			Path = path;
			Reference = GC.AllocateUninitializedArray<TElement>((int)data.Length);
			data.CopyTo(Reference);
			Data = new(Reference);
			Size = size;

			if (data.Length != (Size.Width * 4) * Size.Height) {
				ThrowHelper.ThrowArgumentException($"data length {data.Length} != {(Size.Width * 4) * Size.Height}", nameof(data));
			}
		}
	}

	[StructLayout(LayoutKind.Auto)]
	public readonly struct SpriteDataSet : IDataSet<SpriteData[]> {
		public readonly SpriteData[] Data { get; }

		private readonly uint Index => (Data.Length == 0) ? 0u : (uint)BitOperations.Log2((uint)Data.Length) + 1u;

		internal SpriteDataSet(ReadOnlySpan<SpriteData> data) {
			Data = data.ToArray();
		}

		public override readonly string ToString() => $"{Data.Length}";
	}

	private static ReadOnlySpan<Vector2I> SmallSizes => new Vector2I[] {
		(16, 16),
		(16, 32),
		(32, 32),
		(16 * 3, 32 * 5)
	};

	private sealed class LocalSpriteData<T> where T : unmanaged {
		private readonly string Path;
		private string Name;
		private readonly XorShiftRandom Rand;
		internal readonly SpriteData? Data = null;

		internal LocalSpriteData(ReadOnlySpan<Vector2I> smallSizes, string path) {
			Path = path;
			Name = path;
			Rand = new(path.GetLongHashCode());

			Data = Preload(smallSizes);
		}

		private Vector2I? GetSpriteSubsetIndex(ReadOnlySpan<Vector2I> smallSizes, in SKImageInfo bitmap) {
			int len = smallSizes.Length;
			int subsetIndex = Rand.Next(len);
			Vector2I subset = smallSizes[subsetIndex];

			if (bitmap.Width < subset.Width || bitmap.Height < subset.Height) {
				return null;
			}

			return subset;
		}

		private SpriteData? Preload(ReadOnlySpan<Vector2I> smallSizes) {
			var bitmapInfo = SKBitmap.DecodeBounds(File.OpenRead(Path));

			byte[] data;
			Vector2I size = (bitmapInfo.Width, bitmapInfo.Height);

			if (Program.CurrentOptions.Small) {

				if (GetSpriteSubsetIndex(smallSizes, bitmapInfo) is not { } subset) {
					return null;
				}

				if (bitmapInfo.Width < subset.Width || bitmapInfo.Height < subset.Height) {
					return null;
				}

				using FileStream stream = File.OpenRead(Path);
				using var refBitmap = SKBitmap.Decode(stream);
				if (refBitmap is null) {
					return null;
				}

				Vector2I alignedOffsetBasis = (new Vector2I(refBitmap.Width, refBitmap.Height) / subset);
				Vector2I alignedOffset = new Vector2I(
					Rand.Next(0, alignedOffsetBasis.X),
					Rand.Next(0, alignedOffsetBasis.Y)
				);
				Vector2I offset = alignedOffset * subset;

				using var subsetBitmap = new SKBitmap(
					new SKImageInfo(subset.Width, subset.Height, refBitmap.ColorType, refBitmap.AlphaType, refBitmap.ColorSpace)
				);

				if (!refBitmap.ExtractSubset(subsetBitmap, SKRectI.Create(offset.X, offset.Y, subset.Width, subset.Height))) {
					return null;
				}

				data = subsetBitmap.Pixels.AsReadOnlySpan().AsBytes().ToArray();
				size = subset;
				Name = $"{Path}::{offset}:{subset}";
			}
			else {
				using FileStream stream = File.OpenRead(Path);
				using var bitmap = SKBitmap.Decode(stream);
				if (bitmap is null) {
					return null;
				}
				data = bitmap.Bytes;
			}

			// Convert from u8 to u16, if necessary
			if (typeof(TElement) == typeof(ushort) || typeof(TElement) == typeof(short) || typeof(TElement) == typeof(Fixed16)) {
				var data16 = Color16.Convert(data.AsReadOnlySpan<Color8>()).Cast<Color16, TElement>().ToArray();
				return new(Name, data16, size);
			}

			return new(Name, (TElement[])(object)data, size);
		}
	}

	static Textures() {
		Console.WriteLine($"Short: {Program.CurrentOptions.Short}");
		Console.WriteLine($"Small: {Program.CurrentOptions.Small}");

		var random = new Random(RandomSeed);

		string[] roots;
		if (Program.CurrentOptions.Short) {
			roots = new[] { Roots[0] };
		}
		else {
			roots = Roots;
		}


		var allImages = roots.SelectManyF(dir => Directory.EnumerateFiles(dir, "*.png", SearchOption.AllDirectories).ToArray()).ToArray();

		allImages = allImages.ShuffleF(random);

		List<LocalSpriteData<TElement>> allSpriteDataObjects = new(allImages.Length);

		Console.WriteLine($"Processing Image Data ({allImages.Length})");

		ulong lastPercent = 0;

		var smallSizes = SmallSizes.ToArray();

		var printStopwatch = Stopwatch.StartNew();

		Parallel.ForEach(
			allImages, image => {
				var result = new LocalSpriteData<TElement>(smallSizes, image);
				ulong count;
				lock (allSpriteDataObjects) {
					if (result.Data is not null) {
						allSpriteDataObjects.Add(result);
					}

					count = (ulong)allSpriteDataObjects.Count;
				}

				count *= 1000;

				ulong permille = count / (ulong)allImages.Length;
				ulong percent = permille / 10;
				if (Interlocked.Exchange(ref lastPercent, percent) < percent) {
					if ((permille % 100) == 0) {
						if (printStopwatch.ElapsedMilliseconds >= 500) {
							Console.WriteLine($"{percent}%...");
							printStopwatch.Restart();
						}
					}
				}
			}
		);

		var dataList = allSpriteDataObjects.Select(data => data.Data!).ToArray();

		Console.WriteLine("Image Data Processed");

		if (Program.CurrentOptions.Short && dataList.Length >= ShortLength) {
			dataList = dataList.OrderByF(sd => sd.Path).ShuffleF(random).ToArray();
			dataList = dataList.Take(ShortLength).ToArray();
		}

		dataList = dataList.OrderByF(sd => sd.Path).ToArrayF();

		DefaultDataSetsStatic.Add(new(dataList));
	}

	[IterationSetup]
	public void IterationSetup() {
		foreach (var dataSet in DataSets) {
			foreach (var data in dataSet.Data) {
				data.Setup();
			}
		}
		GC.Collect(int.MaxValue, GCCollectionMode.Forced, blocking: true, compacting: true);
	}

	[IterationCleanup]
	public void IterationCleanup() {
		foreach (var dataSet in DataSets) {
			foreach (var data in dataSet.Data) {
				data.Cleanup();
			}
		}
		GC.Collect(int.MaxValue, GCCollectionMode.Forced, blocking: true, compacting: true);
	}
}
