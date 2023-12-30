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
using System.Collections.Generic;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class CookingInjections
    {
        private const string COOKING_LOCATION_PREFIX = "Cook ";

        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;
        private static StardewItemManager _itemManager;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago, LocationChecker locationChecker, StardewItemManager itemManager)
        {
            _monitor = monitor;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _itemManager = itemManager;
        }

        // public void cookedRecipe(int index)
        public static void CookedRecipe_CheckCooksanityLocation_Postfix(Farmer __instance, int index)
        {
            try
            {
                if (!_itemManager.ObjectExists(index))
                {
                    _monitor.Log($"Unrecognized cooked recipe: {index}", LogLevel.Warn);
                    return;
                }

                var cookedItem = _itemManager.GetObjectById(index);
                var cookedItemName = cookedItem.Name;
                if (_renamedItems.ContainsKey(index))
                {
                    cookedItemName = _renamedItems[index];
                }
                var apLocation = $"{COOKING_LOCATION_PREFIX}{cookedItemName}";
                if (_archipelago.GetLocationId(apLocation) > -1)
                {
                    _locationChecker.AddCheckedLocation(apLocation);
                    return;
                }

                _monitor.Log($"Unrecognized Cooksanity Location: {cookedItemName} [{index}]", LogLevel.Error);
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CookedRecipe_CheckCooksanityLocation_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static readonly Dictionary<int, string> _renamedItems = new()
        {
            { 223, "Cookies" },
        };
    }
}
