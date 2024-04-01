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
using BenchmarkDotNet.Order;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace BoolParse;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.Declared, MethodOrderPolicy.Declared)]
//[InliningDiagnoser(true, true)]
//[TailCallDiagnoser]
//[EtwProfiler]
//[SimpleJob(RuntimeMoniker.CoreRt50)]
public class CheckValue : Common {
	[Benchmark(Description = "Bool.TryParse")]
	[ArgumentsSource(nameof(DataSets))]
	public bool TryParse(string[] data) {
		bool result = false;
		foreach (var item in data) {
			if (bool.TryParse(item, out var value)) {
				result ^= value;
			}
		}

		return result;
	}

	[Benchmark(Description = "Simple")]
	[ArgumentsSource(nameof(DataSets))]
	public bool Simple(string[] data) {
		bool result = false;
		foreach (var item in data) {
			if (item is "true" or "True") {
				result ^= true;
			}
			else if (item is "false" or "False") {
				result ^= false;
			}
		}

		return result;
	}
}
