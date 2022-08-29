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
using StardewValley.Locations;
using xTile.Dimensions;

namespace PlacementPlus.Patches
{
    [HarmonyPatch(typeof(BuildableGameLocation), nameof(BuildableGameLocation.isBuildable))]
    internal class BuildableGameLocationPatches
    {
        private static IMonitor Monitor => PlacementPlus.Instance.Monitor;

        /// <summary> Alters the requirements for where buildings can be built. </summary>
        private static void Postfix(Vector2 tileLocation, BuildableGameLocation __instance, ref bool __result)
        {
            try
            {
                var location = new Location((int)tileLocation.X, (int)tileLocation.Y);

                // Define new (loosened) requirements for building placement.
                var playerIsNotOnTile = !Game1.player.getTileLocation().Equals(tileLocation);
                var tileIsNotOccupied = !__instance.isTileOccupiedForPlacement(tileLocation);
                var tileIsPassable = __instance.isTilePassable(location, Game1.viewport);
                var tileHasNoFurniture = __instance.GetFurnitureAt(tileLocation) == null;

                __result = playerIsNotOnTile && tileIsNotOccupied && tileIsPassable && tileHasNoFurniture;
            }
            catch (Exception e) {
                Monitor.Log($"Failed in {nameof(BuildableGameLocationPatches)}:\n{e}", LogLevel.Error);
            }
        }
    }
}