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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using HarmonyLib;

using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;

namespace Leclair.Stardew.ThemeManager.Patches;

internal static class OptionsDropDown_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(OptionsDropDown), nameof(OptionsDropDown.draw), new Type[] {
					typeof(SpriteBatch), typeof(int), typeof(int), typeof(IClickableMenu)
				}),
				transpiler: new HarmonyMethod(typeof(OptionsDropDown_Patches), nameof(Draw_Transpiler))
			);

		} catch (Exception ex) {
			mod.Log("Unable to apply OptionsDropDown patches due to error.", LogLevel.Error, ex);
		}
	}

	public static Color GetHoverColor() {
		return Mod?.BaseTheme?.DropDownHoverColor ?? Mod?.BaseTheme?.HoverColor ?? Color.Wheat;
	}

	public static Color GetTextColor() {
		return Mod?.BaseTheme?.DropDownTextColor ?? Game1.textColor;
	}

	static IEnumerable<CodeInstruction> Draw_Transpiler(IEnumerable<CodeInstruction> instructions) {
		return PatchUtils.ReplaceColors(
			instructions: instructions,
			type: typeof(OptionsDropDown_Patches),
			replacements: new Dictionary<string, string> {
				{ nameof(Color.Wheat), nameof(GetHoverColor) },
			},
			fieldReplacements: new Dictionary<string, string> {
				{ nameof(Game1.textColor), nameof(GetTextColor) },
			}.HydrateFieldKeys(typeof(Game1)).HydrateMethodValues(typeof(OptionsDropDown_Patches))
		);
	}

}
