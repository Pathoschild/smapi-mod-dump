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

using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;

namespace Leclair.Stardew.ThemeManager.Patches;

internal static class Billboard_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(Billboard), nameof(Billboard.draw), new Type[] {
					typeof(SpriteBatch)
				}),
				transpiler: new HarmonyMethod(typeof(Billboard_Patches), nameof(Draw_Transpiler))
			);

		} catch (Exception ex) {
			mod.Log("Unable to apply Billboard patches due to error.", LogLevel.Error, ex);
		}
	}

	internal static Color GetTextColor() {
		return Mod!.BaseTheme?.BillboardTextColor ?? Game1.textColor;
	}

	internal static Color GetDimColor() {
		return Mod!.BaseTheme?.CalendarDimColor ?? Color.Gray;
	}

	internal static Color GetTodayColor() {
		return Mod!.BaseTheme?.CalendarTodayColor ?? Color.Blue;
	}

	internal static Color GetHoverColor() {
		return Mod!.BaseTheme?.BillboardHoverColor ??
			Mod!.BaseTheme?.ButtonHoverColor ??
			Color.LightPink;
	}

	static IEnumerable<CodeInstruction> Draw_Transpiler(IEnumerable<CodeInstruction> instructions) {
		return PatchUtils.ReplaceColors(
			instructions,
			typeof(Billboard_Patches),
			new Dictionary<string, string> {
				{ nameof(Color.Gray), nameof(GetDimColor) },
				{ nameof(Color.Blue), nameof(GetTodayColor) },
				{ nameof(Color.LightPink), nameof(GetHoverColor) },
			},
			fieldReplacements: new Dictionary<string, string> {
				{ nameof(Game1.textColor), nameof(GetTextColor) }
			}.HydrateFieldKeys(typeof(Game1)).HydrateMethodValues(typeof(Billboard_Patches))
		);
	}

}
