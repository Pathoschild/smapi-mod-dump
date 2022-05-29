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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using HarmonyLib;

using StardewValley.Menus;
using StardewModdingAPI;

namespace Leclair.Stardew.ThemeManager.Patches;

internal static class CoopMenu_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(CoopMenu), "drawTabs", new Type[] {
					typeof(SpriteBatch)
				}),
				transpiler: new HarmonyMethod(typeof(CoopMenu_Patches), nameof(DrawTabs_Transpiler))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(CoopMenu), "drawExtra", new Type[] {
					typeof(SpriteBatch)
				}),
				transpiler: new HarmonyMethod(typeof(CoopMenu_Patches), nameof(DrawExtra_Transpiler))
			);

		} catch (Exception ex) {
			mod.Log("Unable to apply CoopMenu patches due to error.", LogLevel.Error, ex);
		}
	}

	public static Color GetTabSelectedColorSmall() {
		return Mod?.BaseTheme?.CoopSmallTabSelectedColor ?? Color.Orange;
	}

	public static Color GetTabSelectedColor() {
		return Mod?.BaseTheme?.CoopTabSelectedColor ?? new Color(255, 255, 150);
	}

	public static Color GetTabHoverColor() {
		return Mod?.BaseTheme?.CoopTabHoverColor ?? Color.Yellow;
	}

	public static Color GetTabHoverShadowColor() {
		return Mod?.BaseTheme?.CoopTabHoverShadowColor ?? Color.DarkGoldenrod;
	}

	public static Color GetTabSelectedShadowColorSmall() {
		return Mod?.BaseTheme?.CoopSmallTabSelectedShadowColor ?? Color.DarkOrange;
	}

	public static Color GetTabSelectedShadowColor() {
		return Mod?.BaseTheme?.CoopTabSelectedShadowColor ?? new Color(221, 148, 84);
	}

	public static Color GetHoverColor() {
		return Mod?.BaseTheme?.CoopHoverColor ?? Mod?.BaseTheme?.HoverColor ?? Color.Wheat;
	}

	static IEnumerable<CodeInstruction> DrawTabs_Transpiler(IEnumerable<CodeInstruction> instructions) {

		instructions = PatchUtils.ReplaceColors(
			instructions: instructions,
			type: typeof(CoopMenu_Patches),
			replacements: new Dictionary<string, string> {
				{ nameof(Color.Orange), nameof(GetTabSelectedColorSmall) },
				{ nameof(Color.DarkOrange), nameof(GetTabSelectedShadowColorSmall) },
				{ nameof(Color.Yellow), nameof(GetTabHoverColor) },
				{ nameof(Color.DarkGoldenrod), nameof(GetTabHoverShadowColor) }
			}
		);

		instructions = PatchUtils.ReplaceColors(
			instructions: instructions,
			replacements: new Dictionary<Color, string> {
				{ new Color(255, 255, 150), nameof(GetTabSelectedColor) },
				{ new Color(221, 148, 84), nameof(GetTabSelectedShadowColor) },
			}.HydrateMethodValues(typeof(CoopMenu_Patches))
		);

		return instructions;
	}

	static IEnumerable<CodeInstruction> DrawExtra_Transpiler(IEnumerable<CodeInstruction> instructions) {
		return PatchUtils.ReplaceColors(
			instructions: instructions,
			type: typeof(CoopMenu_Patches),
			replacements: new Dictionary<string, string> {
				{ nameof(Color.Wheat), nameof(GetHoverColor) },
			}
		);
	}

}
