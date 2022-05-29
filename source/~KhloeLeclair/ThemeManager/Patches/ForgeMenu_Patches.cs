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

internal static class ForgeMenu_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(ForgeMenu), "draw", new Type[] {
					typeof(SpriteBatch)
				}),
				transpiler: new HarmonyMethod(typeof(ForgeMenu_Patches), nameof(Draw_Transpiler))
			);

		} catch (Exception ex) {
			mod.Log("Unable to apply ForgeMenu patches due to error.", LogLevel.Error, ex);
		}
	}

	public static readonly Color FORGE_BACKGROUND_COLOR = new(116, 11, 3);

	public static Color GetBackgroundColor() {
		return Mod?.BaseTheme?.ForgeMenuBackgroundColor ?? FORGE_BACKGROUND_COLOR;
	}

	static IEnumerable<CodeInstruction> Draw_Transpiler(IEnumerable<CodeInstruction> instructions) {

		return PatchUtils.ReplaceColors(
			instructions: instructions,
			replacements: new Dictionary<Color, string> {
				{ new Color(116, 11, 3), nameof(GetBackgroundColor) },
			}.HydrateMethodValues(typeof(ForgeMenu_Patches))
		);

	}

}
