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
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Xna.Framework;
using SkiaSharp;
using SpriteMaster.Types;
using System.Runtime.CompilerServices;

namespace Benchmarks.Sprites.Benchmarks;

public class Premultiply : Textures8 {
	[Benchmark(Description = "SkiaSharp", Baseline = true)]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void Skia(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var rawPixels = SKPMColor.PreMultiply(data.Span.Cast<byte, SKColor>().ToArray());

			var pixels = GC.AllocateUninitializedArray<Color>(rawPixels.Length);
			for (int i = 0; i < pixels.Length; i++) {
				SKPMColor pixel = rawPixels[i];
				pixels[i] = pixel.Alpha == 0
					? Color.Transparent
					: new Color(r: pixel.Red, g: pixel.Green, b: pixel.Blue, alpha: pixel.Alpha);
			}

			data.TempReference = pixels;
		}
	}

	[Benchmark(Description = "SkiaSharp (No Copy)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void SkiaNoCopy(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			var rawPixels = SKPMColor.PreMultiply(data.Span.Cast<byte, SKColor>().ToArray());

			var pixels = rawPixels.AsSpan().Cast<SKPMColor, Color>();
			for (int i = 0; i < pixels.Length; i++) {
				SKPMColor pixel = rawPixels[i];
				pixels[i] = pixel.Alpha == 0
					? Color.Transparent
					: new Color(r: pixel.Red, g: pixel.Green, b: pixel.Blue, alpha: pixel.Alpha);
			}

			data.TempReference = rawPixels;
		}
	}

	[Benchmark(Description = "Scalar (SMAPI)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarSMAPI(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			ProcessTextureScalarSMAPI(data.Span.Cast<byte, Color8>());
		}
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ProcessTextureScalarSMAPI(Span<Color8> data) {
		for (int i = 0; i < data.Length; i++) {
			var pixel = data[i];
			var alpha = pixel.A;

			// Transparent to zero (what XNA and MonoGame do)
			if (alpha == 0) {
				Unsafe.AsRef(data[i].Packed) = 0;
				continue;
			}

			// Premultiply
			if (alpha == byte.MaxValue)
				continue; // no need to change fully transparent/opaque pixels

			data[i] = new(
				pixel.R * pixel.A / byte.MaxValue,
				pixel.G * pixel.A / byte.MaxValue,
				pixel.B * pixel.A / byte.MaxValue,
				pixel.A
			);
		}
	}

	[Benchmark(Description = "Scalar")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void Scalar(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			SpriteMaster.Caching.TextureFileCache.ProcessTextureScalar(data.Span.Cast<byte, Color8>());
		}
	}

	[Benchmark(Description = "SSE2 (Unrolled)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void Sse2Unrolled(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			SpriteMaster.Caching.TextureFileCache.ProcessTextureSse2Unrolled(data.Span.Cast<byte, Color8>());
		}
	}

	[Benchmark(Description = "AVX2")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void Avx2(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			SpriteMaster.Caching.TextureFileCache.ProcessTextureAvx2(data.Span.Cast<byte, Color8>());
		}
	}
}
