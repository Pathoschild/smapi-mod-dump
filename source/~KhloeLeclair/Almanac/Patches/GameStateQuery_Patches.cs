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
using System.Reflection.Emit;
using System.Reflection;

using HarmonyLib;

using StardewModdingAPI;

using StardewValley;

namespace Leclair.Stardew.Almanac.Patches;

internal static class GameStateQuery_Patches {

	private static IMonitor Monitor;

	internal static void Patch(ModEntry mod) {
		Monitor = mod.Monitor;

		try {
			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(GameStateQuery), nameof(GameStateQuery.DefaultResolvers.IS_FESTIVAL_DAY)),
				transpiler: new HarmonyMethod(typeof(GameStateQuery_Patches), nameof(query_IS_FESTIVAL_DAY_Transpiler))
			);
		} catch (Exception ex) {
			mod.Log("An error occurred while registering a harmony patch for GameStateQuery.", LogLevel.Warn, ex);
		}
	}

	public static WorldDate GetWorldDate() {
		return new WorldDate(Game1.year, Game1.currentSeason, Game1.dayOfMonth);
	}

	public static IEnumerable<CodeInstruction> query_IS_FESTIVAL_DAY_Transpiler(IEnumerable<CodeInstruction> instructions) {
		foreach (var instr in instructions) {
			if (instr.opcode == OpCodes.Call && instr.operand is MethodInfo method && method == AccessTools.PropertyGetter(typeof(Game1), nameof(Game1.Date))) {
				yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GameStateQuery_Patches), nameof(GetWorldDate)));
			} else
				yield return instr;
		}
	}

}
