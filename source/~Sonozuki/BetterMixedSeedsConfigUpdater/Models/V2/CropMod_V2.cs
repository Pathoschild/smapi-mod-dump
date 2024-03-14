/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace BetterMixedSeedsConfigUpdater.Models.V2
{
    /// <summary>Metadata about an integrated mod.</summary>
    public class CropMod_V2
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The season object that contains spring crops.</summary>
        public Season_V2 Spring { get; set; }

        /// <summary>The season object that contains summer crops.</summary>
        public Season_V2 Summer { get; set; }

        /// <summary>The season object that contains fall crops.</summary>
        public Season_V2 Fall { get; set; }

        /// <summary>The season object that contains winter crops.</summary>
        public Season_V2 Winter { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="spring">The season object that contains spring crops.</param>
        /// <param name="summer">The season object that contains summer crops.</param>
        /// <param name="fall">The season object that contains fall crops.</param>
        /// <param name="winter">The season object that contains winter crops.</param>
        public CropMod_V2(Season_V2 spring, Season_V2 summer, Season_V2 fall, Season_V2 winter)
        {
            Spring = spring;
            Summer = summer;
            Fall = fall;
            Winter = winter;
        }
    }
}
