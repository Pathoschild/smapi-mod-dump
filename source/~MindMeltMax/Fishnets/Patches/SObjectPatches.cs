/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Linq;
using Object = StardewValley.Object;

namespace Fishnets.Patches
{
    internal static class SObjectPatches
    {
        private static IMonitor IMonitor => ModEntry.IMonitor;

        public static bool isPlaceablePrefix(Object __instance, ref bool __result)
        {
            try
            {
                if (__instance.ParentSheetIndex == ModEntry.FishNetId)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
            catch (Exception ex) { IMonitor.Log($"Faild patching {nameof(Object.isPlaceable)}", LogLevel.Error); IMonitor.Log($"{ex.Message}\n{ex.StackTrace}"); return true; }
        }

        public static bool canBePlacedHerePrefix(Object __instance, GameLocation l, Vector2 tile, ref bool __result)
        {
            try
            {
                if (__instance.ParentSheetIndex == ModEntry.FishNetId)
                {
                    __result = FishNet.IsValidPlacementLocation(l, (int)tile.X, (int)tile.Y);
                    return false;
                }
                return true;
            }
            catch (Exception ex) { IMonitor.Log($"Faild patching {nameof(Object.canBePlacedHere)}", LogLevel.Error); IMonitor.Log($"{ex.Message}\n{ex.StackTrace}"); return true; }
        }

        public static bool canBePlacedInWaterPrefix(Object __instance, ref bool __result)
        {
            try
            {
                if (__instance.ParentSheetIndex == ModEntry.FishNetId)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
            catch (Exception ex) { IMonitor.Log($"Faild patching {nameof(Object.canBePlacedInWater)}", LogLevel.Error); IMonitor.Log($"{ex.Message}\n{ex.StackTrace}"); return true; }
        }
        
        public static bool placementActionPrefix(Object __instance, ref bool __result, GameLocation location, int x, int y, Farmer who = null)
        {
            try
            {
                if (__instance.ParentSheetIndex == ModEntry.FishNetId)
                {
                    Vector2 tile = new((int)Math.Floor(x / 64f), (int)Math.Floor(y / 64f));
                    if (!FishNet.IsValidPlacementLocation(location, (int)tile.X, (int)tile.Y))
                        return false;
                    __result = new FishNet(tile).placementAction(location, x, y, who);
                    if (__result && __instance.stack <= 0)
                        Game1.player.removeItemFromInventory(__instance);
                    return false;
                }

                return true;
            }
            catch(Exception ex) 
            {
                IMonitor.Log($"Faild patching {nameof(Object.placementAction)}", LogLevel.Error); 
                IMonitor.Log($"{ex.Message}\n{ex.StackTrace}"); 
                return __instance is null || __instance.ParentSheetIndex != ModEntry.FishNetId; 
            }
        }
    }
}
