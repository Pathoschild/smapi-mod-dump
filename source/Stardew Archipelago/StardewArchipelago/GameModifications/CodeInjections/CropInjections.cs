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
using StardewValley.GameData.Crops;

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

        // public static string ResolveSeedId(string yieldItemId, GameLocation location)
        public static bool ResolveSeedId_WildSeedsBecomesUnlockedCrop_Prefix(string itemId, GameLocation location, ref string __result)
        {
            try
            {
                if (itemId != "770")
                {
                    return true; // run original logic
                }

                var randomSeed = GetWeigthedRandomUnlockedCrop(Game1.season);
                __result = randomSeed;
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ResolveSeedId_WildSeedsBecomesUnlockedCrop_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static string GetWeigthedRandomUnlockedCrop(Season season)
        {
            var receivedSeeds = _archipelago.GetAllReceivedItems().Select(x => x.ItemName).Where(x =>
                (x.EndsWith("Seeds") || x.EndsWith("Starter") || x.EndsWith("Seed") || x.EndsWith("Bean")) &&
                _stardewItemManager.ItemExists(x));
            var seedItems = receivedSeeds.Select(x => _stardewItemManager.GetItemByName(x).PrepareForGivingToFarmer());
            var location = Game1.currentLocation;
            var cropData = DataLoader.Crops(Game1.content);

            var seedsICanPlantHere = seedItems.Where(x => SeedCanBePlantedHere(x, location, season, cropData)).ToList();

            switch (season)
            {
                case Season.Spring:
                    seedsICanPlantHere.Add(_stardewItemManager.GetItemByName("Spring Seeds").PrepareForGivingToFarmer());
                    break;
                case Season.Summer:
                    seedsICanPlantHere.Add(_stardewItemManager.GetItemByName("Summer Seeds").PrepareForGivingToFarmer());
                    break;
                case Season.Fall:
                    seedsICanPlantHere.Add(_stardewItemManager.GetItemByName("Fall Seeds").PrepareForGivingToFarmer());
                    break;
                case Season.Winter:
                    seedsICanPlantHere.Add(_stardewItemManager.GetItemByName("Winter Seeds").PrepareForGivingToFarmer());
                    break;
            }

            var weightedSeeds = new List<string>();
            foreach (var seed in seedsICanPlantHere)
            {
                if (_overpoweredSeeds.Contains(seed.Name))
                {
                    weightedSeeds.Add(seed.ItemId);
                }
                else if (SeedRegrows(seed, cropData))
                {
                    weightedSeeds.AddRange(Enumerable.Repeat(seed.ItemId, 10));
                }
                else
                {
                    weightedSeeds.AddRange(Enumerable.Repeat(seed.ItemId, 100));
                }
            }

            var randomIndex = Game1.random.Next(weightedSeeds.Count);
            var randomSeed = weightedSeeds[randomIndex];
            return randomSeed;
        }

        private static bool SeedCanBePlantedHere(Item x, GameLocation location, Season season, Dictionary<string, CropData> cropData)
        {
            if (!cropData.ContainsKey(x.ItemId))
            {
                return false;
            }

            if (location.SeedsIgnoreSeasonsHere())
            {
                return true;
            }

            var seedSeasons = cropData[x.ItemId].Seasons;
            return seedSeasons.Contains(season);
        }

        private static bool SeedRegrows(Item x, Dictionary<string, CropData> cropData)
        {
            if (!cropData.ContainsKey(x.ItemId))
            {
                return false;
            }

            return cropData[x.ItemId].RegrowDays != -1;
        }
    }
}
