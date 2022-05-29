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
using StardewModdingAPI;

namespace Leclair.Stardew.Common;

public static class TimeHelper {

	public static string FormatTime(int time, Translation format) {
		return FormatTime(time, format.HasValue() ? format.ToString() : null);
	}

	public static string FormatTime(int time, string? format) {
		// Limit it to one day.
		time %= 2400;

		if (string.IsNullOrEmpty(format))
			return Game1.getTimeOfDayString(time);

		return LocalizedContentManager.FormatTimeString(time, format).ToString();
	}
}
