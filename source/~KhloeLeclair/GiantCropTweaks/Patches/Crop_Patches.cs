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
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using StardewModdingAPI;

using StardewValley;
using StardewValley.GameData.GiantCrops;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace Leclair.Stardew.GiantCropTweaks.Patches; 
public static class Crop_Patches {

	private static IMonitor? Monitor;

	internal static AllowedLocations AllowedLocations = AllowedLocations.BaseGame;

	public static void Patch(ModEntry mod) {
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(Crop), nameof(Crop.newDay)),
				transpiler: new HarmonyMethod(typeof(Crop_Patches), nameof(Crop_newDay__Transpiler))
			);

		} catch (Exception ex) {
			mod.Log($"An error occurred while registering a harmony patch for Crop.", LogLevel.Warn, ex);
		}

	}


	public static bool ShouldLocationAllowGiantCrops(GameLocation location) {
		return AllowedLocations switch {
			AllowedLocations.BaseGame => location is Farm,
			AllowedLocations.BaseAndIsland => location is Farm || location is IslandWest,
			_ => true
		};
	}

	public static bool TryGetGiantCrops(Crop crop, out IReadOnlyList<KeyValuePair<string, GiantCropData>> giantCrops) {
		if (!crop.TryGetGiantCrops(out giantCrops))
			return false;

		// If this doesn't regrow, this logic doesn't apply.
		if (!crop.RegrowsAfterHarvest())
			return true;

		// If fullyGrown is false (we aren't done growing), then how did we get here?
		if (!crop.fullyGrown.Value && crop.currentPhase.Value != crop.phaseDays.Count - 1)
			return false;

		// If dayOfCurrentPhase is less than or equal to zero, we have fully grown
		// so there's no need to do more filtering.
		if (crop.dayOfCurrentPhase.Value <= 0)
			return true;

		// Alright, we might need to filter. Let's see what crops have restrictions.

		ModEntry.Instance.LoadCropData();
		var forbidden = ModEntry.Instance.CropsWithRegrowthRestriction;
		// None? No filtering!
		if (forbidden.Count == 0)
			return true;

		// If we got here, we have *some* filtering to do. Only return entries
		// that are not in the forbidden list.
		giantCrops = giantCrops
			.Where(pair => ! forbidden.Contains(pair.Key))
			.ToList();

		return giantCrops.Count > 0;
	}


	public static IEnumerable<CodeInstruction> Crop_newDay__Transpiler(IEnumerable<CodeInstruction> instructions) {
		var FarmType = typeof(Farm);
		var Crop_TryGetGiantCrops = AccessTools.Method(typeof(Crop), nameof(Crop.TryGetGiantCrops));

		var Wrapped_ShouldLocationAllowGiantCrops = AccessTools.Method(typeof(Crop_Patches), nameof(ShouldLocationAllowGiantCrops));
		var Wrapped_TryGetGiantCrops = AccessTools.Method(typeof(Crop_Patches), nameof(TryGetGiantCrops));

		foreach (var instr in instructions) {
			if (instr.opcode == OpCodes.Isinst && instr.operand is Type type && type == FarmType)
				yield return new CodeInstruction(instr) {
					opcode = OpCodes.Call,
					operand = Wrapped_ShouldLocationAllowGiantCrops
				};

			else if (instr.opcode == OpCodes.Call && instr.operand is MethodInfo minfo && minfo == Crop_TryGetGiantCrops)
				yield return new CodeInstruction(instr) {
					opcode = OpCodes.Call,
					operand = Wrapped_TryGetGiantCrops
				};

			else
				yield return instr;
		}
	}

}
