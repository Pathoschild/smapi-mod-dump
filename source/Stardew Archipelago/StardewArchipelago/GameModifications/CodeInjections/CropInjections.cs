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
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Events;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Quests;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    public class CropInjections
    {
        private const int SPRING_SEEDS = 495;
        private const int SUMMER_SEEDS = 496;
        private const int FALL_SEEDS = 497;
        private const int WINTER_SEEDS = 498;

        private static readonly string[] _overpoweredSeeds = { "Ancient Seeds", "Rare Seed" };

        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;
        private static StardewItemManager _stardewItemManager;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago, StardewItemManager stardewItemManager)
        {
            _monitor = monitor;
            _archipelago = archipelago;
            _stardewItemManager = stardewItemManager;
        }

        // public static int getRandomLowGradeCropForThisSeason(string season)
        public static bool GetRandomLowGradeCropForThisSeason_OnlyUnlockedCrops_Prefix(string season, ref int __result)
        {
            try
            {
                var receivedSeeds = _archipelago.GetAllReceivedItems().Select(x => x.ItemName).Where(x => (x.EndsWith("Seeds") || x.EndsWith("Starter") || x.EndsWith("Seed") || x.EndsWith("Bean")) && _stardewItemManager.ItemExists(x));
                var seedItems = receivedSeeds.Select(x => _stardewItemManager.GetItemByName(x).PrepareForGivingToFarmer());
                var location = Game1.currentLocation;
                var seedsInfo = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");

                var seedsICanPlantHere = seedItems.Where(x => SeedCanBePlantedHere(x, location, season, seedsInfo)).ToArray();

                if (!seedsICanPlantHere.Any())
                {
                    __result = season switch
                    {
                        "spring" => SPRING_SEEDS,
                        "summer" => SUMMER_SEEDS,
                        "fall" => FALL_SEEDS,
                        "winter" => WINTER_SEEDS,
                        _ => -1
                    };
                    return __result == -1; // run original logic only if I couldn't give good seeds
                }

                var weightedSeeds = new List<Item>();
                foreach (var seed in seedsICanPlantHere)
                {
                    if (_overpoweredSeeds.Contains(seed.Name))
                    {
                        weightedSeeds.Add(seed);
                    }
                    else if (SeedRegrows(seed, seedsInfo))
                    {
                        weightedSeeds.AddRange(Enumerable.Repeat(seed, 10));
                    }
                    else
                    {
                        weightedSeeds.AddRange(Enumerable.Repeat(seed, 100));
                    }
                }

                var randomIndex = Game1.random.Next(weightedSeeds.Count);
                var randomSeed = weightedSeeds[randomIndex];
                __result = randomSeed.ParentSheetIndex;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(GetRandomLowGradeCropForThisSeason_OnlyUnlockedCrops_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static bool SeedCanBePlantedHere(Item x, GameLocation location, string season, Dictionary<int, string> seedsInfo)
        {
            if (!seedsInfo.ContainsKey(x.ParentSheetIndex))
            {
                return false;
            }

            if (location.SeedsIgnoreSeasonsHere())
            {
                return true;
            }

            var seedSeasons = seedsInfo[x.ParentSheetIndex].Split('/')[1].Split(' ');
            return seedSeasons.Contains(season, StringComparer.CurrentCultureIgnoreCase);
        }

        private static bool SeedRegrows(Item x, Dictionary<int, string> seedsInfo)
        {
            if (!seedsInfo.ContainsKey(x.ParentSheetIndex))
            {
                return false;
            }

            return int.Parse(seedsInfo[x.ParentSheetIndex].Split('/')[4]) != -1;
        }
    }
}
