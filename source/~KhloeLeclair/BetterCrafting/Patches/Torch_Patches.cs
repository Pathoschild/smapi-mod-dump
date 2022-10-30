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

using HarmonyLib;

using StardewModdingAPI;

using StardewValley;

namespace Leclair.Stardew.BetterCrafting.Patches;

public static class Torch_Patches {

	private static IMonitor? Monitor;

	public static void Patch(ModEntry mod) {
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(Torch), nameof(Torch.checkForAction)),
				prefix: new HarmonyMethod(typeof(Torch_Patches), nameof(checkForAction_Prefix))
			);
		} catch(Exception ex) {
			mod.Log("An error occurred while registering a harmony patch for the Cookout Kit.", LogLevel.Warn, ex);
		}
	}

	public static bool checkForAction_Prefix(Torch __instance, Farmer who, bool justCheckingForActivity, ref bool __result) {
		try {
			// We only care about the Cookout Kit specifically.
			if (!__instance.bigCraftable.Value || __instance.ParentSheetIndex != 278)
				return true;

			ModEntry mod = ModEntry.Instance;
			if (mod.Config.EnableCookoutWorkbench && !(mod.Config.SuppressBC?.IsDown() ?? false)) {
				// If we're not just checking, open the menu.
				if (!justCheckingForActivity && Game1.activeClickableMenu is null)
					Game1.activeClickableMenu = Menus.BetterCraftingPage.Open(
						mod,
						who.currentLocation,
						__instance.TileLocation,
						standalone_menu: true,
						cooking: true,
						material_containers: (IList<object>?) null
					);

				// Return true and don't call the original method.
				__result = true;
				return false;
			}

		} catch(Exception ex) {
			Monitor?.Log("An error occurred while attempting to interact with a Torch.", LogLevel.Warn);
			Monitor?.Log($"Details:\n{ex}", LogLevel.Warn);
		}

		return true;
	}

}
