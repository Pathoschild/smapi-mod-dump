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
using StardewArchipelago.Constants;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Locations.InGameLocations
{
    internal class ArchipelagoLocationsInjections
    {
        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(IMonitor monitor, IModHelper helper, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _helper = helper;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // public void OnItemReceived(Item item, int countAdded, Item mergedIntoStack, bool hideHudNotification = false)
        public static bool OnItemReceived_PickUpACheck_Prefix(Farmer __instance, Item item, int countAdded, Item mergedIntoStack, bool hideHudNotification)
        {
            try
            {
                if (__instance == null || item == null)
                {
                    return true; // run original logic
                }

                if (item is not ArchipelagoLocation apLocation)
                {
                    return true; // run original logic
                }

                apLocation.SendCheck();
                __instance.removeItemFromInventory(item);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(OnItemReceived_PickUpACheck_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public bool couldInventoryAcceptThisItem(string id, int stack, int quality = 0)
        public static bool CouldInventoryAcceptThisItemById_ChecksFlyingAround_Prefix(Farmer __instance, string id, int stack, int quality, ref bool __result)
        {
            try
            {
                if (id == null)
                {
                    return true; // run original logic
                }

                if (id.Contains(IDProvider.AP_LOCATION, StringComparison.InvariantCultureIgnoreCase))
                {
                    __result = true;
                    return false; // don't run original logic
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CouldInventoryAcceptThisItemById_ChecksFlyingAround_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public bool couldInventoryAcceptThisItem(Item item)
        public static bool CouldInventoryAcceptThisItemByItem_ChecksFlyingAround_Prefix(Farmer __instance, Item item, ref bool __result)
        {
            try
            {
                if (item is ArchipelagoLocation)
                {
                    __result = true;
                    return false; // don't run original logic
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CouldInventoryAcceptThisItemByItem_ChecksFlyingAround_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
