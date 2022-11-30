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
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using StardewModdingAPI;

using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Locations;

namespace Leclair.Stardew.GiantCropTweaks.Patches;

public static class Crop_Patches {

	private static IMonitor? Monitor;

	public static void Patch(ModEntry mod) {
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(Crop), nameof(Crop.newDay)),
				postfix: new HarmonyMethod(typeof(Crop_Patches), nameof(newDay_Postfix)),
				transpiler: new HarmonyMethod(typeof(Crop_Patches), nameof(newDay_Transpiler))
			);

		} catch (Exception ex) {
			mod.Log($"An error occurred while registering a harmony patch for Crop.", LogLevel.Warn, ex);
		}

	}

	public static GameLocation? IsGiantCropLocation(GameLocation location) {
		return ModEntry.Instance.IsGiantCropLocation(location);
	}

	public static void newDay_Postfix(Crop __instance, int state, int fertilizer, int xTile, int yTile, GameLocation environment) {
		try {
			if (state == 1 || __instance.indexOfHarvest.Value == 771)
				ModEntry.Instance.TryGrowingGiantCrop(environment, __instance, state, fertilizer, xTile, yTile);

		} catch(Exception ex) {
			Monitor?.Log($"An error occurred while attempting to grow a giant crop:\n{ex}", LogLevel.Warn);
		}
	}

	public static IEnumerable<CodeInstruction> newDay_Transpiler(IEnumerable<CodeInstruction> instructions) {
		// For now, don't bother changing the giant crop chance. We can do our own
		// giant crops to change rates later.
		/*instructions = ReplaceCalls(
			instructions: instructions,
			callReplacements: new Dictionary<MethodInfo, MethodInfo> {
				{ ModEntry.OneTimeRandom_GetDouble, AccessTools.Method(typeof(Crop_Patches), nameof(CheckShouldGiantGrow)) }
			}
		);*/

		// Remove the "isinstance Farm" check, replacing it with a check that uses the
		// map property introduced in 1.6.
		foreach(var instr in instructions) {
			// This should realistically match two instructions, but overriding both is fine.
			if (instr.opcode == OpCodes.Isinst && instr.operand is Type t && t == typeof(Farm)) {
				yield return new CodeInstruction(instr) {
					opcode = OpCodes.Call,
					operand = AccessTools.Method(typeof(Crop_Patches), nameof(IsGiantCropLocation))
				};
				continue;
			}

			yield return instr;
		}
	}
}
