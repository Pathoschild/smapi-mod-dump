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

internal static class LoadGameMenu_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(LoadGameMenu.SaveFileSlot), nameof(LoadGameMenu.SaveFileSlot.drawVersionMismatchSlot), new Type[] {
					typeof(SpriteBatch), typeof(int)
				}),
				transpiler: new HarmonyMethod(typeof(LoadGameMenu_Patches), nameof(DrawVersionMismatch_Transpiler))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(LoadGameMenu), "drawSlotBackground"),
				transpiler: new HarmonyMethod(typeof(LoadGameMenu_Patches), nameof(DrawSlotBackground_Transpiler))
			);

		} catch (Exception ex) {
			mod.Log("Unable to apply LoadGameMenu patches due to error.", LogLevel.Error, ex);
		}
	}

	public static Color GetErrorColor() {
		return Mod?.BaseTheme?.LoadGameErrorTextColor ?? Mod?.BaseTheme?.ErrorTextColor ?? Color.Red;
	}

	public static Color GetHoverColor() {
		return Mod?.BaseTheme?.LoadGameHoverColor ?? Mod?.BaseTheme?.HoverColor ?? Color.Wheat;
	}


	static IEnumerable<CodeInstruction> DrawVersionMismatch_Transpiler(IEnumerable<CodeInstruction> instructions) {
		return PatchUtils.ReplaceColors(
			instructions: instructions,
			type: typeof(LoadGameMenu_Patches),
			replacements: new Dictionary<string, string> {
				{ nameof(Color.Red), nameof(GetErrorColor) }
			}
		);
	}


	static IEnumerable<CodeInstruction> DrawSlotBackground_Transpiler(IEnumerable<CodeInstruction> instructions) {
		return PatchUtils.ReplaceColors(
			instructions: instructions,
			type: typeof(LoadGameMenu_Patches),
			replacements: new Dictionary<string, string> {
				{ nameof(Color.Wheat), nameof(GetHoverColor) }
			}
		);
	}

}
