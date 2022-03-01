/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Types
{
    public enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter
    }

    public static class SeasonExtensions
    {
        public static string GetEffectDescriptionSuffix(this Season season)
        {
            return " in " + season.ToString();
        }

        /// <summary>
        /// Returns if the current location has the specified season
        /// </summary>        
        public static bool IsActive(this Season season)
        {
            var currentSeason = Game1.GetSeasonForLocation(Game1.currentLocation);
            return currentSeason.ToLower() == season.ToString().ToLower();
        }
    }
}
