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

using Microsoft.Xna.Framework;

using StardewValley.BellsAndWhistles;

using StardewModdingAPI;

using Leclair.Stardew.Common;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Leclair.Stardew.ThemeManager.Managers;

namespace Leclair.Stardew.ThemeManager.Patches;

internal static class SpriteText_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(SpriteText), "OnLanguageChange"),
				postfix: new HarmonyMethod(typeof(SpriteText_Patches), nameof(OnLanguageChange_Postfix))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(SpriteText), nameof(SpriteText.drawString)),
				prefix: new HarmonyMethod(typeof(SpriteText_Patches), nameof(drawString_Prefix))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(SpriteText), nameof(SpriteText.getColorFromIndex)),
				prefix: new HarmonyMethod(typeof(SpriteText_Patches), nameof(getColorFromIndex__Prefix))
			);
		} catch (Exception ex) {
			mod.Log("Unable to apply SpriteText patches due to error.", LogLevel.Error, ex);
		}
	}

	static void OnLanguageChange_Postfix() {
		if (SpriteTextManager.Instance is not null) {
			SpriteTextManager.Instance.UpdateDefaultFont();
			SpriteTextManager.Instance.AssignFonts(Mod?.GameTheme);
		}
	}

	static bool drawString_Prefix(bool junimoText, ref int color) {
		try {
			if (! junimoText && color == -1) {
				var colors = Mod?.GameTheme?.SpriteTextColors;
				if (colors is not null && colors.ContainsKey(-1))
					color = int.MinValue;
			}

		} catch(Exception ex) {
			Monitor?.LogOnce($"An error occurred in {nameof(drawString_Prefix)}. Details:\n{ex}", LogLevel.Warn);
		}

		return true;
	}

	static bool getColorFromIndex__Prefix(int index, ref Color __result) {
		try {
			if (index >= 100) {
				__result = CommonHelper.UnpackColor(index - 100);
				return false;
			}

			var colors = Mod?.GameTheme?.SpriteTextColors;

			if (index == int.MinValue) {
				if (colors is null || !colors.TryGetValue(-1, out __result)) {
					if (LocalizedContentManager.CurrentLanguageLatin)
						__result = Color.White;
					else
						__result = new Color(86, 22, 12);
				}

				return false;
			}

			if (colors is not null && colors.TryGetValue(index, out __result))
				return false;

		} catch(Exception ex) {
			Monitor?.LogOnce($"An error occurred in {nameof(getColorFromIndex__Prefix)}. Details:\n{ex}", LogLevel.Warn);
		}

		return true;
	}

}
