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

internal static class CarpenterMenu_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.draw), new Type[] {
					typeof(SpriteBatch)
				}),
				transpiler: new HarmonyMethod(typeof(CarpenterMenu_Patches), nameof(Draw_Transpiler))
			);

		} catch (Exception ex) {
			mod.Log("Unable to apply CarpenterMenu patches due to error.", LogLevel.Error, ex);
		}
	}

	public static Color GetErrorTextColor() {
		return Mod?.BaseTheme?.ErrorTextColor ?? Color.Red;
	}

	public static Color GetMagicBackgroundColor() {
		return Mod?.BaseTheme?.CarpenterMagicBackgroundColor ?? Color.RoyalBlue;
	}

	public static Color GetMagicTextColor() {
		return Mod?.BaseTheme?.CarpenterMagicTextColor ?? Color.PaleGoldenrod;
	}

	static IEnumerable<CodeInstruction> Draw_Transpiler(IEnumerable<CodeInstruction> instructions) {
		return PatchUtils.ReplaceColors(
			instructions: instructions,
			type: typeof(CarpenterMenu_Patches),
			replacements: new Dictionary<string, string> {
				{ nameof(Color.Red), nameof(GetErrorTextColor) },
				{ nameof(Color.PaleGoldenrod), nameof(GetMagicTextColor) },
				{ nameof(Color.RoyalBlue), nameof(GetMagicBackgroundColor) },
			}
		);
	}

}
