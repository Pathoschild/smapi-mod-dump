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
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

using Leclair.Stardew.Common;

using StardewModdingAPI;

using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Triggers;

namespace Leclair.Stardew.BetterCrafting.Patches;

public static class SObject_Patches {

	private static IMonitor? Monitor;

	public static void Patch(ModEntry mod) {
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(SObject), nameof(SObject.checkForAction)),
				postfix: new HarmonyMethod(typeof(SObject_Patches), nameof(checkForAction_Postfix))
			);
		} catch (Exception ex) {
			mod.Log("An error occurred while registering a harmony patch for the Workbench.", LogLevel.Warn, ex);
		}
	}

	public static void checkForAction_Postfix(SObject __instance, Farmer who, bool justCheckingForActivity, ref bool __result) {

		try {
			// If there was already an action, don't do more.
			if (__result)
				return;

			if (!__instance.bigCraftable.Value || !Game1.bigCraftableData.TryGetValue(__instance.ItemId, out var data))
				return;

			if (data.CustomFields is null || !data.CustomFields.TryGetValue("leclair.bettercrafting_PerformAction", out string? action) || string.IsNullOrEmpty(action))
				return;

			// Yes, we have an action.
			__result = true;

			// Don't perform our action if we're just checking.
			if (justCheckingForActivity)
				return;

			// Run the action.
			__instance.Location.performAction(action, who, __instance.TileLocation.ToLocation());

		} catch(Exception ex) {
			Monitor?.Log("An error occurred while attempting to interact with an object.", LogLevel.Warn);
			Monitor?.Log($"Details:\n{ex}", LogLevel.Warn);
		}

	}

}
