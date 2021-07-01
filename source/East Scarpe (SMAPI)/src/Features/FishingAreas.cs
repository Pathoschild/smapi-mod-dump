/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/eastscarpe
**
*************************************************/

using System;
using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace EastScarpe
{
	public static class FishingAreas
	{
		// private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ModData Data => ModEntry.Instance.data;
		private static HarmonyInstance Harmony => ModEntry.Instance.harmony;

		internal static void Patch ()
		{
			Harmony.Patch (
				original: AccessTools.Method (typeof (GameLocation),
					nameof (GameLocation.getFishingLocation)),
				postfix: new HarmonyMethod (typeof (FishingAreas),
					nameof (FishingAreas.GameLocation_getFishingLocation_Postfix))
			);
		}

		private static void GameLocation_getFishingLocation_Postfix (GameLocation __instance,
			Vector2 tile, ref int __result)
		{
			try
			{
				// Try to use the tile where the line has been cast, instead of
				// where the player is standing. Helps with tight areas.
				if (Game1.player.CurrentTool is FishingRod rod &&
						rod.bobber.Value != Vector2.Zero)
					tile = rod.bobber.Value / 64f;

				foreach (var fishingArea in Data.FishingAreas)
				{
					if (fishingArea.checkArea (__instance, tile) &&
						fishingArea.Conditions.check ())
					{
						__result = fishingArea.Index;
						return;
					}
				}
			}
			catch (Exception e)
			{
				Monitor.Log ($"Failed in {nameof (GameLocation_getFishingLocation_Postfix)}:\n{e}",
					LogLevel.Error);
				Monitor.Log (e.StackTrace, LogLevel.Trace);
			}
		}
	}
}
