/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;

namespace StardewArchipelago.GameModifications.EntranceRandomizer
{
    public static class Entrances
    {
        private const string TRANSITIONAL_STRING = " to ";
        private static readonly Dictionary<string, OneWayEntrance> _allEntrances = new();

        public static readonly (OneWayEntrance, OneWayEntrance) FarmHouseToFarm = AddEntrance("FarmHouse", "Farm", 3, 11, 64, 15, 0, 2);
        public static readonly (OneWayEntrance, OneWayEntrance) FarmHouseToCellar = AddEntrance("FarmHouse", "Cellar", 5, 24, 4, 2, 0, 2);
        
        public static readonly (OneWayEntrance, OneWayEntrance) FarmToGreenhouse = AddEntrance("Farm", "Greenhouse", 28, 16, 10, 23, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) FarmToFarmCave = AddEntrance("Farm", "FarmCave", 34, 6, 8, 11, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) FarmToBackwoods = AddEntrance("Farm", "Backwoods", 40, 0, 14, 39, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) FarmToBusStop = AddEntrance("Farm", "BusStop", 79, 17, 0, 23, 3, 1);
        public static readonly (OneWayEntrance, OneWayEntrance) FarmToForest = AddEntrance("Farm", "Forest", 41, 64, 68, 0, 0, 2);

        public static readonly (OneWayEntrance, OneWayEntrance) BusStopToBackwoods = AddEntrance("BusStop", "Backwoods", 0, 8, 49, 30, 1, 3);
        public static readonly (OneWayEntrance, OneWayEntrance) BusStopToTown = AddEntrance("BusStop", "Town", 34, 23, 0, 54, 3, 1);
        public static readonly (OneWayEntrance, OneWayEntrance) BackwoodsToTunnel = AddEntrance("Backwoods", "Tunnel", 23, 30, 39, 9, 1, 3);


        public static readonly (OneWayEntrance, OneWayEntrance) BackwoodsToMountain = AddEntrance("Backwoods", "Mountain", 48, 14, 0, 13, 1, 1);
        
        public static readonly (OneWayEntrance, OneWayEntrance) ForestToWoods = AddEntrance("Forest", "Woods", 0, 7, 58, 15, 1, 3);
        public static readonly (OneWayEntrance, OneWayEntrance) ForestToWizardHouse = AddEntrance("Forest", "WizardHouse", 5, 27, 8, 24, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) ForestToAnimalShop = AddEntrance("Forest", "AnimalShop", 90, 16, 13, 19, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) ForestToLeahHouse = AddEntrance("Forest", "LeahHouse", 104, 33, 7, 9, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) ForestToTown = AddEntrance("Forest", "Town", 117, 25, 0, 90, 3, 1);

        public static readonly (OneWayEntrance, OneWayEntrance) TownToCommunityCenter = AddEntrance("Town", "CommunityCenter", 52, 21, 32, 23, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) TownToHospital = AddEntrance("Town", "Hospital", 36, 56, 10, 19, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) TownToSeedShop = AddEntrance("Town", "SeedShop", 43, 57, 6, 29, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) TownToSaloon = AddEntrance("Town", "Saloon", 45, 71, 14, 24, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) TownToJoshHouse = AddEntrance("Town", "JoshHouse", 57, 64, 9, 24, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) TownToTrailer = AddEntrance("Town", "Trailer", 72, 69, 12, 9, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) TownToTrailerBig = AddEntrance("Town", "Trailer_Big", 72, 69, 13, 24, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) TownToHaleyHouse = AddEntrance("Town", "HaleyHouse", 20, 89, 2, 24, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) TownToSamHouse = AddEntrance("Town", "SamHouse", 10, 86, 4, 23, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) TownToSewer = AddEntrance("Town", "Sewer", 35, 97, 16, 11, 2, 2);
        public static readonly (OneWayEntrance, OneWayEntrance) TownToManorHouse = AddEntrance("Town", "ManorHouse", 58, 86, 4, 11, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) TownToBeach = AddEntrance("Town", "Beach", 54, 108, 38, 0, 0, 2);
        public static readonly (OneWayEntrance, OneWayEntrance) TownToArchaeologyHouse = AddEntrance("Town", "ArchaeologyHouse", 101, 90, 3, 14, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) TownToBlacksmith = AddEntrance("Town", "Blacksmith", 94, 82, 5, 19, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) TownToMountain = AddEntrance("Town", "Mountain", 81, 0, 15, 40, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) TownToJojaMart = AddEntrance("Town", "JojaMart", 95, 51, 14, 29, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) TownToAbandonedJojaMart = AddEntrance("Town", "AbandonedJojaMart", 96, 51, 9, 13, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) TownToMovieTheater = AddEntrance("Town", "MovieTheater", 95, 51, 13, 15, 2, 0);

        public static readonly (OneWayEntrance, OneWayEntrance) MountainToRailroad = AddEntrance("Mountain", "Railroad", 9, 0, 29, 59, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) MountainToTent = AddEntrance("Mountain", "Tent", 29, 7, 2, 5, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) MountainToScienceHouse = AddEntrance("Mountain", "ScienceHouse", 12, 26, 6, 24, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) MountainToAdventureGuild = AddEntrance("Mountain", "AdventureGuild", 76, 9, 6, 19, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) MountainToMine = AddEntrance("Mountain", "Mine", 54, 5, 18, 13, 2, 0);

        public static readonly (OneWayEntrance, OneWayEntrance) BeachToFishShop = AddEntrance("Beach", "FishShop", 30, 34, 5, 9, 2, 0);
        
