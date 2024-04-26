/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/stardew-mods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using FishExclusions.Types;

namespace FishExclusions
{
    internal class Utils
    {
        public static string[] GetExcludedFish(ModConfig config, string seasonName, string locationName, bool raining)
        {
            var excludedFish = new List<string>();

            // First add all of the common exclusions (since they are always excluded).
            foreach (var commonExclusion in config.ItemsToExclude.CommonExclusions)
            {
                var itemId = commonExclusion;
                    
                if(!excludedFish.Contains(itemId))
                    excludedFish.Add(itemId);
            }

            foreach (var exclusion in config.ItemsToExclude.ConditionalExclusions)
            {
                var exclusionWeather = exclusion.Weather.ToLower();

                if (exclusion.Season.ToLower() != seasonName.ToLower()) continue;
                if (exclusion.Location.ToLower() != locationName.ToLower()) continue;
                if ((exclusionWeather == "sunny" && raining) || (exclusionWeather == "rain" && !raining)) continue;
                
                foreach (var item in exclusion.Exclusions)
                {
                    var itemId = item;
                    
                    if(!excludedFish.Contains(itemId))
                        excludedFish.Add(itemId);
                }
            }

            return excludedFish.ToArray<string>();
        }
    }
}
