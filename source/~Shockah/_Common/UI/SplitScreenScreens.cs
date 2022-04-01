/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;

namespace Shockah.CommonModCode.UI
{
	public enum SplitScreenScreens { First, Last, All }

	public static class SplitScreenScreensExtensions
	{
		public static bool MatchesCurrentScreen(this SplitScreenScreens self)
		{
			return self switch
			{
				SplitScreenScreens.First => Context.ScreenId == 0,
				SplitScreenScreens.Last => GameRunner.instance.gameInstances.Count > 0 && Context.ScreenId == GameRunner.instance.gameInstances[^1].instanceId,
				SplitScreenScreens.All => true,
				_ => throw new ArgumentException($"{nameof(SplitScreenScreens)} has an invalid value."),
			};
		}
	}
}