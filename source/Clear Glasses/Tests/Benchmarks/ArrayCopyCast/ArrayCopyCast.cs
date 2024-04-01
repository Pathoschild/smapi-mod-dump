/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using BenchmarkDotNet.Running;

namespace ArrayCopyCast;

public class ArrayCopyCast {
	//[STAThread]
	public static int Main(string[] args) {
		var summary = BenchmarkRunner.Run<Algorithms>();
		return 0;
	}
}