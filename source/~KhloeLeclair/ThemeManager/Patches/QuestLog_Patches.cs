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

using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using HarmonyLib;

using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;

namespace Leclair.Stardew.ThemeManager.Patches;

internal static class QuestLog_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(QuestLog), nameof(QuestLog.draw), new Type[] {
					typeof(SpriteBatch)
				}),
				transpiler: new HarmonyMethod(typeof(QuestLog_Patches), nameof(Draw_Transpiler))
			);

		} catch (Exception ex) {
			mod.Log("Unable to apply QuestLog patches due to error.", LogLevel.Error, ex);
		}
	}

	public static Color GetTextColor() {
		return Game1.textColor;
	}

	public static Color GetHoverColor() {
		return Mod?.BaseTheme?.QuestHoverColor ?? Mod?.BaseTheme?.HoverColor ?? Color.Wheat;
	}

	public static Color GetObjectiveColor() {
		return Mod?.BaseTheme?.QuestObjectiveTextColor ?? Color.DarkBlue;
	}

	public static Color GetBarIncompleteColor() {
		return Mod?.BaseTheme?.QuestBarIncompleteColor ?? Color.Red;
	}

	public static Color GetBarIncompleteDarkColor() {
		return Mod?.BaseTheme?.QuestBarIncompleteDarkColor ?? Color.DarkRed;
	}

	public static Color GetBarCompleteColor() {
		return Mod?.BaseTheme?.QuestBarCompleteColor ?? Color.LimeGreen;
	}

	public static Color GetBarCompleteDarkColor() {
		return Mod?.BaseTheme?.QuestBarCompleteDarkColor ?? Color.Green;
	}

	static IEnumerable<CodeInstruction> Draw_Transpiler(IEnumerable<CodeInstruction> instructions) {
		return PatchUtils.ReplaceColors(
			instructions: instructions,
			type: typeof(QuestLog_Patches),
			replacements: new Dictionary<string, string> {
				{ nameof(Color.Wheat), nameof(GetHoverColor) },
				{ nameof(Color.DarkBlue), nameof(GetObjectiveColor) },
				{ nameof(Color.Red), nameof(GetBarIncompleteColor) },
				{ nameof(Color.DarkRed), nameof(GetBarIncompleteDarkColor) },
				{ nameof(Color.LimeGreen), nameof(GetBarCompleteColor) },
				{ nameof(Color.Green), nameof(GetBarCompleteDarkColor) }
			},
			fieldReplacements: new Dictionary<string, string> {
				{ nameof(Game1.textColor), nameof(GetTextColor) },
			}.HydrateFieldKeys(typeof(Game1)).HydrateMethodValues(typeof(QuestLog_Patches))
		);
	}

}
