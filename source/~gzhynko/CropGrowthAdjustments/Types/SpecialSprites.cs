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
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;

namespace CropGrowthAdjustments.Types
{
    public class SpecialSprites
    {
        public string Season { get; set; }
        public string Sprites { get; set; }
        public string LocationsToIgnore { get; set; }
        
        [JsonIgnore]
        public Texture2D SpritesTexture { get; set; }

        public List<string> GetLocationsToIgnore()
        {
            if (LocationsToIgnore != null)
                return LocationsToIgnore.Split(',').ToList().Select(e => e.Trim()).ToList();

            return new List<string>();
        }

        public Season GetSeason()
        {
            switch (Season.Trim().ToLower())
            {
                case "spring":
                    return StardewValley.Season.Spring;
                case "summer": 
                    return StardewValley.Season.Summer;
                case "fall": 
                    return StardewValley.Season.Fall;
                case "winter": 
                    return StardewValley.Season.Winter;
                default:
                    ModEntry.ModMonitor.Log($"Unknown season in SpecialSprites: {Season}", LogLevel.Warn);
                    return StardewValley.Season.Spring;
            }
        }
    }
}