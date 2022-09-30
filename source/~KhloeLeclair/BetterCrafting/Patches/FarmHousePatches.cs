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

using Microsoft.Xna.Framework;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

using xTile.Dimensions;

namespace Leclair.Stardew.BetterCrafting.Patches;

public static class GameLocation_Patches {

	private static IMonitor? Monitor;

	public static void Patch(ModEntry mod) {
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(FarmHouse), nameof(FarmHouse.performAction)),
				prefix: new HarmonyMethod(typeof(GameLocation_Patches), nameof(performAction_Prefix))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(FarmHouse), nameof(IslandFarmHouse.performAction)),
				prefix: new HarmonyMethod(typeof(GameLocation_Patches), nameof(performAction_Prefix))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction)),
				prefix: new HarmonyMethod(typeof(GameLocation_Patches), nameof(performAction_Prefix))
			);

		} catch (Exception ex) {
			mod.Log("An error occurred while registering a harmony patch for the FarmHouse.", LogLevel.Warn, ex);
		}
	}

	public static bool performAction_Prefix(GameLocation __instance, string action, Farmer who, Location tileLocation, ref bool __result) {
		try {
			if (action is not null && who.IsLocalPlayer && action.Split(' ')[0] == "kitchen") {
				// It's the kitchen! Handle it ourselves.
				ModEntry mod = ModEntry.Instance;
				if (mod.Config.ReplaceCrafting && !(mod.Config.SuppressBC?.IsDown() ?? false)) {

					// TODO: Build containers list!

					Game1.activeClickableMenu = Menus.BetterCraftingPage.Open(
						mod,
						location: __instance,
						position: new Vector2(tileLocation.X, tileLocation.Y),
						standalone_menu: true,
						cooking: true,
						material_containers: (IList<object>?) null
					);

					__result = true;
					return false;
				}
			}

		} catch(Exception ex) {
			Monitor?.Log($"An error occurred while attempting to perform a location action.", LogLevel.Warn);
			Monitor?.Log($"Details:\n{ex}", LogLevel.Warn);
		}

		return true;
	}


}
