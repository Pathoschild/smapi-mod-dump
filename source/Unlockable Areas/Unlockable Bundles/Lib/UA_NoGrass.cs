/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace Unlockable_Bundles.Lib
{
    public class UA_NoGrass
    {
        public static Mod Mod;
        public static IMonitor Monitor;
        public static IModHelper Helper;

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = ModEntry._Monitor;
            Helper = ModEntry._Helper;

            var harmony = new Harmony(Mod.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.placementAction)),
                prefix: new HarmonyMethod(typeof(UA_NoGrass), nameof(UA_NoGrass.placementAction_Prefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.playerCanPlaceItemHere)),
                prefix: new HarmonyMethod(typeof(UA_NoGrass), nameof(UA_NoGrass.playerCanPlaceItemHere_Prefix))
            );
        }

        //These Objects + Bombs ignore the Placeable check because they return isPassable true
        private static List<int> BannedObjects = new List<int>() {
            93, // Torch
            297, //Grass Starters
            293, 328, 329, 331, 333, 401, 405, 407, 409, 411, 415, 840, 841, //floors/paths
        };

        public static bool playerCanPlaceItemHere_Prefix(ref bool __result, GameLocation location, Item item, int x, int y, Farmer f)
        {
            if ((item is StardewValley.Object && (item as StardewValley.Object).bigCraftable.Value) || item is StardewValley.Objects.Furniture)
                return true;

            if (!BannedObjects.Contains(item.ParentSheetIndex))
                return true;

            Vector2 placementTile = new Vector2(x / 64, y / 64);
            var res = location.doesTileHavePropertyNoNull((int)placementTile.X, (int)placementTile.Y, "UA_NoGrass", "Back") != "T";

            if (!res) {
                __result = false;
                return false;
            }

            return true;
        }

        public static bool placementAction_Prefix(StardewValley.Object __instance, ref bool __result, GameLocation location, int x, int y, Farmer who = null)
        {
            return playerCanPlaceItemHere_Prefix(ref __result, location, __instance, x, y, who);
        }
    }
}
