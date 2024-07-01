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
using mouahrarasModuleCollection.ArcadeGames.KonamiCode.Utilities;

namespace mouahrarasModuleCollection.ArcadeGames.KonamiCode.Patches
{
	internal class MultiplayerPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(Multiplayer), nameof(Multiplayer.globalChatInfoMessage), new Type[] { typeof(string), typeof(string[]) }),
				prefix: new HarmonyMethod(typeof(MultiplayerPatch), nameof(GlobalChatInfoMessagePrefix))
			);
		}

		private static bool GlobalChatInfoMessagePrefix(string messageKey)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlayKonamiCode || !KonamiCodeUtility.InfiniteLivesMode)
				return true;

			if (messageKey.Equals("PrairieKing") || messageKey.Equals("JunimoKart"))
				return false;
			return true;
		}
	}
}
