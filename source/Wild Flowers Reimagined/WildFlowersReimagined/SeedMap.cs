/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jpparajeles/StardewValleyMods
**
*************************************************/

using Microsoft.CodeAnalysis;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.ItemTypeDefinitions;
using System.Dynamic;

namespace WildFlowersReimagined
{
    sealed class SeedMap
    {
        /// <summary>
        /// Reformatted game data for faster lookups
        /// </summary>
        private readonly Dictionary<string, (ItemMetadata flowerData, List<(string seedId, CropData cropData)> seeds)> mapData = new();
        /// <summary>
        /// Flag to mark the seed mnap as initialized
        /// </summary>
        private bool initialized = false;

        /// <summary>
        /// Creates a new seedMap with the game data already formatted
        /// </summary>
        public SeedMap() {}

        /// <summary>
        /// Initializes the seed map, this needs to be done after all the mods have loaded to ensure we have access to modded data
        /// </summary>
        /// <param name="force">Force the initialization of the map even if it's supposed to be full</param>
        public void Init(bool force=false)
        {
            if (initialized && !force)
            {
                return;
            }
            initialized = true;
            mapData.Clear();
            foreach (var (seedId, cropData) in Game1.cropData)
            {
                if (!mapData.TryGetValue(cropData.HarvestItemId, out var seedValues))
                {
                    var localList = new List<(string seedId, CropData cropData)>();
                    var itemInfo = ItemRegistry.GetMetadata(cropData.HarvestItemId);
                    
                    
                    if (itemInfo == null || !itemInfo.Exists()) {
                        // todo: get a logger in here
                        continue;
                    }
                    if (itemInfo.GetParsedData().Category != StardewValley.Object.flowersCategory)
                    {
                        continue;
                    }

                    seedValues = (itemInfo, localList);
                    mapData[cropData.HarvestItemId] = seedValues;
                }
                seedValues.seeds.Add((seedId, cropData));
            }

        }

        /// <summary>
        /// Returns a list of all known flowers
        /// </summary>
        /// <returns></returns>
        public Dictionary<ItemMetadata, List<ItemMetadata>> GetFlowerConfigMap()
        {
            return mapData.ToDictionary(p => p.Value.flowerData, 
                p => p.Value.seeds.Select(s => ItemRegistry.GetMetadata(s.seedId))
                    .Where(i => i.Exists())
                    .ToList()
                    );
        }

        /// <summary>
        /// Return a list of all flowers for this location
        /// </summary>
        /// <param name="location"> Location to filter down the list</param>
        /// <returns></returns>
        public List<string> GetSeedCandidatesForLocation(GameLocation location)
        {
            var candidates = new List<string>();
            if (location.SeedsIgnoreSeasonsHere())
            {
                candidates = mapData.SelectMany(p => p.Value.seeds.Select(s => s.seedId)).ToList();
            }
            else
            {
                var localSeason = location.GetSeason();
                candidates = mapData.SelectMany(p => p.Value.seeds.Where(p => p.cropData.Seasons.Contains(localSeason)).Select(p => p.seedId)).ToList();
            }
            return candidates;

        }

        /// <summary>
        /// Gets the seeds for this location scaled to the probability set
        /// </summary>
        /// <param name="location">Location to filter the seeds</param>
        /// <param name="flowerProbabilityMap">Map of the "probability" of each seed</param>
        /// <returns></returns>
        public List<string> GetSeedsForLocation(GameLocation location, Dictionary<string, int> flowerProbabilityMap) {
            var seeds = new List<string>();
            var candidates = GetSeedCandidatesForLocation(location);
            foreach (var seed in candidates)
            {
                for ( var i = 0; i < flowerProbabilityMap.GetValueOrDefault(seed, 3); i++)
                {
                    seeds.Add(seed);
                }
            }
            return seeds;
        }
    }
}
