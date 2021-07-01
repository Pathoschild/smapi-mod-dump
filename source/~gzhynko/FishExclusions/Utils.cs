/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FishExclusions.Types;

namespace FishExclusions
{
    internal class Utils
    {
        public static int[] GetExcludedFish(ModConfig config, string seasonName, string locationName, bool raining)
        {
            var excludedFish = new List<int>();

            // First add all of the common exclusions (since they are always excluded).
            foreach (var commonExclusion in config.ItemsToExclude.CommonExclusions)
            {
                var itemId = GetIdForExcludedObject(commonExclusion);
                if(itemId == -1) continue;
                    
                if(!excludedFish.Contains(itemId))
                    excludedFish.Add(itemId);
            }

            foreach (var exclusion in config.ItemsToExclude.ConditionalExclusions)
            {
                var exclusionSeason = exclusion.Season.ToLower();
                var exclusionWeather = exclusion.Weather.ToLower();
                var exclusionLocation = exclusion.Location.ToLower();

                var exclusionWeatherBool = exclusionWeather == "rain";

                if (exclusionSeason != "any" && exclusionSeason != seasonName.ToLower()) continue;
                if (exclusionLocation != "any" && exclusionLocation != locationName.ToLower()) continue;
                if (exclusionWeather != "any" && exclusionWeatherBool != raining) continue;
                
                foreach (var item in exclusion.FishToExclude)
                {
                    var itemId = GetIdForExcludedObject(item);
                    if(itemId == -1) continue;
                    
                    if(!excludedFish.Contains(itemId))
                        excludedFish.Add(itemId);
                }
            }

            return excludedFish.ToArray<int>();
        }

        private static int GetIdForExcludedObject(object exclusion)
        {
            if (IsNumber(exclusion))
            {
                return Convert.ToInt16(exclusion);
            }
            
            if (ModEntry.JsonAssetsApi == null) return -1;
            return ModEntry.JsonAssetsApi.GetObjectId(exclusion.ToString());
        }

        private static bool IsNumber(object o)
        {
            return int.TryParse(o.ToString(), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out _);
        }
    }
}
