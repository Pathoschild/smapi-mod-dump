/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using BetterMixedSeedsConfigUpdater.Models.V2;

namespace BetterMixedSeedsConfigUpdater.Models.V3
{
    /// <summary>Metadata about an integrated mod.</summary>
    public class CropMod_V3
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Whether the crops in the mod can be planted from mixed seeds.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>The season object that contains spring crops.</summary>
        public Season_V3 Spring { get; set; }

        /// <summary>The season object that contains summer crops.</summary>
        public Season_V3 Summer { get; set; }

        /// <summary>The season object that contains fall crops.</summary>
        public Season_V3 Fall { get; set; }

        /// <summary>The season object that contains winter crops.</summary>
        public Season_V3 Winter { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="oldCropMod">The <see cref="BetterMixedSeedsConfigUpdater.Models.V2.CropMod_V2"/> to recreate.</param>
        public CropMod_V3(CropMod_V2 oldCropMod)
        {
            // validate
            if (oldCropMod == null)
                return;

            // initialise
            Spring = new Season_V3(oldCropMod.Spring);
            Summer = new Season_V3(oldCropMod.Summer);
            Fall = new Season_V3(oldCropMod.Fall);
            Winter = new Season_V3(oldCropMod.Winter);
        }
    }
}
