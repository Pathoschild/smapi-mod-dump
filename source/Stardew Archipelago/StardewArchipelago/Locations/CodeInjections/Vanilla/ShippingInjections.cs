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
using StardewArchipelago.Constants.Modded;
using StardewArchipelago.Stardew.NameMapping;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla
{
    public class NightShippingBehaviors
    {
        public const string SHIPSANITY_PREFIX = "Shipsanity: ";

        private IMonitor _monitor;
        private ArchipelagoClient _archipelago;
        private LocationChecker _locationChecker;
        private NameSimplifier _nameSimplifier;

        public NightShippingBehaviors(IMonitor monitor, ArchipelagoClient archipelago, LocationChecker locationChecker, NameSimplifier nameSimplifier)
        {
            _monitor = monitor;
            _archipelago = archipelago;
            _locationChecker = locationChecker;
            _nameSimplifier = nameSimplifier;
        }

        // private static IEnumerator<int> _newDayAfterFade()
        public void CheckShipsanityLocationsBeforeSleep()
        {
            try
            {
                if (_archipelago.SlotData.Shipsanity == Shipsanity.None)
                {
                    return;
                }

                _monitor.Log($"Currently attempting to check shipsanity locations for the current day", LogLevel.Info);
                var allShippedItems = GetAllItemsShippedToday();
                _monitor.Log($"{allShippedItems.Count} items shipped", LogLevel.Info);
                CheckAllShipsanityLocations(allShippedItems);
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckShipsanityLocationsBeforeSleep)}:\n{ex}", LogLevel.Error);
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

        private void CheckAllShipsanityLocations(List<Item> allShippedItems)
        {
            foreach (var shippedItem in allShippedItems)
            {
                var name = _nameSimplifier.GetSimplifiedName(shippedItem);
                if (IgnoredModdedStrings.Shipments.Contains(name))
                {
                    continue;
                }
                var apLocation = $"{SHIPSANITY_PREFIX}{name}";
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
    }
}
