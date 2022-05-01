/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/eastscarp
**
*************************************************/

using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace EastScarp
{
	public static class WinterGrasses
	{
		// private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ModData Data => ModEntry.Instance.data;
		private static Harmony Harmony => ModEntry.Instance.harmony;

		internal static void Patch ()
		{
			Harmony.Patch (
				original: AccessTools.Method (typeof (Grass),
					nameof (Grass.seasonUpdate)),
				prefix: new HarmonyMethod (typeof (WinterGrasses),
					nameof (WinterGrasses.Grass_seasonUpdate_Prefix))
			);
		}

		private static bool Grass_seasonUpdate_Prefix (Grass __instance,
			ref bool __result)
		{
			try
			{
				// Only applicable to standard grass in winter locations.
				if (__instance.grassType.Value != 1 ||
						Game1.GetSeasonForLocation (__instance.currentLocation) != "winter")
					return true;

				// Only applicable in configured locations and areas.
				if (!Data.WinterGrasses.Exists ((grassArea) =>
						grassArea.checkArea (__instance.currentLocation, __instance.currentTileLocation)))
					return true;

				// Update the grass as if it weren't winter.
				__instance.loadSprite ();
				__result = false;
				return false;
			}
			catch (Exception e)
			{
				Monitor.Log ($"Failed in {nameof (Grass_seasonUpdate_Prefix)}:\n{e}",
					LogLevel.Error);
				Monitor.Log (e.StackTrace, LogLevel.Trace);
			}
			return true;
		}

		public static void Apply ()
		{
			// World must be ready.
			if (!Context.IsWorldReady)
				return;

			foreach (var grassArea in Data.WinterGrasses)
				ApplyGrass (grassArea);
		}

		private static bool ApplyGrass (WinterGrass grassArea)
		{
			// Must be in the right location.
			var location = Game1.player.currentLocation;
			if (location.Name != grassArea.Location)
				return false;

			// Must be winter in the location.
			if (Game1.GetSeasonForLocation (location) != "winter")
				return false;

			// Apply the appearance.
			Rectangle area = grassArea.adjustArea (location);
			Monitor.Log ($"Applying winter grass appearance in area {area} of location '{location.Name}'.",
				LogLevel.Trace);
			foreach (var tf in location.terrainFeatures.Values)
			{
				if (tf is Grass grass &&
						area.Contains (Utility.Vector2ToPoint (grass.currentTileLocation)))
					grass.grassSourceOffset.Value = 80;
			}
			return true;
		}
	}
}
