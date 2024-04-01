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
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using xTile.Dimensions;

using static PlacementPlus.ModState;

namespace PlacementPlus.Patches
{
    [HarmonyPatch(typeof(GameLocation), nameof(GameLocation.isBuildable))]
    internal class GameLocationPatches
    {
        /// <summary> Alters the requirements for where buildings can be built. </summary>
        private static void Postfix(Vector2 tileLocation, GameLocation __instance, ref bool __result)
        {
            try
            {
                var location = new Location((int)tileLocation.X, (int)tileLocation.Y);

                // Define new (loosened) requirements for building placement.
                var playerIsNotOnTile = !Game1.player.Tile.Equals(tileLocation);
                var tileIsNotOccupied = !__instance.CanItemBePlacedHere(tileLocation);
                var tileIsPassable = __instance.isTilePassable(location, Game1.viewport);
                var tileHasNoFurniture = __instance.GetFurnitureAt(tileLocation) == null;

                __result |= playerIsNotOnTile && tileIsNotOccupied && tileIsPassable && tileHasNoFurniture;
            }
            catch (Exception e) {
                Monitor.Log($"Failed in {nameof(GameLocationPatches)}:\n{e}", LogLevel.Error);
            }
        }
    }
}