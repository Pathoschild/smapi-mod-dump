/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using BenchmarkDotNet.Running;

namespace Texture;

public class Texture {
	//[STAThread]
	public static int Main(string[] args) {
		_ = BenchmarkRunner.Run<Tests>();
		return 0;
	}
}