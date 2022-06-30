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

public class ReversePremultiply16 : Textures<ushort> {
	#region ByRef
	[Benchmark(Description = "Scalar (ByRef, Switched)", Baseline = true)]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByRefSwitched(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByRef.Scalar.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByRef, Unrolled)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByRefUnrolled(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByRef.ScalarUnrolled.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByRef, Branched)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByRefBranched(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByRef.ScalarBranched.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByRef, Branchless)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByRefBranchless(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByRef.ScalarBranchless.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByRef, Ternary)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByRefTernary(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByRef.ScalarTernary.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByRef, Float)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByRefFloat(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByRef.ScalarFloat.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByRef, Init)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByRefInit(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByRef.ScalarInit.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}
	#endregion

	#region ByIndex
	[Benchmark(Description = "Scalar (ByIndex, Switched)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByIndexSwitched(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByIndex.Scalar.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByIndex, Unrolled)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByIndexUnrolled(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByIndex.ScalarUnrolled.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByIndex, Branched)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByIndexBranched(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByIndex.ScalarBranched.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByIndex, Branchless)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByIndexBranchless(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByIndex.ScalarBranchless.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByIndex, Ternary)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByIndexTernary(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByIndex.ScalarTernary.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByIndex, Float)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByIndexFloat(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByIndex.ScalarFloat.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByIndex, Init)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByIndexInit(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByIndex.ScalarInit.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}
	#endregion

	#region ByFixed
	[Benchmark(Description = "Scalar (ByFixed, Switched)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByFixedSwitched(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByFixed.Scalar.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByFixed, Switched, DeRef)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByFixedSwitched2(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByFixed.Scalar2.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByFixed, Switched, Increment)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByFixedSwitched3(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByFixed.Scalar3.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByFixed, Switched, CompareEnd)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByFixedSwitched4(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByFixed.Scalar4.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByFixed, Switched, CompareDiff)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByFixedSwitched5(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByFixed.Scalar5.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByFixed, Unrolled)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByFixedUnrolled(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByFixed.ScalarUnrolled.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByFixed, Branched)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByFixedBranched(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByFixed.ScalarBranched.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByFixed, Branchless)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByFixedBranchless(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByFixed.ScalarBranchless.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByFixed, Ternary)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByFixedTernary(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByFixed.ScalarTernary.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByFixed, Float)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByFixedFloat(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByFixed.ScalarFloat.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}

	[Benchmark(Description = "Scalar (ByFixed, Init)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public void ScalarByFixedInit(in SpriteDataSet dataSet) {
		foreach (var data in dataSet.Data) {
			Methods.ReversePremultiply16.ByFixed.ScalarInit.Reverse(data.Span.Cast<ushort, Color16>(), data.Size);
		}
	}
	#endregion
}
