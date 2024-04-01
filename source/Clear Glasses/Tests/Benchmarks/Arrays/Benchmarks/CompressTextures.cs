/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using BenchmarkDotNet.Attributes;
using MonoGame.Framework.Utilities;
using SpriteMaster.Extensions;
using System.Runtime.CompilerServices;
using ZstdNet;

// ReSharper disable AccessToStaticMemberViaDerivedType

namespace Benchmarks.Arrays.Benchmarks;

public class CompressTextures : Textures {
	#region Zstd Default

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static long ZstdDefaultImpl(in SpriteDataSet dataSet, int compressionLevel) {
		using var compressionOptions = new CompressionOptions(compressionLevel);
		using var compressor = new ZstdNet.Compressor(compressionOptions);

		long length = 0;

		foreach (var data in dataSet.Data) {
			length += compressor.Wrap(data.Span).Length;
		}

		return length;
	}

	[Benchmark(Description = "Zstd.Default[default]", Baseline = true)]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long ZstdDefaultDefault(in SpriteDataSet dataSet) {
		return ZstdDefaultImpl(dataSet, CompressionOptions.DefaultCompressionLevel);
	}

	[Benchmark(Description = "Zstd.Default[min]")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long ZstdDefaultMin(in SpriteDataSet dataSet) {
		return ZstdDefaultImpl(dataSet, CompressionOptions.MinCompressionLevel);
	}

	/*
	[Benchmark(Description = "Zstd.Default[max]")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long ZstdDefaultMax(in SpriteDataSet dataSet) {
		return ZstdDefaultImpl(dataSet, CompressionOptions.MaxCompressionLevel);
	}
	*/

	#endregion

	#region Zlib

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static long ZlibDefaultImpl(in SpriteDataSet dataSet, CompressionLevel compressionLevel) {
		long length = 0;

		foreach (var data in dataSet.Data) {
			using var outStream = new MemoryStream(data.Span.Length);
			using (var compressStream = new ZlibStream(outStream, CompressionMode.Compress, compressionLevel)) {
				compressStream.Write(data.Reference, 0, data.Span.Length);
			}

			var result = outStream.GetArray();
			length += result.Length;
		}

		return length;
	}

	[Benchmark(Description = "Zlib[default]")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long ZlibDefault(in SpriteDataSet dataSet) {
		return ZlibDefaultImpl(dataSet, CompressionLevel.Default);
	}

	[Benchmark(Description = "Zlib[min]")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long ZlibMin(in SpriteDataSet dataSet) {
		return ZlibDefaultImpl(dataSet, CompressionLevel.BestSpeed);
	}

	[Benchmark(Description = "Zlib[max]")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long ZlibMax(in SpriteDataSet dataSet) {
		return ZlibDefaultImpl(dataSet, CompressionLevel.BestCompression);
	}

	#endregion

	#region MonoGame DeflateStream

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static long MGDeflateDefaultImpl(in SpriteDataSet dataSet, MonoGame.Framework.Utilities.Deflate.CompressionLevel compressionLevel) {
		long length = 0;

		foreach (var data in dataSet.Data) {
			using var outStream = new MemoryStream(data.Span.Length);
			using (var compressStream = new MonoGame.Framework.Utilities.Deflate.DeflateStream(outStream, MonoGame.Framework.Utilities.Deflate.CompressionMode.Compress, compressionLevel)) {
				compressStream.Write(data.Reference, 0, data.Span.Length);
			}

			var result = outStream.GetArray();
			length += result.Length;
		}

		return length;
	}

	[Benchmark(Description = "MonoGame.DeflateStream[default]")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long MGDeflateDefault(in SpriteDataSet dataSet) {
		return MGDeflateDefaultImpl(dataSet, MonoGame.Framework.Utilities.Deflate.CompressionLevel.Default);
	}

	[Benchmark(Description = "MonoGame.DeflateStream[min]")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long MGDeflateMin(in SpriteDataSet dataSet) {
		return MGDeflateDefaultImpl(dataSet, MonoGame.Framework.Utilities.Deflate.CompressionLevel.BestSpeed);
	}

	[Benchmark(Description = "MonoGame.DeflateStream[max]")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long MGDeflateMax(in SpriteDataSet dataSet) {
		return MGDeflateDefaultImpl(dataSet, MonoGame.Framework.Utilities.Deflate.CompressionLevel.BestCompression);
	}

	#endregion

	#region System.IO

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static long IODeflateDefaultImpl(in SpriteDataSet dataSet, System.IO.Compression.CompressionLevel compressionLevel) {
		long length = 0;

		foreach (var data in dataSet.Data) {
			using var outStream = new MemoryStream(data.Span.Length);
			using (var compressStream = new System.IO.Compression.DeflateStream(outStream, compressionLevel)) {
				compressStream.Write(data.Reference, 0, data.Span.Length);
			}

			var result = outStream.GetArray();
			length += result.Length;
		}

		return length;
	}

	[Benchmark(Description = "IO.DeflateStream[optimal]")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long IODeflateOptimal(in SpriteDataSet dataSet) {
		return IODeflateDefaultImpl(dataSet, System.IO.Compression.CompressionLevel.Optimal);
	}

	[Benchmark(Description = "IO.DeflateStream[min]")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long IODeflateMin(in SpriteDataSet dataSet) {
		return IODeflateDefaultImpl(dataSet, System.IO.Compression.CompressionLevel.Fastest);
	}

	/*
	[Benchmark(Description = "Zlib[max]")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long ZlibMax(in SpriteDataSet dataSet) {
		return ZlibDefaultImpl(dataSet, CompressionLevel.BestCompression);
	}
	*/

	#endregion
}
