/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using StardewValley;

namespace Leclair.Stardew.Common;

public static class TimeHelper {

	public static string FormatTime(int time) {
		// Limit it to one day.
		time %= 2400;

		return Game1.getTimeOfDayString(time);
	}
}
