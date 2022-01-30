/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using xBRZ;

namespace SpriteMaster.Tools;

public static class XBRZProgram {
	[STAThread]
	public static int Main(string[] args) {
		if (args.Contains("--ui") || args.Contains("--preview")) {
			return PreviewProgram.SubMain(args);
		}
		else {
			return ConverterProgram.SubMain(args);
		}

		return 0;
	}
}
