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
        
        // public Crop(int seedIndex, int tileX, int tileY)
        public static bool CropConstructor_WildSeedsBecomesUnlockedCrop_Prefix(Crop __instance, ref int seedIndex, int tileX, int tileY)
        {
            try
            {
                if (seedIndex != 770)
                {
                    return true; // run original logic
                }

                var randomSeed = GetWeigthedRandomUnlockedCrop(Game1.currentSeason);
                seedIndex = randomSeed;
                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CropConstructor_WildSeedsBecomesUnlockedCrop_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static int GetWeigthedRandomUnlockedCrop(string season)
        {
            var receivedSeeds = _archipelago.GetAllReceivedItems().Select(x => x.ItemName).Where(x =>
                (x.EndsWith("Seeds") || x.EndsWith("Starter") || x.EndsWith("Seed") || x.EndsWith("Bean")) &&
                _stardewItemManager.ItemExists(x));
            var seedItems = receivedSeeds.Select(x => _stardewItemManager.GetItemByName(x).PrepareForGivingToFarmer());
            var location = Game1.currentLocation;
            var seedsInfo = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");

            var seedsICanPlantHere = seedItems.Where(x => SeedCanBePlantedHere(x, location, season, seedsInfo)).ToList();

            switch (season)
            {
                case "spring":
                    seedsICanPlantHere.Add(_stardewItemManager.GetItemByName("Spring Seeds").PrepareForGivingToFarmer());
                    break;
                case "summer":
                    seedsICanPlantHere.Add(_stardewItemManager.GetItemByName("Summer Seeds").PrepareForGivingToFarmer());
                    break;
                case "fall":
                    seedsICanPlantHere.Add(_stardewItemManager.GetItemByName("Fall Seeds").PrepareForGivingToFarmer());
                    break;
                case "winter":
                    seedsICanPlantHere.Add(_stardewItemManager.GetItemByName("Winter Seeds").PrepareForGivingToFarmer());
                    break;
            }

            var weightedSeeds = new List<int>();
            foreach (var seed in seedsICanPlantHere)
            {
                if (_overpoweredSeeds.Contains(seed.Name))
                {
                    weightedSeeds.Add(seed.ParentSheetIndex);
                }
                else if (SeedRegrows(seed, seedsInfo))
                {
                    weightedSeeds.AddRange(Enumerable.Repeat(seed.ParentSheetIndex, 10));
                }
                else
                {
                    weightedSeeds.AddRange(Enumerable.Repeat(seed.ParentSheetIndex, 100));
                }
            }

            var randomIndex = Game1.random.Next(weightedSeeds.Count);
            var randomSeed = weightedSeeds[randomIndex];
            return randomSeed;
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
