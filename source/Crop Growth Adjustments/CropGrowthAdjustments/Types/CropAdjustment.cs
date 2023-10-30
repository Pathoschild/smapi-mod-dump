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
using Newtonsoft.Json;

namespace CropGrowthAdjustments.Types
{
    public class CropAdjustment
    {
        public string CropProduceName { get; set; }
        public string SeasonsToGrowIn { get; set; }
        public string SeasonsToProduceIn { get; set; }
        public string LocationsWithDefaultSeasonBehavior { get; set; }
        public List<SpecialSpriteForSeason> SpecialSpritesForSeasons { get; set; } = null;
        
        [JsonIgnore]
        public int CropProduceItemId { get; set; }
        
        [JsonIgnore]
        public int OriginalRowInSpriteSheet { get; set; }
        
        public List<string> GetSeasonsToGrowIn()
        {
            return SeasonsToGrowIn.Split(',').ToList().Select(e => e.Trim()).ToList();
        }
        
        public List<string> GetSeasonsToProduceIn()
        {
            return SeasonsToProduceIn.Split(',').ToList().Select(e => e.Trim()).ToList();
        }
        
        public List<string> GetLocationsWithDefaultSeasonBehavior()
        {
            if (LocationsWithDefaultSeasonBehavior != null)
                return LocationsWithDefaultSeasonBehavior.Split(',').ToList().Select(e => e.Trim()).ToList();

            return new List<string>();
        }
    }
}