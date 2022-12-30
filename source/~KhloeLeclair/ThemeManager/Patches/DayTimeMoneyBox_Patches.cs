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

using Leclair.Stardew.Common.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using HarmonyLib;

using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;

namespace Leclair.Stardew.ThemeManager.Patches;

internal static class DayTimeMoneyBox_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(DayTimeMoneyBox), "updatePosition"),
				prefix: new HarmonyMethod(
					typeof(DayTimeMoneyBox_Patches),
					nameof(UpdatePosition_Prefix)
				)
			);

		} catch (Exception ex) {
			mod.Log("Unable to apply DayTimeMoneyBox patches due to error.", LogLevel.Error, ex);
		}
	}

	static bool UpdatePosition_Prefix(DayTimeMoneyBox __instance) {
		try {
			var mode = Mod?.Config?.ClockMode;
			if (!mode.HasValue || mode.Value == ClockAlignMode.Default)
				return true;

			Alignment? alignment = mode.Value == ClockAlignMode.ByTheme ?
				Mod!.GameTheme?.DayTimeAlignment :
				Mod!.Config.ClockAlignment;
			int? offsetX = Mod?.GameTheme?.DayTimeOffsetX;
			int? offsetY = Mod?.GameTheme?.DayTimeOffsetY;

			if (alignment.HasValue || offsetX.HasValue || offsetY.HasValue) {
				Alignment align = alignment ?? Alignment.None;

				int offX = offsetX ?? 0;
				int offY = offsetY ?? 8;

				int posX;
				int posY;

				if (align.HasFlag(Alignment.Left)) {
					posX = offX;

				} else if (align.HasFlag(Alignment.Center)) {
					posX = (Game1.uiViewport.Width - 300) / 2 + offX;

				} else {
					posX = Game1.uiViewport.Width - 300 - offX;

					if (Game1.isOutdoorMapSmallerThanViewport())
						posX = Math.Min(
							posX,
							-Game1.uiViewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 - 300 - offX
						);
				}

				if (align.HasFlag(Alignment.Middle)) {
					posY = (Game1.uiViewport.Height - 284) / 2 + offY;

				} else if (align.HasFlag(Alignment.Bottom)) {
					posY = Game1.uiViewport.Height - 284 - offY;

				} else {
					posY = offY;
				}

				Utility.makeSafe(ref posX, ref posY, 300, 284);

				// Check if the position changed, and do an early exit if
				// possible to avoid doing extra work.
				if (posX == __instance.position.X && posY == __instance.position.Y)
					return false;

				__instance.position = new Vector2(posX, posY);
				__instance.xPositionOnScreen = posX;
				__instance.yPositionOnScreen = posY;

				__instance.questButton.bounds = new Rectangle(
					posX + 212, posY + 240, 44, 46
				);

				__instance.zoomOutButton.bounds = new Rectangle(
					posX + 92, posY + 244, 28, 32
				);

				__instance.zoomInButton.bounds = new Rectangle(
					posX + 124, posY + 244, 28, 32
				);

				return false;
			}

		} catch (Exception ex) {
			Monitor?.LogOnce($"An error occurred in UpdatePosition_Prefix. Details:\n{ex}", LogLevel.Error);
		}

		return true;
	}

}
