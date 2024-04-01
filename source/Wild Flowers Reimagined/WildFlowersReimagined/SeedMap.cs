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

namespace WildFlowersReimagined
{
    sealed class SeedMap
    {
        /// <summary>
        /// Reformatted game data for faster lookups
        /// </summary>
        private readonly Dictionary<string, List<(string seedId, CropData cropData)>> mapData = new();
        private bool initialized = false;

        /// <summary>
        /// Creates a new seedMap with the game data already formatted
        /// </summary>
        public SeedMap() {}

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
                if (!mapData.TryGetValue(cropData.HarvestItemId, out var localList))
                {
                    localList = new();
                    mapData[cropData.HarvestItemId] = localList;
                }
                localList.Add((seedId, cropData));
            }

        }

        /// <summary>
        /// Gets the list of all the seeds that can grow in that location
        /// </summary>
        /// <param name="harvestId">Object Id to look for</param>
        /// <param name="location">Location to filter out the seeds</param>
        /// <returns>
        /// List with the following behavior
        /// - Empty if no seeds were found
        /// - All of the seeds for the Object Id, if the location ignores the season
        /// - Otherwise all the sees that have the current season in their crop data
        /// </returns>
        public List<string> GetSeedsForLocation(string harvestId, GameLocation location)
        {
            
            if (!mapData.TryGetValue(harvestId, out var seedList))
            {
                // No seeds found for that object
                return new();
            }
            if (location.SeedsIgnoreSeasonsHere())
            {
                // Season doesn't matter
                return seedList.Select(p => p.seedId).ToList();
            }
            // filter out data to only match season
            var localSeason = location.GetSeason();
            return seedList.Where(p => p.cropData.Seasons.Contains(localSeason)).Select(p=> p.seedId).ToList();
        }

    }
}
