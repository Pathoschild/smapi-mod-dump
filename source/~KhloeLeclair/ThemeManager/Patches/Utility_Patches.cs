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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using HarmonyLib;

using StardewValley;
using StardewModdingAPI;

using Leclair.Stardew.Common;

namespace Leclair.Stardew.ThemeManager.Patches;

internal static class Utility_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(Utility), nameof(Utility.drawTextWithShadow), new Type[] {
					typeof(SpriteBatch), typeof(StringBuilder), typeof(SpriteFont), typeof(Vector2), typeof(Color),
					typeof(float), typeof(float), typeof(int), typeof(int), typeof(float), typeof(int)
				}),
				transpiler: new HarmonyMethod(typeof(Utility_Patches), nameof(DrawTextWithShadow_Transpiler))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(Utility), nameof(Utility.drawTextWithShadow), new Type[] {
					typeof(SpriteBatch), typeof(string), typeof(SpriteFont), typeof(Vector2), typeof(Color),
					typeof(float), typeof(float), typeof(int), typeof(int), typeof(float), typeof(int)
				}),
				transpiler: new HarmonyMethod(typeof(Utility_Patches), nameof(DrawTextWithShadow_Transpiler))
			);

		} catch (Exception ex) {
			mod.Log("Unable to apply Utility patches due to error.", LogLevel.Error, ex);
		}
	}

	private static readonly Color DEFAULT_SHADOW_COLOR = new(221, 148, 84);

	public static Color GetShadowColor() {
		return Mod?.BaseTheme?.TextShadowAltColor ?? DEFAULT_SHADOW_COLOR;
	}

	static IEnumerable<CodeInstruction> DrawTextWithShadow_Transpiler(IEnumerable<CodeInstruction> instructions) {

		return PatchUtils.ReplaceColors(
			instructions: instructions,
			replacements: new Dictionary<Color, string> {
				{ DEFAULT_SHADOW_COLOR, nameof(GetShadowColor) }
			}.HydrateMethodValues(typeof(Utility_Patches))
		);

	}

}
