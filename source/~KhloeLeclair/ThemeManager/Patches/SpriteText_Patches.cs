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

namespace Leclair.Stardew.ThemeManager.Patches;

internal static class SpriteText_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(SpriteText), nameof(SpriteText.getColorFromIndex)),
				prefix: new HarmonyMethod(typeof(SpriteText_Patches), nameof(getColorFromIndex__Prefix))
			);
		} catch (Exception ex) {
			mod.Log("Unable to apply SpriteText patches due to error.", LogLevel.Error, ex);
		}
	}

	static bool getColorFromIndex__Prefix(int index, ref Color __result) {
		try {
			if (index >= 100) {
				__result = CommonHelper.UnpackColor(index - 100);
				return false;
			}

			var colors = Mod?.BaseTheme?.SpriteTextColors;
			if (colors is not null && colors.TryGetValue(index, out __result))
				return false;

		} catch(Exception ex) {
			Monitor?.LogOnce($"An error occurred in {nameof(getColorFromIndex__Prefix)}. Details:\n{ex}", LogLevel.Warn);
		}

		return true;
	}

}
