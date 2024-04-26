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
using StardewModdingAPI;
using StardewValley.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Leclair.Stardew.MoreNightlyEvents.Patches;

public static class TreeCrop_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	internal static bool IsSpawning = false;

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		try {

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(FruitTree), nameof(FruitTree.IgnoresSeasonsHere)),
				postfix: new HarmonyMethod(typeof(TreeCrop_Patches), nameof(FruitTree_IgnoresSeasonsHere__Postfix))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(Crop), nameof(Crop.IsInSeason), [typeof(GameLocation)]),
				postfix: new HarmonyMethod(typeof(TreeCrop_Patches), nameof(Crop_IsInSeason__Postfix))
			);

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(Crop), nameof(Crop.IsInSeason), [typeof(GameLocation), typeof(string)]),
				postfix: new HarmonyMethod(typeof(TreeCrop_Patches), nameof(Crop_IsInSeason__Postfix))
			);

		} catch (Exception ex) {
			mod.Log($"Unable to apply Utility patches due to an error. This mod will not function.", LogLevel.Error, ex);
		}
	}

	internal static void Crop_IsInSeason__Postfix(Crop __instance, ref bool __result) {
		if (IsSpawning)
			__result = true;
		else if (__instance.modData.TryGetValue(ModEntry.IGNORE_SEASON_DATA, out string? value) && value != null && value.Equals("true", StringComparison.OrdinalIgnoreCase))
			__result = true;
	}

	internal static void FruitTree_IgnoresSeasonsHere__Postfix(FruitTree __instance, ref bool __result) {
		if (IsSpawning)
			__result = true;
		else if (__instance.modData.TryGetValue(ModEntry.IGNORE_SEASON_DATA, out string? value) && value != null && value.Equals("true", StringComparison.OrdinalIgnoreCase))
			__result = true;
	}

}
