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

using Microsoft.Xna.Framework;

using StardewModdingAPI;

using StardewValley;
using StardewValley.TerrainFeatures;

namespace Leclair.Stardew.GiantCropTweaks.Patches;

public static class HoeDirt_Patches {

	private static IMonitor? Monitor;

	public static void Patch(ModEntry mod) {
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(HoeDirt), nameof(GiantCrop.performToolAction)),
				prefix: new HarmonyMethod(typeof(HoeDirt_Patches), nameof(performToolAction_Prefix))
			);

		} catch (Exception ex) {
			mod.Log($"An error occurred while registering a harmony patch for HoeDirt.", LogLevel.Warn, ex);
		}

	}

	public static bool performToolAction_Prefix(HoeDirt __instance, GameLocation location, Vector2 tileLocation, ref bool __result) {
		// It's important not to break crops as part of breaking a giant crop, so
		// that our newly restored crops aren't immediately destroyed by the
		// player's axe.
		try {
			if (! ModEntry.Instance.CanBreakCrops.Value) {
				__result = false;
				return false;
			}

		} catch (Exception ex) {
			Monitor?.Log($"An error occurred while attempting to interact with HoeDirt.", LogLevel.Warn);
			Monitor?.Log($"Details:\n{ex}", LogLevel.Warn);
		}

		return true;
	}

}
