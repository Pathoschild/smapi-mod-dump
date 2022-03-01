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

using HarmonyLib;

using StardewModdingAPI;

using StardewValley;

namespace Leclair.Stardew.Hydrology.Patches {
	public class UtilityPatches {

		public static IMonitor Monitor => ModEntry.instance.Monitor;

		[HarmonyPatch(typeof(Utility), nameof(Utility.isWithinTileWithLeeway))]
		public static class WithinTileWithLeeway {

			static bool Prefix(int x, int y, Item item, Farmer f, ref bool __result) {
				try {
					if (item is WaterFeatureObject) {
						__result = true;
						return false;
					}

				} catch (Exception ex) {
					Monitor.Log($"Failed in {nameof(WithinTileWithLeeway)}:\n{ex}", LogLevel.Error);
				}

				return true;
			}

		}

	}
}
