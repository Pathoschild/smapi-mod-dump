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
using System.Diagnostics;
using System.Linq;
using System.Net;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Extensions;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace StardewArchipelago.GameModifications.EntranceRandomizer
{
    public class EntranceManager
    {
        private const string TRANSITIONAL_STRING = " to ";

        private readonly IMonitor _monitor;
        private Dictionary<string, string> _modifiedEntrances;

        private HashSet<string> _checkedEntrancesToday;
        private Dictionary<string, WarpRequest> generatedWarps;

        public EntranceManager(IMonitor monitor)
        {
            _monitor = monitor;
            generatedWarps = new Dictionary<string, WarpRequest>(StringComparer.OrdinalIgnoreCase);
        }

        public void ResetCheckedEntrancesToday(SlotData slotData)
        {
            _checkedEntrancesToday = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (slotData.EntranceRandomization == EntranceRandomization.Chaos)
            {
                ReshuffleEntrances(slotData);
            }
        }

        private void ReshuffleEntrances(SlotData slotData)
        {
            var random = new Random(int.Parse(slotData.Seed) + (int)Game1.stats.DaysPlayed);
            var numShuffles = _modifiedEntrances.Count * _modifiedEntrances.Count;
            for (var i = 0; i < numShuffles; i++)
            {
                var keys = _modifiedEntrances.Keys.ToArray();
                var chosenEntrance1 = keys[random.Next(keys.Length)];
                var chosenEntrance2 = keys[random.Next(keys.Length)];
                SwapTwoEntrances(chosenEntrance1, chosenEntrance2);
            }
        }

        public void SetEntranceRandomizerSettings(SlotData slotData)
        {
            _modifiedEntrances = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (slotData.EntranceRandomization == EntranceRandomization.Disabled)
            {
                return;
            }

            foreach (var (originalEntrance, replacementEntrance) in slotData.ModifiedEntrances)
            {
                RegisterRandomizedEntrance(originalEntrance, replacementEntrance);
            }

            CleanCoordinatesFromEntrancesWithoutSiblings();

            if (slotData.EntranceRandomization == EntranceRandomization.PelicanTown)
            {
                return;
            }

            SwapFarmhouseEntranceWithAnotherEmptyAreaEntrance(slotData);
        }

        private void CleanCoordinatesFromEntrancesWithoutSiblings()
        {
            foreach (var entranceKey in _modifiedEntrances.Keys.ToArray())
            {
                var entranceValue = _modifiedEntrances[entranceKey];
                if (TryCleanEntrance(entranceValue, _modifiedEntrances.Values, out var newValue))
                {
                    _modifiedEntrances[entranceKey] = newValue;
                    entranceValue = newValue;
                }
                if (TryCleanEntrance(entranceKey, _modifiedEntrances.Keys, out var newKey))
                {
                    _modifiedEntrances.Add(newKey, entranceValue);
                    _modifiedEntrances.Remove(entranceKey);
                }
            }
        }

        private bool TryCleanEntrance(string entrance, IEnumerable<string> otherEntrances, out string cleanEntrance)
        {
            var (location1, location2) = GetLocationNames(entrance);
            cleanEntrance = entrance;
            if (!location1.Contains("|") && !location2.Contains("|"))
            {
                return false;
            }

            var location1Name = location1.Split("|")[0];
            var location2Name = location2.Split("|")[0];
            var numberSiblings = otherEntrances.Count(x => x.StartsWith(location1Name) && x.Contains(location2Name));
            if (numberSiblings >= 2)
            {
                return false;
            }

            cleanEntrance = GetKey(location1Name, location2Name);
            return true;
        }

        private void SwapFarmhouseEntranceWithAnotherEmptyAreaEntrance(SlotData slotData)
        {
            var outsideAreas = new[] { "Town", "Mountain", "Farm", "Forest", "BusStop", "Desert", "Beach" };
            var random = new Random(int.Parse(slotData.Seed));
            var chosenEntrance = "";
            var replacementIsOutside = false;
            while (!replacementIsOutside)
            {
                chosenEntrance = _modifiedEntrances.Keys.ToArray()[random.Next(_modifiedEntrances.Keys.Count)];
                replacementIsOutside = outsideAreas.Contains(chosenEntrance.Split(" ")[0]) && !chosenEntrance.Contains("67|17"); // 67|17 is Quarry Mine
            }

            var farmToFarmhouse = "Farm to Farmhouse";
            var farmhouseToFarm = ReverseKey(farmToFarmhouse);
            _modifiedEntrances.Add(farmToFarmhouse, farmToFarmhouse);
            _modifiedEntrances.Add(farmhouseToFarm, farmhouseToFarm);
            SwapTwoEntrances(chosenEntrance, farmToFarmhouse);
        }

        private void SwapTwoEntrances(string entrance1, string entrance2)
        {
            // Trust me
            var destination1 = _modifiedEntrances[entrance1];
            var destination2 = _modifiedEntrances[entrance2];
            var reversed1 = ReverseKey(entrance1);
            var reversed2 = ReverseKey(entrance2);
            var reversedDestination1 = ReverseKey(destination1);
            var reversedDestination2 = ReverseKey(destination2);
            _modifiedEntrances[entrance1] = destination2;
            _modifiedEntrances[reversedDestination1] = reversed2;
            _modifiedEntrances[entrance2] = destination1;
            _modifiedEntrances[reversedDestination2] = reversed1;
        }

        private void RegisterRandomizedEntrance(string originalEntrance, string replacementEntrance)
        {
            var aliasedOriginal = TurnAliased(originalEntrance);
            var aliasedReplacement = TurnAliased(replacementEntrance);
            var originalEquivalentEntrances = _equivalentAreas.FirstOrDefault(x => x.Contains(aliasedOriginal)) ?? new[] { aliasedOriginal };
            var replacementEquivalentEntrances = _equivalentAreas.FirstOrDefault(x => x.Contains(aliasedReplacement)) ??
                                                 new[] { aliasedReplacement };
            foreach (var originalEquivalentEntrance in originalEquivalentEntrances)
            {
                foreach (var replacementEquivalentEntrance in replacementEquivalentEntrances)
                {
                    RegisterRandomizedEntranceWithCoordinates(originalEquivalentEntrance, replacementEquivalentEntrance);
                }
            }
        }

        private void RegisterRandomizedEntranceWithCoordinates(string originalEquivalentEntrance,
            string replacementEquivalentEntrance)
        {
            _modifiedEntrances.Add(originalEquivalentEntrance, replacementEquivalentEntrance);
        }

        public bool TryGetEntranceReplacement(string currentLocationName, string locationRequestName, Point targetPosition, out WarpRequest warpRequest)
        {
            warpRequest = null;
            var key = GetKeys(currentLocationName, locationRequestName, targetPosition);
            if (!TryGetModifiedWarpName(key, out var desiredWarpName))
            {
                return false;
            }

            if (_checkedEntrancesToday.Contains(desiredWarpName))
            {
                if (generatedWarps.ContainsKey(desiredWarpName))
                {
                    warpRequest = generatedWarps[desiredWarpName];
                    return true;
                }

                return false;
            }

            return TryFindWarpToDestination(desiredWarpName, out warpRequest);
        }

        private bool TryGetModifiedWarpName(string[] keys, out string desiredWarpName)
        {
            foreach (var key in keys)
            {
                if (_modifiedEntrances.ContainsKey(key))
                {
                    desiredWarpName = _modifiedEntrances[key];
                    return true;
                }
            }

            desiredWarpName = "";
            return false;
        }

        private bool TryFindWarpToDestination(string desiredWarpKey, out WarpRequest warpRequest)
        {
            var (locationOriginName, locationDestinationName) = GetLocationNames(desiredWarpKey);
            _checkedEntrancesToday.Add(desiredWarpKey);

            if (!locationOriginName.TryGetClosestWarpPointTo(ref locationDestinationName, out var locationOrigin, out var warpPoint))
            {
                warpRequest = null;
                return false;
            }

            var warpPointTarget = locationOrigin.GetWarpPointTarget(warpPoint, locationDestinationName);
            var locationDestination = Game1.getLocationFromName(locationDestinationName);
            var warpAwayPoint = locationDestination.GetClosestWarpPointTo(locationOriginName, warpPointTarget);
            var facingDirection = warpPointTarget.GetFacingAwayFrom(warpAwayPoint);
            var locationRequest = new LocationRequest(locationDestinationName, locationDestination.isStructure.Value,
                locationDestination);
            warpRequest = new WarpRequest(locationRequest, warpPointTarget.X, warpPointTarget.Y, facingDirection);
            generatedWarps[desiredWarpKey] = warpRequest;
            return true;
        }

        private static string[] GetKeys(string currentLocationName, string locationRequestName, Point targetPosition)
        {
            var currentPosition = Game1.player.getTileLocationPoint();
            return new[]
            {
                GetKey(currentLocationName, locationRequestName),
                GetKey(currentLocationName, locationRequestName, targetPosition),
                GetKey(currentLocationName, currentPosition, locationRequestName),
                GetKey(currentLocationName, currentPosition, locationRequestName, targetPosition),
            };
        }

        private static string GetKey(string currentLocationName, string locationRequestName)
        {
            var key = $"{currentLocationName}{TRANSITIONAL_STRING}{locationRequestName}";
            return key;
        }

        private static string GetKey(string currentLocationName, string locationRequestName, Point targetPosition)
        {
            var key = $"{currentLocationName}{TRANSITIONAL_STRING}{locationRequestName}|{targetPosition.X}|{targetPosition.Y}";
            return key;
        }

        private static string GetKey(string currentLocationName, Point currentPosition, string locationRequestName)
        {
            var key = $"{currentLocationName}|{currentPosition.X}|{currentPosition.Y}{TRANSITIONAL_STRING}{locationRequestName}";
            return key;
        }

        private static string GetKey(string currentLocationName, Point currentPosition, string locationRequestName, Point targetPosition)
        {
            var key = $"{currentLocationName}|{currentPosition.X}|{currentPosition.Y}{TRANSITIONAL_STRING}{locationRequestName}|{targetPosition.X}|{targetPosition.Y}";
            return key;
        }

        private static string ReverseKey(string key)
        {
            var parts = key.Split(TRANSITIONAL_STRING);
            return $"{parts[1]}{TRANSITIONAL_STRING}{parts[0]}";
        }

        private static (string, string) GetLocationNames(string key)
        {
            var split = key.Split(TRANSITIONAL_STRING);
            return (split[0], split[1]);
        }

        private static string TurnAliased(string key)
        {
            if (key.Contains(TRANSITIONAL_STRING))
            {
                var parts = key.Split(TRANSITIONAL_STRING);
                var aliased1 = TurnAliased(parts[0]);
                var aliased2 = TurnAliased(parts[1]);
                return $"{aliased1}{TRANSITIONAL_STRING}{aliased2}";
            }

            var modifiedString = key;
            foreach (var (oldString, newString) in _aliases)
            {
                var customizedNewString = newString;
                if (customizedNewString.Contains("{0}"))
                {
                    customizedNewString = string.Format(newString, Game1.player.isMale ? "Mens" : "Womens");
                }
                modifiedString = modifiedString.Replace(oldString, customizedNewString);
            }

            return modifiedString;
        }

        private static readonly Dictionary<string, string> _aliases = new()
        {
            { "Mayor's Manor", "ManorHouse" },
            { "Pierre's General Store", "SeedShop" },
            { "Clint's Blacksmith", "Blacksmith" },
            { "Alex", "Josh" },
            { "Tunnel Entrance", "Backwoods" },
            { "Marnie's Ranch", "AnimalShop" },
            { "Cottage", "House" },
            { "Tower", "House" },
            { "Carpenter Shop", "ScienceHouse|6|24" }, // LockedDoorWarp 6 24 ScienceHouse 900 2000S–
            { "Maru's Room", "ScienceHouse|3|8" }, // LockedDoorWarp 3 8 ScienceHouse 900 2000 Maru 500N
            { "Adventurer", "Adventure" },
            { "Willy's Fish Shop", "FishShop" },
            { "Museum", "ArchaeologyHouse" },
            { "Wizard Basement", "WizardHouseBasement"},
            { "The Mines", "Mine|18|13" }, // 54 4 Mine 18 13
            { "Quarry Mine Entrance", "Mine|67|17" },  // 103 15 Mine 67 17
            { "Quarry", "Mountain" },
            { "Shipwreck", "CaptainRoom" },
            { "Gourmand Cave", "IslandFarmcave" },
            { "Crystal Cave", "IslandWestCave1" },
            { "Boulder Cave", "IslandNorthCave1" },
            { "Skull Cavern Entrance", "SkullCave" },
            { "Oasis", "SandyHouse" },
            { "Casino", "Club" },
            { "Bathhouse Entrance", "BathHouse_Entry" },
            { "Locker Room", "BathHouse_{0}Locker" },
            { "Public Bath", "BathHouse_Pool" },
            { "Pirate Cove", "IslandSouthEastCave" },
            { "Leo Hut", "IslandHut" },
            { "Field Office", "IslandFieldOffice" },
            { "Island Farmhouse", "IslandFarmHouse" },
            { "Volcano", "VolcanoDungeon0" },
            { "Qi Walnut Room", "QiNutRoom" },
            { "Eugene's Garden", "Custom_EugeneNPC_EugeneHouse" },
            { "Eugene's Bedroom", "Custom_EugeneNPC_EugeneRoom" },
            { "Deep Woods House", "DeepWoodsMaxHouse" },
            { "Alec's Pet Shop", "Custom_AlecsPetShop" },
            { "Alec's Bedroom", "Custom_AlecsRoom" },
            { "Juna's Cave", "Custom_JunaNPC_JunaCave" },
            { "Jasper's Bedroom", "Custom_LK_Museum2"},
            { "'s", "" },
            { " ", "" },
        };

        private static string[] _jojaMartLocations = new[] { "JojaMart", "AbandonedJojaMart", "MovieTheater" };
        private static string[] _trailerLocations = new[] { "Trailer", "Trailer_Big" };
        private static string[] _beachLocations = new[] { "Beach", "BeachNightMarket" };

        private List<string[]> _equivalentAreas = new()
        {
            _jojaMartLocations,
            _trailerLocations,
            _beachLocations,
        };
    }

    public enum FacingDirection
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3,
    }
}