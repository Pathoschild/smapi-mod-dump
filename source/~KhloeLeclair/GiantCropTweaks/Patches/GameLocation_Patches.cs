/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

using Microsoft.Xna.Framework;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace Leclair.Stardew.GiantCropTweaks.Patches;

public static class GameLocation_Patches {

	private static IMonitor? Monitor;

	public static void Patch(ModEntry mod) {
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.removeEverythingFromThisTile)),
				prefix: new HarmonyMethod(typeof(GameLocation_Patches), nameof(removeEverythingFromThisTile_Prefix))
			);

		} catch(Exception ex) {
			mod.Log($"An error occurred while registering a harmony patch for GameLocation.", LogLevel.Error, ex);
		}
	}

	public static bool removeEverythingFromThisTile_Prefix(GameLocation __instance, int x, int y) {
		try {
			// We need to un-protect HoeDirt under giant crops.
			foreach (var clump in __instance.resourceClumps) {
				if (clump is GiantCrop crop && clump.Tile.X == x && clump.Tile.Y == y)
					ModEntry.Instance.UnprotectGiantCropDirt(__instance, crop);
			}

		} catch(Exception ex) {
			Monitor?.LogOnce($"An error occurred while attempting to clear a region from a GameLocation:\n{ex}", LogLevel.Warn);
		}

		return true;
	}


}
