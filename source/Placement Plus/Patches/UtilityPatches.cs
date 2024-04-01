/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/2Retr0/PlacementPlus
**
*************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

using static PlacementPlus.Utility.Utility;
using static PlacementPlus.ModState;

namespace PlacementPlus.Patches
{
    [HarmonyPatch(typeof(StardewValley.Utility), nameof(StardewValley.Utility.playerCanPlaceItemHere))]
    internal class UtilityPatches
    {
        /// <summary>
        /// Alters the requirements for where certain objects can be placed. This visually translates to altering the
        /// cursor tile to tint green when an object can be swapped with another.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleLossOfFraction")]
        private static void Postfix(GameLocation location, Item item, int x, int y, Farmer f, ref bool __result)
        {
            try
            {
                var tilePos = new Vector2(x / 64, y / 64);
                var tileObject = location.getObjectAt(x, y);

                // As we are only widening where objects can be placed, if it has already been determined that that the
                // object can be placed, we skip our logic.
                if (__result || !StardewValley.Utility.tileWithinRadiusOfPlayer((int)tilePos.X, (int)tilePos.Y, 1, f)) 
                    return;

                if (IsItemFlooring(item))
                {
                    // Assume that any tile that has an object is a valid tile for flooring to be placed.
                    __result = tileObject != null ||
                               (location is Farm farm && (IsTileOnBuildingEdge(farm, tilePos) || DoesTileHaveMainMailbox(farm, tilePos)));
                }
                else if (IsItemFence(item))
                {
                    __result = IsItemFence(tileObject);
                }
            } catch (Exception e) {
                Monitor.Log($"Failed in {nameof(UtilityPatches)}:\n{e}", LogLevel.Error);
            }
        }
    }
}