/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Hashing;

public class Hashing {
	//[STAThread]
	public static int Main(string[] args) {
		var config = DefaultConfig.Instance
			.WithOptions(ConfigOptions.Default)
			.WithOptions(ConfigOptions.DisableOptimizationsValidator);
		_ = BenchmarkRunner.Run<Algorithms>(config);
		//_ = BenchmarkRunner.Run<AlgorithmsString>(config);
		return 0;
	}
}