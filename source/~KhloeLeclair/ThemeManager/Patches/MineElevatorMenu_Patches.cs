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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using HarmonyLib;

using StardewValley.Menus;
using StardewModdingAPI;

namespace Leclair.Stardew.ThemeManager.Patches;

internal static class MineElevatorMenu_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(MineElevatorMenu), nameof(MineElevatorMenu.draw), new Type[] {
					typeof(SpriteBatch)
				}),
				transpiler: new HarmonyMethod(typeof(MineElevatorMenu_Patches), nameof(Draw_Transpiler))
			);

		} catch (Exception ex) {
			mod.Log("Unable to apply MineElevatorMenu patches due to error.", LogLevel.Error, ex);
		}
	}

	public static Color GetCurrentFloorColor() {
		return Mod?.BaseTheme?.ElevatorCurrentFloorTextColor ?? Color.Gray;
	}

	public static Color GetFloorColor() {
		return Mod?.BaseTheme?.ElevatorFloorTextColor ?? Color.Gold;
	}

	static IEnumerable<CodeInstruction> Draw_Transpiler(IEnumerable<CodeInstruction> instructions) {
		return PatchUtils.ReplaceColors(instructions, typeof(MineElevatorMenu_Patches), new Dictionary<string, string> {
			{ nameof(Color.Gray), nameof(GetCurrentFloorColor) },
			{ nameof(Color.Gold), nameof(GetFloorColor) },
		});
	}

}
