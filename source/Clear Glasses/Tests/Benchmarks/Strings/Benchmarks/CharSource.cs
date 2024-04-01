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
using Benchmarks.BenchmarkBase.Benchmarks;
using Benchmarks.Strings.Benchmarks.Sources;

namespace Benchmarks.Strings.Benchmarks;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
public partial class CharSource : /*MultiStringSource<Dictionary, RandomText>*/Dictionary {
	#region CharSource0

	[Benchmark(Description = "CharSource0 (string)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long StringCharSource0(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource0(data.String);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	[Benchmark(Description = "CharSource0 (builder)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long BuilderCharSource0(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource0(data.Builder);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	[Benchmark(Description = "CharSource0 (random)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long RandomCharSource0(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource0(data.Reference);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	#endregion

	#region CharSource1

	[Benchmark(Description = "CharSource1 (string)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long StringCharSource1(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource1(data.String);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	[Benchmark(Description = "CharSource1 (builder)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long BuilderCharSource1(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource1(data.Builder);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	[Benchmark(Description = "CharSource1 (random)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long RandomCharSource1(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource1(data.Reference);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	#endregion

	#region CharSource2

	[Benchmark(Description = "CharSource2 (string)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long StringCharSource2(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource2(data.String);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	[Benchmark(Description = "CharSource2 (builder)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long BuilderCharSource2(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource2(data.Builder);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	[Benchmark(Description = "CharSource2 (random)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long RandomCharSource2(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource2(data.Reference);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	#endregion

	#region CharSource3

	[Benchmark(Description = "CharSource3 (string)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long StringCharSource3(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource3(data.String);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	[Benchmark(Description = "CharSource3 (builder)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long BuilderCharSource3(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource3(data.Builder);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	[Benchmark(Description = "CharSource3 (random)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long RandomCharSource3(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource3(data.Reference);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	#endregion

	#region CharSource4

	[Benchmark(Description = "CharSource4 (string)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long StringCharSource4(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource4(data.String);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	[Benchmark(Description = "CharSource4 (builder)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long BuilderCharSource4(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource4(data.Builder);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	[Benchmark(Description = "CharSource4 (random)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long RandomCharSource4(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource4(data.Reference);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	#endregion

	#region CharSource5

	[Benchmark(Description = "CharSource5 (string)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long StringCharSource5(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource5(data.String);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	[Benchmark(Description = "CharSource5 (builder)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long BuilderCharSource5(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource5(data.Builder);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	[Benchmark(Description = "CharSource5 (random)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long RandomCharSource5(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource5(data.Reference);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	#endregion

	#region CharSource6

	[Benchmark(Description = "CharSource6 (string)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long StringCharSource6(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource6(data.String);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	[Benchmark(Description = "CharSource6 (builder)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long BuilderCharSource6(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource6(data.Builder);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	[Benchmark(Description = "CharSource6 (random)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long RandomCharSource6(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource6(data.Reference);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	#endregion

	#region CharSource7

	[Benchmark(Description = "CharSource7 (string)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long StringCharSource7(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource7(data.String);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	[Benchmark(Description = "CharSource7 (builder)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long BuilderCharSource7(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource7(data.Builder);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	[Benchmark(Description = "CharSource7 (random)")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public long RandomCharSource7(in DataSet<StringData[]> dataSet) {
		long result = 0L;

		foreach (var data in dataSet.Data) {
			var source = new Impl.CharSource7(data.Reference);
			for (int i = 0; i < source.Length; ++i) {
				result += source[i];
			}

			result += source.Length;
		}

		return result;
	}

	#endregion
}
