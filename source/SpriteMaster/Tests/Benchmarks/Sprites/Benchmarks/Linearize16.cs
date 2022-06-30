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
using SpriteMaster.Types;

namespace Benchmarks.Sprites.Benchmarks;

public class Linearize16 : Textures<ushort> {
	[Benchmark(Description = "RefSpan", Baseline = true)]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void RefSpan(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.RefSpan.Linearize(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "RefSpan (static)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void RefSpanStatic(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.RefSpan.LinearizeStatic(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "RefSpan (converter)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void RefSpanConverter(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.RefSpan.LinearizeConverter(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "RefSpan (converterRef)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void RefSpanConverterRef(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.RefSpan.LinearizeConverterRef(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Fixed")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void Fixed(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.Fixed.Linearize(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Fixed (static)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void FixedStatic(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.Fixed.LinearizeStatic(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Fixed (converter)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void FixedConverter(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.Fixed.LinearizeConverter(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Fixed (converterCopy)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void FixedConverterCopy(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.Fixed.LinearizeConverterCopy(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Fixed (converterRef)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void FixedConverterRef(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.Fixed.LinearizeConverterRef(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Fixed (converterRefCopy)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void FixedConverterRefCopy(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.Fixed.LinearizeConverterRefCopy(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Fixed (converterRefCopy2)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void FixedConverterRefCopy2(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.Fixed.LinearizeConverterRefCopy2(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Fixed (converterRefCopy3)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void FixedConverterRefCopy3(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.Fixed.LinearizeConverterRefCopy3(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Fixed (converterRefCopy4)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void FixedConverterRefCopy4(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.Fixed.LinearizeConverterRefCopy4(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Fixed (converterRefCopy5)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void FixedConverterRefCopy5(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.Fixed.LinearizeConverterRefCopy5(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Fixed (converterRefCopy6)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void FixedConverterRefCopy6(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.Fixed.LinearizeConverterRefCopy6(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Fixed (converterRefCopy7)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void FixedConverterRefCopy7(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.Fixed.LinearizeConverterRefCopy7(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Fixed (converterRefCopy8)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void FixedConverterRefCopy8(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.Fixed.LinearizeConverterRefCopy8(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Fixed (converterRefCopy9)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void FixedConverterRefCopy9(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.Fixed.LinearizeConverterRefCopy9(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Fixed (uncached simd)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void FixedUncachedSimd(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.Fixed.LinearizeConverterUncachedSimd(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	/*
	[Benchmark(Description = "Fixed (uncached)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void FixedUncached(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.Fixed.LinearizeUncached(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Fixed (uncached single)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void FixedUncachedF(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.Fixed.LinearizeUncachedF(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Fixed (uncached unwrapped)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void FixedUncachedUnwrapped(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.Linearize.Fixed.LinearizeUncachedUnwrapped(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}
	*/
}
