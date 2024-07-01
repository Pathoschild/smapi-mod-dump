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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley.Minigames;
using mouahrarasModuleCollection.ArcadeGames.KonamiCode.Utilities;

namespace mouahrarasModuleCollection.ArcadeGames.KonamiCode.Patches
{
	internal class AbigailGamePatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.reset), new Type[] { typeof(bool) }),
				postfix: new HarmonyMethod(typeof(AbigailGamePatch), nameof(ResetPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.unload)),
				postfix: new HarmonyMethod(typeof(AbigailGamePatch), nameof(UnloadPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.SaveGame)),
				prefix: new HarmonyMethod(typeof(AbigailGamePatch), nameof(SaveGamePrefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.receiveKeyPress), new Type[] { typeof(Keys) }),
				postfix: new HarmonyMethod(typeof(AbigailGamePatch), nameof(ReceiveKeyPressPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.getPowerUp), new Type[] { typeof(AbigailGame.CowboyPowerup) }),
				postfix: new HarmonyMethod(typeof(AbigailGamePatch), nameof(GetPowerUpPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.usePowerup), new Type[] { typeof(int) }),
				postfix: new HarmonyMethod(typeof(AbigailGamePatch), nameof(UsePowerupPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.tick), new Type[] { typeof(GameTime) }),
				postfix: new HarmonyMethod(typeof(AbigailGamePatch), nameof(TickPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.playerDie)),
				prefix: new HarmonyMethod(typeof(AbigailGamePatch), nameof(PlayerDiePrefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.playerDie)),
				postfix: new HarmonyMethod(typeof(AbigailGamePatch), nameof(PlayerDiePostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.forceQuit)),
				postfix: new HarmonyMethod(typeof(AbigailGamePatch), nameof(ForceQuitPostfix))
			);
		}

		private static void ResetPostfix(AbigailGame __instance)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlayKonamiCode || !KonamiCodeUtility.InfiniteLivesMode)
				return;
			__instance.lives = 99;
		}

		private static void UnloadPostfix(AbigailGame __instance)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlayKonamiCode || !KonamiCodeUtility.InfiniteLivesMode)
				return;
			__instance.lives = 99;
		}

		private static bool SaveGamePrefix()
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlayKonamiCode || !KonamiCodeUtility.InfiniteLivesMode)
				return true;
			return false;
		}

		private static void ReceiveKeyPressPostfix(AbigailGame __instance, Keys k)
		{
			KonamiCodeUtility.ReceiveKeyPressPostfix(k);
			if (KonamiCodeUtility.InfiniteLivesMode)
				__instance.lives = 99;
		}

		private static void GetPowerUpPostfix(AbigailGame __instance)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlayKonamiCode || !KonamiCodeUtility.InfiniteLivesMode)
				return;
			__instance.lives = 99;
		}

		private static void UsePowerupPostfix(AbigailGame __instance)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlayKonamiCode || !KonamiCodeUtility.InfiniteLivesMode)
				return;
			__instance.lives = 99;
		}

		private static void TickPostfix(AbigailGame __instance, bool __result)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlayKonamiCode || !KonamiCodeUtility.InfiniteLivesMode)
				return;

			if (__result == false)
				__instance.lives = 99;
			else
				KonamiCodeUtility.Reset();
		}

		private static bool PlayerDiePrefix(AbigailGame __instance)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlayKonamiCode || !KonamiCodeUtility.InfiniteLivesMode)
				return true;
			__instance.lives = 99;
			return true;
		}

		private static void PlayerDiePostfix(AbigailGame __instance)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlayKonamiCode || !KonamiCodeUtility.InfiniteLivesMode)
				return;
			__instance.lives = 99;
		}

		private static void ForceQuitPostfix(bool __result)
		{
			if (!ModEntry.Config.ArcadeGamesPayToPlayKonamiCode || !KonamiCodeUtility.InfiniteLivesMode)
				return;

			if (__result == true)
				KonamiCodeUtility.Reset();
		}
	}
}
