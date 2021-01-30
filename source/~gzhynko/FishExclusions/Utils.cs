/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;

namespace FishExclusions
{
    internal class Utils
    {
        public static int[] GetExcludedFish(ModConfig config, string seasonName, string locationName, bool raining)
        {
            // Initialize the list with common exclusions (since they are always excluded).
            var excludedFish = config.ItemsToExclude.CommonExclusions.ToList();

            foreach (var exclusion in config.ItemsToExclude.ConditionalExclusions)
            {
                var exclusionSeason = exclusion.Season.ToLower();
                var exclusionWeather = exclusion.Weather.ToLower();
                var exclusionLocation = exclusion.Location.ToLower();

                var exclusionWeatherBool = exclusionWeather == "rain";

                if (exclusionSeason != "any" && exclusionSeason != seasonName.ToLower()) continue;
                if (exclusionLocation != "any" && exclusionLocation != locationName.ToLower()) continue;
                if (exclusionWeather != "any" && exclusionWeatherBool != raining) continue;
                
                foreach (var itemId in exclusion.FishToExclude)
                {
                    if(!excludedFish.Contains(itemId))
                        excludedFish.Add(itemId);
                }
            }

            return excludedFish.ToArray<int>();
        }
    }
}
