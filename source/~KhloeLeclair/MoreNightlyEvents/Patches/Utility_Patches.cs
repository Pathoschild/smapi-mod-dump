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

using StardewModdingAPI;

using StardewValley;
using StardewValley.Events;

namespace Leclair.Stardew.MoreNightlyEvents.Patches;

internal static class Utility_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		try {

			mod.Harmony.Patch(
				original: AccessTools.Method(typeof(Utility), nameof(Utility.pickFarmEvent)),
				postfix: new HarmonyMethod(typeof(Utility_Patches), nameof(pickFarmEvent__Postfix))
			);

		} catch(Exception ex) {
			mod.Log($"Unable to apply Utility patches due to an error. This mod will not function.", LogLevel.Error, ex);
		}
	}

	internal static void pickFarmEvent__Postfix(ref FarmEvent? __result) {
		try {
			__result = Mod!.PickEvent(__result);

		} catch(Exception ex) {
			Monitor?.Log("An error occurred within pickFarmEvent.", LogLevel.Error);
			Monitor?.Log($"Details:\n{ex}", LogLevel.Error);
		}
	}

}
