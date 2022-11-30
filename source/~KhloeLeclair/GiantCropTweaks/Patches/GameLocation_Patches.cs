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

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.TransferDataFromSavedLocation)),
				postfix: new HarmonyMethod(typeof(GameLocation_Patches), nameof(TransferData_Postfix))
			);

		} catch(Exception ex) {
			mod.Log($"An error occurred while registering a harmony patch for GameLocation.", LogLevel.Error, ex);
		}
	}

	public static void TransferData_Postfix(GameLocation __instance, GameLocation l) {
		// game handles these two.
		if (__instance is IslandWest || __instance.Name.Equals("Farm", StringComparison.OrdinalIgnoreCase))
			return;

		// If we already seem to have enough clumps, abort.
		if (__instance.resourceClumps.Count >= l.resourceClumps.Count)
			return;

		// We need to avoid accidentally adding duplicates.
		// Keep track of occupied tiles here.
		HashSet<Vector2> prev = new(l.resourceClumps.Count);

		foreach (var clump in __instance.resourceClumps)
			prev.Add(clump.tile.Value);

		// restore previous giant crops
		int count = 0;
		foreach(var clump in l.resourceClumps) {
			if (clump is GiantCrop && prev.Add(clump.tile.Value)) {
				count++;
				__instance.resourceClumps.Add(clump);
			}
		}

		if (count > 0)
			ModEntry.Instance.Log($"Restored {count} giant crops at {__instance.NameOrUniqueName}.", LogLevel.Debug);
	}

	public static bool removeEverythingFromThisTile_Prefix(GameLocation __instance, int x, int y) {
		try {
			// We need to unprotect HoeDirt under giant crops.
			foreach (var clump in __instance.resourceClumps) {
				if (clump is GiantCrop crop && clump.tile.X == x && clump.tile.Y == y)
					ModEntry.Instance.UnprotectGiantCropDirt(__instance, crop);
			}

		} catch(Exception ex) {
			Monitor?.LogOnce($"An error occurred while attempting to clear a region from a GameLocation:\n{ex}", LogLevel.Warn);
		}

		return true;
	}


}
