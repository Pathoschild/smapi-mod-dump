/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Items.Unlocks;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using xTile.ObjectModel;
using xTile.Tiles;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    public class MountainInjections
    {
        private const int RAT_PROBLEM_ID = 26;

        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _archipelago = archipelago;
        }

        // public void ApplyTreehouseIfNecessary()
        public static bool ApplyTreehouseIfNecessary_ApplyTreeHouseIfReceivedApItem_Prefix(Mountain __instance)
        {
            try
            {
                if (__instance.treehouseBuilt || !_archipelago.HasReceivedItem(VanillaUnlockManager.TREEHOUSE))
                {
                    return false; // don't run original logic
                }

                TileSheet tileSheet = __instance.map.GetTileSheet("untitled tile sheet2");
                __instance.map.GetLayer("Buildings").Tiles[16, 6] = new StaticTile(__instance.map.GetLayer("Buildings"), tileSheet, BlendMode.Alpha, 197);
                __instance.map.GetLayer("Buildings").Tiles[16, 7] = new StaticTile(__instance.map.GetLayer("Buildings"), tileSheet, BlendMode.Alpha, 213);
                __instance.map.GetLayer("Back").Tiles[16, 8] = new StaticTile(__instance.map.GetLayer("Back"), tileSheet, BlendMode.Alpha, 229);
                __instance.map.GetLayer("Buildings").Tiles[16, 7].Properties["Action"] = new PropertyValue("LockedDoorWarp 3 8 LeoTreeHouse 600 2300");
                __instance.treehouseBuilt = true;
                if (!Game1.IsMasterGame)
                {
                    return false; // don't run original logic
                }

                __instance.updateDoors();
                __instance.treehouseDoorDirty = true;

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ApplyTreehouseIfNecessary_ApplyTreeHouseIfReceivedApItem_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
