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

internal static class LevelUpMenu_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(LevelUpMenu), nameof(LevelUpMenu.update), new Type[] {
					typeof(GameTime)
				}),
				transpiler: new HarmonyMethod(typeof(LevelUpMenu_Patches), nameof(Update_Transpiler))
			);

		} catch (Exception ex) {
			mod.Log("Unable to apply LevelUpMenu patches due to error.", LogLevel.Error, ex);
		}
	}

	public static Color GetHoverColor() {
		return Mod?.BaseTheme?.LevelUpHoverTextColor ?? Color.Green;
	}

	static IEnumerable<CodeInstruction> Update_Transpiler(IEnumerable<CodeInstruction> instructions) {
		return PatchUtils.ReplaceColors(instructions, typeof(LevelUpMenu_Patches), new Dictionary<string, string> {
			{ nameof(Color.Green), nameof(GetHoverColor) },
		});
	}

}
