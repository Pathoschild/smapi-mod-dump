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
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Tools;

namespace Leclair.Stardew.ThemeManager.Patches;

internal static class OptionsDropDown_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	private static OptionsDropDown? Current;
	private static int LastOption;
	private static int CurrentX;
	private static int CurrentY;

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		Mod.Helper.Events.Input.CursorMoved += OnCursorMoved;
		Mod.Helper.Events.Input.ButtonPressed += OnButtonPressed;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(OptionsDropDown), nameof(OptionsDropDown.draw)),
				prefix: new HarmonyMethod(typeof(OptionsDropDown_Patches), nameof(Draw_Prefix)),
				postfix: new HarmonyMethod(typeof(OptionsDropDown_Patches), nameof(Draw_Postfix))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(OptionsDropDown), nameof(OptionsDropDown.receiveLeftClick)),
				prefix: new HarmonyMethod(typeof(OptionsDropDown_Patches), nameof(LeftClickPressed_Prefix))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(OptionsDropDown), nameof(OptionsDropDown.leftClickHeld)),
				prefix: new HarmonyMethod(typeof(OptionsDropDown_Patches), nameof(LeftClickHeld_Prefix))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(OptionsDropDown), nameof(OptionsDropDown.leftClickReleased)),
				prefix: new HarmonyMethod(typeof(OptionsDropDown_Patches), nameof(LeftClickReleased_Prefix))
			);

		} catch(Exception ex) {
			mod.Log($"Unable to apply OptionsDropDown patches due to error.", LogLevel.Error, ex);
		}
	}

	static void CloseCurrent() {
		if (Current is not null) {
			var field = Mod?.Helper.Reflection.GetField<bool>(Current, "clicked", false);
			field?.SetValue(false);
			Current.selectedOption = LastOption;
			Current = null;
		}
	}

	static void OnButtonPressed(object? sender, ButtonPressedEventArgs e) {
		if (Current is not null && ! e.IsSuppressed() && e.Button == SButton.MouseLeft) {
			var field = Mod?.Helper.Reflection.GetField<bool>(Current, "clicked", false);
			bool clicked = field?.GetValue() ?? false;
			if (!clicked)
				return;

			Mod!.Helper.Input.Suppress(e.Button);
			var pos = e.Cursor.GetScaledScreenPixels();

			// Do the bounds contain the dropdown?
			if (Current.dropDownBounds.Contains(pos.X - CurrentX, pos.Y - CurrentY) || (Game1.options.gamepadControls && ! Game1.lastCursorMotionWasMouse)) {
				Game1.playSound("drumkit6");
				field?.SetValue(false);
				OptionsDropDown.selected = Current;
				Game1.options.changeDropDownOption(Current.whichOption, Current.dropDownOptions[Current.selectedOption]);
				OptionsDropDown.selected = null;

			} else
				CloseCurrent();
		}
	}

	static void OnCursorMoved(object? sender, CursorMovedEventArgs e) {
		if (Current is not null) {
			var field = Mod?.Helper.Reflection.GetField<bool>(Current, "clicked", false);
			bool clicked = field?.GetValue() ?? false;
			if (!clicked) {
				Current = null;
				return;
			}

			var pos = e.NewPosition.GetScaledScreenPixels();

			// Do the bounds contain the dropdown?
			if (Current.dropDownBounds.Contains(pos.X - CurrentX, pos.Y - CurrentY)) {
				Current.selectedOption = (int) Math.Max(Math.Min((float) (pos.Y - CurrentY - Current.dropDownBounds.Y) / (float) Current.bounds.Height, Current.dropDownOptions.Count - 1), 0);
			}
		}
	}


	static bool LeftClickPressed_Prefix(OptionsDropDown __instance, int x, int y) {
		try {
			if (!__instance.greyedOut) {
				if (Current != __instance) {
					CloseCurrent();

					Current = __instance;
					var field = Mod?.Helper.Reflection.GetField<bool>(Current, "clicked", false);
					field?.SetValue(true);
					LastOption = Current.selectedOption;
				}

				return false;
			}

		} catch (Exception ex) {
			Monitor?.LogOnce($"An error occurred in {nameof(LeftClickPressed_Prefix)}: {ex}", LogLevel.Error);
		}

		return true;
	}

	static bool LeftClickHeld_Prefix(OptionsDropDown __instance, int x, int y) {
		try {
			if (!__instance.greyedOut) {
				return false;
			}

		} catch (Exception ex) {
			Monitor?.LogOnce($"An error occurred in {nameof(LeftClickHeld_Prefix)}: {ex}", LogLevel.Error);
		}

		return true;
	}

	static bool LeftClickReleased_Prefix(OptionsDropDown __instance, int x, int y) {
		try {
			if (!__instance.greyedOut) {
				return false;
			}

		} catch(Exception ex) {
			Monitor?.LogOnce($"An error occurred in {nameof(LeftClickReleased_Prefix)}: {ex}", LogLevel.Error);
		}

		return true;
	}

	static bool Draw_Prefix(OptionsDropDown __instance, SpriteBatch b, int slotX, int slotY, IClickableMenu context) {
		try {
			if (__instance == Current) {
				CurrentX = slotX;
				CurrentY = slotY;
			}

		} catch(Exception ex) {
			Monitor?.LogOnce($"An error occurred in {nameof(Draw_Prefix)}: {ex}", LogLevel.Error);
		}

		return true;
	}

	static void Draw_Postfix(OptionsDropDown __instance, SpriteBatch b, int slotX, int slotY, IClickableMenu context) {
		if (__instance.greyedOut)
			return;

		bool clicked = Mod?.Helper.Reflection.GetField<bool>(__instance, "clicked", false)?.GetValue() ?? false;
		if (clicked) {
			// TODO: Draw scroll handle.

		}
	}

}
