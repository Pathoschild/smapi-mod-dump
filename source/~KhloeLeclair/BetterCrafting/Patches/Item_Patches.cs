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
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Objects;

namespace Leclair.Stardew.BetterCrafting.Patches;

public static class Item_Patches {

	private static IMonitor? Monitor;

	internal static bool OverrideStackSize = false;

	public static void Patch(ModEntry mod) {
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(Item), nameof(Item.canStackWith)),
				transpiler: new HarmonyMethod(typeof(Item_Patches), nameof(canStackWith_Transpiler))
			);

		} catch (Exception ex) {
			mod.Log($"An error occurred while registering a harmony patch for items.", LogLevel.Warn, ex);
		}
	}

	private static IEnumerable<CodeInstruction> canStackWith_Transpiler(IEnumerable<CodeInstruction> instructions) {

		var method = AccessTools.Method(typeof(ISalable), nameof(ISalable.maximumStackSize));
		var method_two = AccessTools.Method(typeof(Item), nameof(Item.maximumStackSize));

		var ours = AccessTools.Method(typeof(Item_Patches), nameof(GetMaximumStackSize));

		foreach(var instr in instructions) {
			if (instr.opcode == OpCodes.Callvirt && instr.operand is MethodInfo minfo && (minfo == method || minfo == method_two))
				yield return new CodeInstruction(instr) {
					opcode = OpCodes.Call,
					operand = ours
				};

			else
				yield return instr;
		}

	}

	public static int GetMaximumStackSize(ISalable salable) {
		return OverrideStackSize ? 2 : salable.maximumStackSize();
	}

}
