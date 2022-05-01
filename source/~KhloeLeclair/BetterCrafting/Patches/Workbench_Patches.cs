/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections.Generic;

using HarmonyLib;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Objects;

namespace Leclair.Stardew.BetterCrafting.Patches;

public static class Workbench_Patches {

	private static IMonitor? Monitor;

	public static void Patch(ModEntry mod) {
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(Workbench), nameof(Workbench.checkForAction)),
				prefix: new HarmonyMethod(typeof(Workbench_Patches), nameof(Workbench_Patches.checkForAction_Prefix))
			);
		} catch(Exception ex) {
			mod.Log("An error occurred while registering a harmony patch for the Workbench.", LogLevel.Warn, ex);
		}
	}

	public static bool checkForAction_Prefix(Workbench __instance, Farmer who, bool justCheckingForActivity, ref bool __result) {
		try {
			ModEntry mod = ModEntry.Instance;
			if (mod.Config.ReplaceCrafting && !(mod.Config.SuppressBC?.IsDown() ?? false)) {
				if (justCheckingForActivity) {
					__result = true;
					return false;
				}

				Game1.activeClickableMenu = Menus.BetterCraftingPage.Open(
					mod,
					who.currentLocation,
					__instance.TileLocation,
					standalone_menu: true,
					material_containers: (IList<object>?) null
				);

				return false;
			}
		} catch(Exception ex) {
			Monitor?.Log("An error occurred while attempting to interact with a Workbench.", LogLevel.Warn);
			Monitor?.Log($"Details:\n{ex}", LogLevel.Warn);
		}

		return true;
	}

}
