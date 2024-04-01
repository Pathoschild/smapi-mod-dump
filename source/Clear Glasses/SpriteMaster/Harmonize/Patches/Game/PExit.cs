/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Configuration;
using System.Diagnostics;

namespace SpriteMaster.Harmonize.Patches.Game;

internal static class PExit {
	[Harmonize(
		typeof(StardewValley.InstanceGame),
		"Exit",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	public static bool Exit() {
		if (!Config.IsUnconditionallyEnabled || !Config.Extras.FastQuit) {
			return true;
		}

		Process.GetCurrentProcess().Kill();

		return false;
	}
}
