/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using System;

namespace Shockah.FlexibleSprinklers
{
	internal static class BetterSplinklersPatches
	{
		private static readonly string BetterSprinklersSprinklerModQualifiedName = "BetterSprinklers.SprinklerMod, SMAPISprinklerMod";

		internal static void Apply(Harmony harmony)
		{
			try
			{
				harmony.Patch(
					original: AccessTools.DeclaredMethod(Type.GetType(BetterSprinklersSprinklerModQualifiedName), "RunSprinklers"),
					prefix: new HarmonyMethod(typeof(BetterSplinklersPatches), nameof(RunSprinklers_Prefix))
				);
			}
			catch (Exception e)
			{
				ModEntry.Instance.Monitor.Log($"Could not patch BetterSprinklers - they probably won't work.\nReason: {e}", LogLevel.Warn);
			}
		}

		internal static bool RunSprinklers_Prefix()
		{
			return false;
		}
	}
}