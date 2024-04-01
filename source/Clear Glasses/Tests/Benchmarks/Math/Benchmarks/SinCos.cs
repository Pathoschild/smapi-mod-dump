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
using Benchmarks.Math.Benchmarks.Sources;
using SpriteMaster.Extensions;

namespace Benchmarks.Math.Benchmarks;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
public class SinCos : RandomReals {
	[Benchmark(Description = "SinCosF", Baseline = true)]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public float SinCosF(in DataSet<RealData[]> dataSet) {
		float sinSum = 0.0f;
		float cosSum = 0.0f;

		foreach (var real in dataSet.Data) {
			var (sin, cos) = MathExt.SinCos(real.Single);
			sinSum += sin;
			cosSum += cos;
		}

		return sinSum + cosSum;
	}

	[Benchmark(Description = "SinCosD")]
	[ArgumentsSource(nameof(DataSets), Priority = 0)]
	public double SinCosD(in DataSet<RealData[]> dataSet) {
		double sinSum = 0.0;
		double cosSum = 0.0;

		foreach (var real in dataSet.Data) {
			var (sin, cos) = MathExt.SinCos(real.Double);
			sinSum += sin;
			cosSum += cos;
		}

		return sinSum + cosSum;
	}
}
