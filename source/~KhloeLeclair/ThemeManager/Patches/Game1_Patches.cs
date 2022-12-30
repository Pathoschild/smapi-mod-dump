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

using HarmonyLib;

using Leclair.Stardew.ThemeManager.Models;

using StardewModdingAPI;

using StardewValley;

namespace Leclair.Stardew.ThemeManager.Patches;

internal static class Game1_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(Game1), "LoadContent"),
				postfix: new HarmonyMethod(typeof(Game1_Patches), nameof(LoadContent_Postfix))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(Game1), nameof(Game1.TranslateFields)),
				postfix: new HarmonyMethod(typeof(Game1_Patches), nameof(TranslateFields_Postfix))
			);

		} catch (Exception ex) {
			mod.Log("Unable to apply Game1 patches due to error.", LogLevel.Error, ex);
		}
	}

	static void LoadContent_Postfix() {
		if (Mod?.SpriteFontManager is not null) {
			Mod.SpriteFontManager.UpdateDefaultFonts();
			Mod.SpriteFontManager.AssignFonts(Mod.GameTheme);
		}
		if (Mod?.SpriteTextManager is not null) {
			Mod.SpriteTextManager.UpdateDefaultTextures();
			Mod.SpriteTextManager.AssignFonts(Mod.GameTheme);
		}
	}

	static void TranslateFields_Postfix() {
		if (Mod?.SpriteFontManager is not null) {
			Mod.SpriteFontManager.UpdateDefaultFonts();
			Mod.SpriteFontManager.AssignFonts(Mod.GameTheme);
		}
	}

}
