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

internal static class SkillsPage_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(SkillsPage), nameof(SkillsPage.draw), new Type[] {
					typeof(SpriteBatch)
				}),
				transpiler: new HarmonyMethod(typeof(SkillsPage_Patches), nameof(Draw_Transpiler))
			);

		} catch (Exception ex) {
			mod.Log("Unable to apply ShopMenu patches due to error.", LogLevel.Error, ex);
		}
	}

	public static Color GetTextColor() {
		return Mod!.BaseTheme?.SkillsPageTextColor ??
			Game1.textColor;
	}

	public static Color GetModifiedNumberColor() {
		return Mod!.BaseTheme?.SkillsPageModifiedNumberColor ??
			Color.LightGreen;
	}

	public static Color GetNumberColor() {
		return Mod!.BaseTheme?.SkillsPageNumberColor ??
			Color.SandyBrown;
	}

	static IEnumerable<CodeInstruction> Draw_Transpiler(IEnumerable<CodeInstruction> instructions) {

		return PatchUtils.ReplaceColors(
			instructions: instructions,
			type: typeof(SkillsPage_Patches),
			replacements: new Dictionary<string, string> {
				{ nameof(Color.LightGreen), nameof(GetModifiedNumberColor) },
				{ nameof(Color.SandyBrown), nameof(GetNumberColor) }
			},
			fieldReplacements: new Dictionary<string, string> {
				{ nameof(Game1.textColor), nameof(GetTextColor) }
			}.HydrateFieldKeys(typeof(Game1)).HydrateMethodValues(typeof(SkillsPage_Patches))
		);

	}

}
