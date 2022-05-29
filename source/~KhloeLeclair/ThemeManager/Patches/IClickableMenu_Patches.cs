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
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using HarmonyLib;

using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using StardewModdingAPI;

namespace Leclair.Stardew.ThemeManager.Patches;

internal static class IClickableMenu_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.drawHoverText), new Type[] {
					typeof(SpriteBatch), typeof(StringBuilder), typeof(SpriteFont),
					typeof(int), typeof(int), typeof(int), typeof(string), typeof(int),
					typeof(string[]), typeof(Item),
					typeof(int), typeof(int), typeof(int), typeof(int), typeof(int),
					typeof(float), typeof(CraftingRecipe), typeof(IList<Item>)
				}),
				transpiler: new HarmonyMethod(typeof(IClickableMenu_Patches), nameof(DrawHoverText_Transpiler))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(Item), nameof(Item.drawTooltip)),
				transpiler: new HarmonyMethod(typeof(IClickableMenu_Patches), nameof(Item_drawTooltip_Transpiler))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(Boots), nameof(Boots.drawTooltip)),
				transpiler: new HarmonyMethod(typeof(IClickableMenu_Patches), nameof(Item_drawTooltip_Transpiler))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(MeleeWeapon), nameof(MeleeWeapon.drawTooltip)),
				transpiler: new HarmonyMethod(typeof(IClickableMenu_Patches), nameof(Item_drawTooltip_Transpiler))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(Ring), nameof(Ring.drawTooltip)),
				transpiler: new HarmonyMethod(typeof(IClickableMenu_Patches), nameof(Item_drawTooltip_Transpiler))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(CraftingRecipe), nameof(CraftingRecipe.drawRecipeDescription)),
				transpiler: new HarmonyMethod(typeof(IClickableMenu_Patches), nameof(CraftingRecipe_drawDescription_Transpiler))
			);

		} catch (Exception ex) {
			mod.Log("Unable to apply IClickableMenu patches due to error.", LogLevel.Error, ex);
		}
	}

	public static readonly Color MODIFIED_STAT_COLOR = new Color(0, 120, 120);
	public static readonly Color TOOL_ENCHANTMENT_COLOR = new Color(120, 0, 210);

	public static Color GetModifiedStatColor() {
		return Mod!.BaseTheme?.HoverTextModifiedStatTextColor ??
			MODIFIED_STAT_COLOR;
	}

	public static Color GetToolEnchantmentColor() {
		return Mod!.BaseTheme?.HoverTextEnchantmentTextColor ??
			TOOL_ENCHANTMENT_COLOR;
	}

	public static Color GetTextColor() {
		return Mod!.BaseTheme?.HoverTextTextColor ??
			Game1.textColor;
	}

	public static Color GetShadowColor() {
		return Mod!.BaseTheme?.HoverTextShadowColor ??
			Game1.textShadowColor;
	}

	public static Color GetInsufficientColor() {
		return Mod!.BaseTheme?.HoverTextInsufficientTextColor ??
			Color.Red;
	}

	public static Color GetForgeCountTextColor() {
		return Mod?.BaseTheme?.HoverTextForgeCountTextColor ??
			Color.DimGray;
	}

	public static Color GetForgedTextColor() {
		return Mod?.BaseTheme?.HoverTextForgedTextColor ??
			Color.DarkRed;
	}

	static IEnumerable<CodeInstruction> Item_drawTooltip_Transpiler(IEnumerable<CodeInstruction> instructions) {

		instructions = PatchUtils.ReplaceCalls(
			instructions: instructions,
			fieldReplacements: new Dictionary<string, string> {
				{ nameof(Game1.textColor), nameof(GetTextColor) },
				{ nameof(Game1.textShadowColor), nameof(GetShadowColor) }
			}.HydrateFieldKeys(typeof(Game1))
			.HydrateMethodValues(typeof(IClickableMenu_Patches))
		);

		return PatchUtils.ReplaceColors(
			instructions: instructions,
			new Dictionary<Color, string> {
				{ new Color(0, 120, 120), nameof(GetModifiedStatColor) },
				{ new Color(120, 0, 210), nameof(GetToolEnchantmentColor) },
			}.HydrateMethodValues(typeof(IClickableMenu_Patches))
		);

	}

	static IEnumerable<CodeInstruction> CraftingRecipe_drawDescription_Transpiler(IEnumerable<CodeInstruction> instructions) {

		return PatchUtils.ReplaceColors(
			instructions: instructions,
			type: typeof(IClickableMenu_Patches),
			replacements: new Dictionary<string, string> {
				{ nameof(Color.Red), nameof(GetInsufficientColor) }
			},
			fieldReplacements: new Dictionary<string, string> {
				{ nameof(Game1.textColor), nameof(GetTextColor) },
				{ nameof(Game1.textShadowColor), nameof(GetShadowColor) }
			}.HydrateFieldKeys(typeof(Game1)).HydrateMethodValues(typeof(IClickableMenu_Patches))
		);

	}

	static IEnumerable<CodeInstruction> DrawHoverText_Transpiler(IEnumerable<CodeInstruction> instructions) {

		return PatchUtils.ReplaceColors(
			instructions: instructions,
			type: typeof(IClickableMenu_Patches),
			replacements: new Dictionary<string, string> {
				{ nameof(Color.DimGray), nameof(GetForgeCountTextColor) },
				{ nameof(Color.DarkRed), nameof(GetForgedTextColor) }
			},
			fieldReplacements: new Dictionary<string, string> {
				{ nameof(Game1.textColor), nameof(GetTextColor) },
				{ nameof(Game1.textShadowColor), nameof(GetShadowColor) }
			}.HydrateFieldKeys(typeof(Game1)).HydrateMethodValues(typeof(IClickableMenu_Patches))
		);

	}

}
