/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/2Retr0/PlacementPlus
**
*************************************************/

#region

using System;
using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using xTile.Dimensions;

#endregion

namespace PlacementPlus.Patches
{
    // Lost code: https://pastebin.com/ZhRqL54L
    [HarmonyPatch(typeof(BuildableGameLocation), nameof(BuildableGameLocation.isBuildable))]
    internal class BuildableGameLocationPatches_IsBuildable
    {
        private static IMonitor Monitor => PlacementPlus.Instance.Monitor;
        
        private static void Postfix(Vector2 tileLocation, BuildableGameLocation __instance, ref bool __result)
        {
            try
            {
                var playerIsNotOnTile  = !Game1.player.getTileLocation().Equals(tileLocation) || !Game1.player.currentLocation.Equals(__instance);
                var tileIsNotOccupied  = !__instance.isTileOccupiedForPlacement(tileLocation);
                var tileIsPassable     =  __instance.isTilePassable(new Location((int) tileLocation.X, (int) tileLocation.Y), Game1.viewport);
                var tileHasNoFurniture =  __instance.GetFurnitureAt(tileLocation) == null;

                if (!(playerIsNotOnTile   &&
                      tileIsNotOccupied   &&
                      tileIsPassable      &&
                      tileHasNoFurniture)
                ) return; // Run original logic.
                
                __result = true; // Original method will now return true.
            } catch (Exception e) {
                Monitor.Log($"Failed in {nameof(BuildableGameLocationPatches_IsBuildable)}:\n{e}", LogLevel.Error);
            }
        }
    }
}