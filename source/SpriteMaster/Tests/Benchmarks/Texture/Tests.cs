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
using BenchmarkDotNet.Order;
using System.Runtime.CompilerServices;
using System.Text;

namespace Texture;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.Declared, MethodOrderPolicy.Declared)]
//[InliningDiagnoser(true, true)]
//[TailCallDiagnoser]
//[EtwProfiler]
//[SimpleJob(RuntimeMoniker.CoreRt50)]
public class Tests : Common {
	[Benchmark(Description = "Bool.TryParse")]
	[ArgumentsSource(nameof(DataSets))]
	public bool TryParse(string[] data) {
		bool result = false;
		foreach (var item in data) {
			result ^= bool.TryParse(item, out _);
		}

		return result;
	}

	[Benchmark(Description = "Simple")]
	[ArgumentsSource(nameof(DataSets))]
	public bool Simple(string[] data) {
		bool result = false;
		foreach (var item in data) {
			result ^= item is "true" or "True" or "false" or "False";
		}

		return result;
	}
}
