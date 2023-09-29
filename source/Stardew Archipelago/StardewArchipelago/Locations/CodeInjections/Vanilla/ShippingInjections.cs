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
using System.Linq;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Goals;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public static class ShippingInjections
    {
        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;
        private static LocationChecker _locationChecker;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _monitor = monitor;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
        }

        // private static IEnumerator<int> _newDayAfterFade()
        public static bool NewDayAfterFade_CheckShipsanityLocations_Prefix(ref IEnumerator<int> __result)
        {
            try
            {
                var allShippedItems = GetAllItemsShippedToday();
                CheckAllShipsanityLocations(allShippedItems);
                
                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(NewDayAfterFade_CheckShipsanityLocations_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // private static IEnumerator<int> _newDayAfterFade()
        public static void NewDayAfterFade_CheckGoalCompletion_Postfix(ref IEnumerator<int> __result)
        {
            try
            {
                GoalCodeInjection.CheckFullShipmentGoalCompletion();
                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(NewDayAfterFade_CheckGoalCompletion_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static List<Item> GetAllItemsShippedToday()
        {
            var allShippedItems = new List<Item>();
            allShippedItems.AddRange(Game1.getFarm().getShippingBin(Game1.player));
            foreach (var gameLocation in GetAllGameLocations())
            {
                foreach (var locationObject in gameLocation.Objects.Values)
                {
                    if (locationObject is not Chest { SpecialChestType: Chest.SpecialChestTypes.MiniShippingBin } chest)
                    {
                        continue;
                    }

                    allShippedItems.AddRange(chest.items);
                }
            }

            return allShippedItems;
        }

        private static IEnumerable<GameLocation> GetAllGameLocations()
        {
            foreach (var location in Game1.locations)
            {
                yield return location;
                if (location is not BuildableGameLocation buildableLocation)
                {
                    continue;
                }

                foreach (var building in buildableLocation.buildings.Where(building => building.indoors.Value != null))
                {
                    yield return building.indoors.Value;
                }
            }
        }

        private static void CheckAllShipsanityLocations(List<Item> allShippedItems)
        {
            foreach (var shippedItem in allShippedItems)
            {
                var name = shippedItem.Name;
                if (_renamedItems.ContainsKey(shippedItem.ParentSheetIndex))
                {
                    name = _renamedItems[shippedItem.ParentSheetIndex];
                }

                var apLocation = $"Shipsanity: {name}";
                if (_archipelago.GetLocationId(apLocation) > -1)
                {
                    _locationChecker.AddCheckedLocation(apLocation);
                }
                else
                {
                    _monitor.Log($"Unrecognized Shipsanity Location: {name} [{shippedItem.ParentSheetIndex}]", LogLevel.Error);
                }
            }
        }

        private static readonly Dictionary<int, string> _renamedItems = new()
        {
            { 180, "Egg (Brown)" },
            { 182, "Large Egg (Brown)" },
            { 438, "Large Goat Milk" },
        };
    }
}
