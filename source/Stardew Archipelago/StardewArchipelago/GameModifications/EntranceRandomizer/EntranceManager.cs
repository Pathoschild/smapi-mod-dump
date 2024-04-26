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
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants.Modded;
using StardewArchipelago.Extensions;
using StardewArchipelago.Serialization;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.GameModifications.EntranceRandomizer
{
    public class EntranceManager
    {
        public const string TRANSITIONAL_STRING = " to ";
        private const string FARM_TO_FARMHOUSE = "Farm to FarmHouse";

        private readonly IMonitor _monitor;
        private readonly EquivalentWarps _equivalentAreas;
        private readonly ModEntranceManager _modEntranceManager;
        private readonly ArchipelagoStateDto _state;

        public Dictionary<string, string> ModifiedEntrances { get; private set; }
        private HashSet<string> _checkedEntrancesToday;
        private Dictionary<string, WarpRequest> generatedWarps;

        public EntranceManager(IMonitor monitor, ArchipelagoClient archipelago, ArchipelagoStateDto state)
        {
            _monitor = monitor;
            _equivalentAreas = new EquivalentWarps(archipelago);
            _modEntranceManager = new ModEntranceManager();
            _state = state;
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
            var seed = int.Parse(slotData.Seed) + (int)Game1.stats.DaysPlayed; // 998252633 on day 9
            var random = new Random(seed);
            var numShuffles = ModifiedEntrances.Count * ModifiedEntrances.Count;
            var newModifiedEntrances = ModifiedEntrances.ToDictionary(x => x.Key, x => x.Value);

            for (var i = 0; i < numShuffles; i++)
            {
                var keys = newModifiedEntrances.Keys.ToArray();
                var chosenIndex1 = random.Next(keys.Length);
                var chosenIndex2 = random.Next(keys.Length);
                var chosenEntrance1 = keys[chosenIndex1];
                var chosenEntrance2 = keys[chosenIndex2];
                SwapTwoEntrances(newModifiedEntrances, chosenEntrance1, chosenEntrance2);
            }

            ModifiedEntrances = new Dictionary<string, string>(newModifiedEntrances, StringComparer.OrdinalIgnoreCase);
        }

        public void SetEntranceRandomizerSettings(SlotData slotData)
        {
            ModifiedEntrances = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (slotData.EntranceRandomization == EntranceRandomization.Disabled)
            {
                return;
            }

            foreach (var (locationName, locationAlias)  in _modEntranceManager.GetModLocationAliases(slotData))
            {
                _locationAliases[locationName] = locationAlias;
            }

            foreach (var (originalEntrance, replacementEntrance) in slotData.ModifiedEntrances)
            {
                RegisterRandomizedEntrance(originalEntrance, replacementEntrance);
            }

            if (slotData.EntranceRandomization == EntranceRandomization.PelicanTown || slotData.EntranceRandomization == EntranceRandomization.NonProgression)
            {
                return;
            }

            AddFarmhouseToModifiedEntrances();

            if (slotData.EntranceRandomization == EntranceRandomization.Chaos)
            {
                return;
            }

            SwapFarmhouseEntranceWithAnotherEmptyAreaEntrance(slotData);
        }

        private void AddFarmhouseToModifiedEntrances()
        {
            var farmhouseToFarm = ReverseKey(FARM_TO_FARMHOUSE);
            ModifiedEntrances.Add(FARM_TO_FARMHOUSE, FARM_TO_FARMHOUSE);
            ModifiedEntrances.Add(farmhouseToFarm, farmhouseToFarm);
        }

        private void SwapFarmhouseEntranceWithAnotherEmptyAreaEntrance(SlotData slotData)
        {
            var outsideAreas = new List<string>() { "Town", "Mountain", "Farm", "Forest", "BusStop", "Desert", "Beach" };
            outsideAreas.AddRange(IncludeOutsideModEntrancesToOutsideAreas(slotData));
            var random = new Random(int.Parse(slotData.Seed));
            var chosenEntrance = "";
            var replacementIsOutside = false;
            
            while (!replacementIsOutside)
            {
                chosenEntrance = ModifiedEntrances.Keys.ToArray()[random.Next(ModifiedEntrances.Keys.Count)];
                var barredEntranceRule = !chosenEntrance.Contains("67|17")  &&  !chosenEntrance.Contains("SpriteSpring");
                replacementIsOutside = outsideAreas.Contains(chosenEntrance.Split(TRANSITIONAL_STRING)[0]) && barredEntranceRule; // 67|17 is Quarry Mine
            }

            SwapTwoEntrances(ModifiedEntrances, chosenEntrance, FARM_TO_FARMHOUSE);
        }

        private IEnumerable<string> IncludeOutsideModEntrancesToOutsideAreas(SlotData slotData)
        {
            if (slotData.Mods.HasMod(ModNames.SVE))
            {
                yield return "Custom_ForestWest"; 
                yield return "Custom_BlueMoonVineyard";
            }
            if (slotData.Mods.HasMod(ModNames.BOARDING_HOUSE))
            {
                yield return "Custom_BoardingHouse_BackwoodsPlateau";
            }
        }

        private static void SwapTwoEntrances(Dictionary<string, string> entrances, string entrance1, string entrance2)
        {
            // Trust me
            var destination1 = entrances[entrance1];
            var destination2 = entrances[entrance2];
            var reversed1 = ReverseKey(entrance1);
            var reversed2 = ReverseKey(entrance2);
            var reversedDestination1 = ReverseKey(destination1);
            var reversedDestination2 = ReverseKey(destination2);
            if (destination2 == reversed1 || destination1 == reversed2)
            {
                return;
            }
            entrances[entrance1] = destination2;
            entrances[reversedDestination1] = reversed2;
            entrances[entrance2] = destination1;
            entrances[reversedDestination2] = reversed1;
        }

        private void RegisterRandomizedEntrance(string originalEntrance, string replacementEntrance)
        {
            var aliasedOriginal = TurnAliased(originalEntrance);
            var aliasedReplacement = TurnAliased(replacementEntrance);
            RegisterRandomizedEntranceWithCoordinates(aliasedOriginal, aliasedReplacement);
        }

        private void RegisterRandomizedEntranceWithCoordinates(string originalEquivalentEntrance,
            string replacementEquivalentEntrance)
        {
            ModifiedEntrances.Add(originalEquivalentEntrance, replacementEquivalentEntrance);
        }

        public bool TryGetEntranceReplacement(string currentLocationName, string locationRequestName, Point targetPosition, out WarpRequest warpRequest)
        {
            warpRequest = null;
            var defaultCurrentLocationName = _equivalentAreas.GetDefaultEquivalentEntrance(currentLocationName);
            var defaultLocationRequestName = _equivalentAreas.GetDefaultEquivalentEntrance(locationRequestName);
            targetPosition = targetPosition.CheckSpecialVolcanoEdgeCaseWarp(defaultLocationRequestName);
            var key = GetKeys(defaultCurrentLocationName, defaultLocationRequestName, targetPosition);
            // return false;
            if (!TryGetModifiedWarpName(key, out var desiredWarpName))
            {
                _monitor.Log($"Tried to find warp from {currentLocationName} but found none.  Giving default warp.", LogLevel.Trace);
                return false;
            }
            
            var correctDesiredWarpName =_equivalentAreas.GetCorrectEquivalentEntrance(desiredWarpName);

            if (_checkedEntrancesToday.Contains(correctDesiredWarpName))
            {
                if (generatedWarps.ContainsKey(correctDesiredWarpName))
                {
                    warpRequest = generatedWarps[correctDesiredWarpName];
                    return true;
                }
                _monitor.Log($"Desired warp {correctDesiredWarpName} was checked, but not generated.", LogLevel.Trace);
                return false;
            }

            return TryFindWarpToDestination(correctDesiredWarpName, out warpRequest);
        }

        private bool TryGetModifiedWarpName(IEnumerable<string> keys, out string desiredWarpName)
        {
            foreach (var key in keys)
            {
                if (ModifiedEntrances.ContainsKey(key))
                {
                    desiredWarpName = ModifiedEntrances[key];
                    if (!_state.EntrancesTraversed.Contains(key))
                    {
                        _state.EntrancesTraversed.Add(key);
                    }
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

            if (!locationOriginName.TryGetClosestWarpPointTo(ref locationDestinationName, _equivalentAreas, out var locationOrigin, out var warpPoint))
            {
                _monitor.Log($"Could not find closest warp for {desiredWarpKey}, returning a null warpRequest.", LogLevel.Error);
                warpRequest = null;
                return false;
            }

            var warpPointTarget = locationOrigin.GetWarpPointTarget(warpPoint, locationDestinationName, _equivalentAreas);
            var locationDestination = Game1.getLocationFromName(locationDestinationName);
            var locationRequest = new LocationRequest(locationDestinationName, locationDestination.isStructure.Value, locationDestination);
            (locationRequest, warpPointTarget) = locationRequest.PerformLastLocationRequestChanges(locationOrigin, warpPoint, warpPointTarget);
            var warpAwayPoint = locationDestination.GetClosestWarpPointTo(locationOriginName, warpPointTarget);
            var facingDirection = warpPointTarget.GetFacingAwayFrom(warpAwayPoint);
            warpRequest = new WarpRequest(locationRequest, warpPointTarget.X, warpPointTarget.Y, facingDirection);
            generatedWarps[desiredWarpKey] = warpRequest;
            return true;
        }

        private static List<string> GetKeys(string currentLocationName, string locationRequestName,
            Point targetPosition)
        {
            var currentPosition = Game1.player.getTileLocationPoint();
            var currentPositions = new List<Point>();
            var targetPositions = new List<Point>();
            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    currentPositions.Add(new Point(currentPosition.X + x, currentPosition.Y + y));
                    targetPositions.Add(new Point(targetPosition.X + x, targetPosition.Y + y));
                }
            }

            var keys = new List<string>();
            keys.Add(GetKey(currentLocationName, locationRequestName));
            keys.AddRange(targetPositions.Select(targetPositionWithOffset => GetKey(currentLocationName, locationRequestName, targetPositionWithOffset)));
            keys.AddRange(currentPositions.Select(currentPositionWithOffset => GetKey(currentLocationName, currentPositionWithOffset, locationRequestName)));
            foreach (var currentPositionWithOffset in currentPositions)
            {
                keys.AddRange(targetPositions.Select(targetPositionWithOffset => GetKey(currentLocationName, currentPositionWithOffset, locationRequestName, targetPositionWithOffset)));
            }

            return keys;
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

        private string TurnAliased(string key)
        {
            if (key.Contains(TRANSITIONAL_STRING))
            {
                var parts = key.Split(TRANSITIONAL_STRING);
                var aliased1 = TurnAliased(parts[0]);
                var aliased2 = TurnAliased(parts[1]);
                var newEntrance = $"{aliased1}{TRANSITIONAL_STRING}{aliased2}";
                var newEntranceAliased = TurnAliased(newEntrance, _entranceAliases, false);
                return newEntranceAliased;
            }

            var modifiedString = TurnAliased(TurnAliased(key, _locationAliases, false), _locationsSingleWordAliases, true);
            //modifiedString = ModTurnAliased(key, modifiedString);
            return modifiedString;
        }

        private string TurnAliased(string key, Dictionary<string, string> aliases, bool singleWord)
        {
            var modifiedString = key;
            foreach (var oldString in aliases.Keys.OrderByDescending(x => x.Length))
            {
                var newString = aliases[oldString];
                var customizedNewString = newString;
                if (customizedNewString.Contains("{0}"))
                {
                    customizedNewString = string.Format(newString, Game1.player.isMale ? "Mens" : "Womens");
                }

                if (singleWord)
                {
                    modifiedString = modifiedString.Replace(oldString, customizedNewString);
                }
                else
                {
                    if (modifiedString.Contains(oldString) && !modifiedString.Equals(oldString))
                    {
                        // throw new ArgumentException($"This string is a partial replacement! {oldString}");
                    }

                    if (modifiedString.Equals(oldString))
                    {
                        modifiedString = customizedNewString;
                    }
                }
            }

            return modifiedString;
        }

        private static readonly Dictionary<string, string> _entranceAliases = new()
        {
            { "SebastianRoom to ScienceHouse|6|24", "SebastianRoom to ScienceHouse" }, // LockedDoorWarp 6 24 ScienceHouse 900 2000S–
            { "ScienceHouse|6|24 to SebastianRoom", "ScienceHouse to SebastianRoom" }, // LockedDoorWarp 6 24 ScienceHouse 900 2000S–
        };

        private Dictionary<string, string> _locationAliases = new()
        {
            { "Mayor's Manor", "ManorHouse" },
            { "Pierre's General Store", "SeedShop" },
            { "Clint's Blacksmith", "Blacksmith" },
            { "Alex's House", "JoshHouse" },
            { "Tunnel Entrance", "Backwoods" },
            { "Marnie's Ranch", "AnimalShop" },
            { "Leah's Cottage", "LeahHouse" },
            { "Wizard Tower", "WizardHouse" },
            { "Sewers", "Sewer" },
            { "Bus Tunnel", "Tunnel" },
            { "Carpenter Shop", "ScienceHouse|6|24" }, // LockedDoorWarp 6 24 ScienceHouse 900 2000S–
            { "Maru's Room", "ScienceHouse|3|8" }, // LockedDoorWarp 3 8 ScienceHouse 900 2000 Maru 500N
            { "Adventurer's Guild", "AdventureGuild" },
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
            { "Dig Site", "Island North" },
            { "Field Office", "IslandFieldOffice" },
            { "Island Farmhouse", "IslandFarmHouse" },
            { "Volcano Entrance", "VolcanoDungeon0|31|53" },
            { "Volcano River", "VolcanoDungeon0|6|49" },
            { "Secret Beach", "IslandNorth|12|31" },
            { "Professor Snail Cave", "IslandNorthCave1"},
            { "Qi Walnut Room", "QiNutRoom" },
            { "Mutant Bug Lair", "BugLand"},
        };

        private Dictionary<string, string> _locationsSingleWordAliases = new()
        {
            { "'s", "" },
            { " ", "" },
        };

        private static readonly Dictionary<string, Dictionary<string,string>> _modifiedAliases = new()
        {
            { "Stardew Valley Expanded", new(){{"WizardHouseBasement", "Custom_WizardBasement"}}
            }
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