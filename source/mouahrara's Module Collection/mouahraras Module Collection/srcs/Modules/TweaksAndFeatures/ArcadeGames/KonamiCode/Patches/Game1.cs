/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using System;
using HarmonyLib;
using StardewValley;
using mouahrarasModuleCollection.TweaksAndFeatures.ArcadeGames.KonamiCode.Utilities;

namespace mouahrarasModuleCollection.TweaksAndFeatures.ArcadeGames.KonamiCode.Patches
{
	internal class Game1Patch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(Game1), nameof(Game1.addMailForTomorrow), new Type[] { typeof(string), typeof(bool), typeof(bool) }),
				prefix: new HarmonyMethod(typeof(Game1Patch), nameof(AddMailForTomorrowPrefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(Game1), nameof(Game1.getSteamAchievement), new Type[] { typeof(string) }),
				prefix: new HarmonyMethod(typeof(Game1Patch), nameof(GetSteamAchievementPrefix))
			);
		}

		private static bool AddMailForTomorrowPrefix(string mailName)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlayKonamiCode || !KonamiCodeUtility.InfiniteLivesMode)
				return true;

			if (mailName.Equals("Beat_PK") || mailName.Equals("JunimoKart"))
				return false;
			return true;
		}

		private static bool GetSteamAchievementPrefix(string which)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlayKonamiCode || !KonamiCodeUtility.InfiniteLivesMode)
				return true;

			if (which.Equals("Achievement_PrairieKing") || which.Equals("Achievement_FectorsChallenge"))
				return false;
			return true;
		}
	}
}