        public static readonly (OneWayEntrance, OneWayEntrance) RailroadToBathHouse_Entry = AddEntrance("Railroad", "BathHouse_Entry", 10, 57, 5, 9, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) RailroadToWitchWarpCave = AddEntrance("Railroad", "WitchWarpCave", 54, 34, 4, 9, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) WitchWarpCaveToWitchSwamp = AddEntrance("WitchWarpCave", "WitchSwamp", 4, 5, 20, 42, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) WitchSwampToWitchHut = AddEntrance("WitchSwamp", "WitchHut", 20, 21, 7, 15, 2, 0);

        public static readonly (OneWayEntrance, OneWayEntrance) ScienceHouseToSebastianRoom = AddEntrance("ScienceHouse", "SebastianRoom", 12, 21, 1, 1, 0, 2);
        
        public static readonly (OneWayEntrance, OneWayEntrance) HospitalToHarveyRoom = AddEntrance("Hospital", "HarveyRoom", 10, 2, 6, 12, 2, 0);
        public static readonly (OneWayEntrance, OneWayEntrance) WizardHouseToWizardHouseBasement = AddEntrance("WizardHouse", "WizardHouseBasement", 4, 5, 4, 4, 0, 2);


        // Quarry Mine
        // public static readonly (OneWayEntrance, OneWayEntrance) MountainToMine = AddEntrance("Mountain", "Mine", 103, 17, 67, 17, 2, 0);


        public static bool TryGetEntrance(string key, out OneWayEntrance entrance)
        {
            var aliasedKey = TurnAliased(key);

            if (_allEntrances.TryGetValue(aliasedKey, out entrance))
            {
                return true;
            }

            if (_allEntrances.TryGetValue(aliasedKey.ToLower(), out entrance))
            {
                return true;
            }

            return _allEntrances.TryGetValue(aliasedKey.ToUpper(), out entrance);
        }

        public static bool TryGetEntrance(string location1, string location2, out OneWayEntrance entrance)
        {
            var aliasedlocation1 = TurnAliased(location1);
            var aliasedlocation2 = TurnAliased(location2);
            var key = $"{aliasedlocation1}{TRANSITIONAL_STRING}{aliasedlocation2}";
            return TryGetEntrance(key, out entrance);
        }

        public static void UpdateDynamicEntrances()
        {
            UpdateFarmCaveWarp();
            // UpdateFarmHouseWarp();
            // UpdateGreenhouseWarp();
        }

        private static void UpdateFarmCaveWarp()
        {
            var (farmToFarmCave, farmCavetoFarm) = FarmToFarmCave;
            UpdateDynamicWarp(farmToFarmCave, farmCavetoFarm, new Point(0, 1));
        }

        private static void UpdateFarmHouseWarp()
        {
            var (farmHouseToFarm, farmToFarmHouse) = FarmHouseToFarm;
            UpdateDynamicWarp(farmToFarmHouse, farmHouseToFarm, new Point(0, 0));
        }

        private static void UpdateGreenhouseWarp()
        {
            var (farmToGreenhouse, greenhouseToFarm) = FarmToGreenhouse;
            UpdateDynamicWarp(farmToGreenhouse, greenhouseToFarm, new Point(0, 0));
        }

        private static void UpdateDynamicWarp(OneWayEntrance entrance1, OneWayEntrance entrance2, Point offset)
        {
            var location1 = Game1.getLocationFromName(entrance1.OriginName);
            var location2 = Game1.getLocationFromName(entrance1.DestinationName);
            var warpPointOnFarm = location1.getWarpPointTo(entrance1.DestinationName) + offset;
            entrance1.OriginPosition = warpPointOnFarm;
            entrance2.DestinationPosition = warpPointOnFarm;
        }

        private static (OneWayEntrance, OneWayEntrance) AddEntrance(string location1Name, string location2Name, int location1X, int location1Y, int location2X, int location2Y, int facingDirection1, int facingDirection2)
        {
            return AddEntrance(location1Name, location2Name, new Point(location1X, location1Y), new Point(location2X, location2Y), facingDirection1, facingDirection2);
        }

        private static (OneWayEntrance, OneWayEntrance) AddEntrance(string location1Name, string location2Name, Point location1Position, Point location2Position, int facingDirection1, int facingDirection2)
        {
            var entrance1 = new OneWayEntrance(location1Name, location2Name, location1Position, location2Position, facingDirection2);
            var entrance2 = new OneWayEntrance(location2Name, location1Name, location2Position, location1Position, facingDirection1);
            var key1 = $"{location1Name}{TRANSITIONAL_STRING}{location2Name}";
            var key2 = $"{location2Name}{TRANSITIONAL_STRING}{location1Name}";

            AddDefaultAliases(key1, entrance1);
            AddDefaultAliases(key2, entrance2);

            return (entrance1, entrance2);
        }

        private static void AddDefaultAliases(string entranceName, OneWayEntrance entrance)
        {
            _allEntrances.Add(entranceName, entrance);
            _allEntrances.Add(entranceName.ToLower(), entrance);
            _allEntrances.Add(entranceName.ToUpper(), entrance);
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
                modifiedString = modifiedString.Replace(oldString, newString);
            }

            return modifiedString;
        }

        private static readonly Dictionary<string, string> _aliases = new()
        {
            {"Mayor's Manor", "ManorHouse"},
            {"Pierre's General Store", "SeedShop"},
            {"Clint's Blacksmith", "Blacksmith"},
            {"Alex", "Josh"},
            {"Tunnel Entrance", "Backwoods"},
            {"Marnie's Ranch", "AnimalShop"},
            {"Cottage", "House"},
            {"Tower", "House"},
            {"Carpenter Shop", "ScienceHouse"},
            {"Adventurer", "Adventure"},
            {"Willy's Fish Shop", "FishShop"},
            {"Museum", "ArchaeologyHouse"},
            {"The Mines", "Mine"},
            {"'s", ""},
            {" ", ""},
        };
    }
}
