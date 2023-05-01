/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using Object = StardewValley.Object;

namespace BNWCore
{
    internal static class SObjectPatches
    {
        public static bool isPlaceablePrefix(Object __instance, ref bool __result)
        {
            try
            {
                if (__instance.ParentSheetIndex == ModEntry.BNWCoreMagicNetId)
                {
                    __result = true;
                    return false;
                }
            }
            catch (Exception){}
            return true;
        }
        public static bool canBePlacedHerePrefix(Object __instance, GameLocation l, Vector2 tile, ref bool __result)
        {
            try
            {
                if (__instance.ParentSheetIndex == ModEntry.BNWCoreMagicNetId)
                {
                    __result = BNWCoreMagicNet.IsValidPlacementLocation(l, (int)tile.X, (int)tile.Y);
                    return false;
                }
            }
            catch (Exception){}
            return true;
        }
        public static bool canBePlacedInWaterPrefix(Object __instance, ref bool __result)
        {
            try
            {
                if (__instance.ParentSheetIndex == ModEntry.BNWCoreMagicNetId)
                {
                    __result = true;
                    return false;
                }             
            }
            catch (Exception) {}
            return true;
        }       
        public static bool placementActionPrefix(Object __instance, ref bool __result, GameLocation location, int x, int y, Farmer who = null)
        {
            try
            {
                if (__instance.ParentSheetIndex == ModEntry.BNWCoreMagicNetId)
                {
                    Vector2 tile = new((int)Math.Floor(x / 64f), (int)Math.Floor(y / 64f));
                    if (!BNWCoreMagicNet.IsValidPlacementLocation(location, (int)tile.X, (int)tile.Y))
                        return false;
                    __result = new BNWCoreMagicNet(tile).placementAction(location, x, y, who);
                    if (__result && __instance.Stack <= 0)
                        Game1.player.removeItemFromInventory(__instance);
                    return false;
                }

                return true;
            }
            catch(Exception) 
            {
                return __instance is null || __instance.ParentSheetIndex != ModEntry.BNWCoreMagicNetId; 
            }
        }
    }
}
