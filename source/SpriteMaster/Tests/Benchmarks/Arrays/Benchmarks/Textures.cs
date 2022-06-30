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
using SkiaSharp;
using SpriteMaster.Extensions;
using System.Collections.Concurrent;
using System.Numerics;

namespace Benchmarks.Arrays.Benchmarks;

public class Textures : BenchmarkBase<Textures.SpriteDataSet, Textures.SpriteData[]> {
	public class SpriteData : IDisposable {
		internal readonly string Path;
		public Memory<byte> Data { get; private set; }
		public Span<byte> Span => Data.Span;
		internal object? TempReference = null;
		internal readonly byte[] Reference;
		internal SKBitmap Image;

		public void Setup() {
			TempReference = null;
			Data = new(Reference.CloneFast());
		}

		public void Cleanup() {
			TempReference = null;
		}

		internal SpriteData(string path, ReadOnlyMemory<byte> data, SKBitmap image) {
			Path = path;
			Reference = GC.AllocateUninitializedArray<byte>((int)data.Length);
			data.CopyTo(Reference);
			Data = new(Reference);
			Image = image;
		}

		public void Dispose() {
			Image?.Dispose();
		}
	}

	public readonly struct SpriteDataSet : IDataSet<SpriteData[]> {
		public readonly SpriteData[] Data { get; }

		private readonly uint Index => (Data.Length == 0) ? 0u : (uint)BitOperations.Log2((uint)Data.Length) + 1u;

		internal SpriteDataSet(ReadOnlySpan<SpriteData> data) {
			Data = data.ToArray();
		}

		public override readonly string ToString() => $"{Data.Length}";
	}

	static Textures() {
		const string ContentRoot = @"D:\Stardew\Reference\Content";
		const string ModRoot = @"D:\Stardew\root_mods";

		var allImages = new[] { ContentRoot, ModRoot }.SelectMany(dir => Directory.EnumerateFiles(dir, "*.png", SearchOption.AllDirectories)).ToArray();

		ConcurrentBag<SpriteData> allSpriteDatas = new();


		Parallel.ForEach(
			allImages, image => {
				using FileStream stream = File.OpenRead(image);
				SKBitmap bitmap = SKBitmap.Decode(stream);
				if (bitmap is not null) {
					var data = bitmap.Bytes;
					allSpriteDatas.Add(new(image, data, bitmap));
				}
			}
		);

		var dataList = allSpriteDatas.OrderBy(sd => sd.Path).ToArray();

		DataSets.Add(new(dataList));
	}

	[IterationSetup]
	public void IterationSetup() {
		foreach (var dataSet in DataSets) {
			foreach (var data in dataSet.Data) {
				data.Setup();
			}
		}
	}

	[IterationCleanup]
	public void IterationCleanup() {
		foreach (var dataSet in DataSets) {
			foreach (var data in dataSet.Data) {
				data.Cleanup();
			}
		}
	}
}
