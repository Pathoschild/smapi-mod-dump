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

using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewModdingAPI;

namespace Leclair.Stardew.ThemeManager.Patches;

internal static class MoneyDial_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;
	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(MoneyDial), nameof(MoneyDial.draw)),
				transpiler: new HarmonyMethod(typeof(MoneyDial_Patches), nameof(Draw_Transpiler))
			);

		} catch (Exception ex) {
			mod.Log("Unable to apply MoneyDial patches due to error.", LogLevel.Error, ex);
		}
	}

	public static Color GetSparkleColor() {
		return Mod?.BaseTheme?.MoneySparkleColor ?? Color.Gold;
	}

	public static Color GetMoneyColor() {
		return Mod?.BaseTheme?.MoneyTextColor ?? Color.Maroon;
	}

	static IEnumerable<CodeInstruction> Draw_Transpiler(IEnumerable<CodeInstruction> instructions) {
		return PatchUtils.ReplaceColors(
			instructions: instructions,
			type: typeof(MoneyDial_Patches),
			replacements: new Dictionary<string, string> {
				{ nameof(Color.Gold), nameof(GetSparkleColor) },
				{ nameof(Color.Maroon), nameof(GetMoneyColor) }
			}
		);
	}

}
