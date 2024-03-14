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
using System.Collections.Generic;

namespace BetterMixedSeedsConfigUpdater.Models.V3
{
    /// <summary>A wrapper for a list of crops.</summary>
    public class Season_V3
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The list of crops that are present in this season.</summary>
        public List<Crop_V3> Crops { get; set; } = new List<Crop_V3>();


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="oldSeason">The <see cref="BetterMixedSeedsConfigUpdater.Models.V2.Season_V2"/> to recreate.</param>
        public Season_V3(Season_V2 oldSeason)
        {
            // validate
            if (oldSeason == null || oldSeason.Crops == null)
                return;

            // initialise
            foreach (var oldCrop in oldSeason.Crops)
                Crops.Add(new Crop_V3(oldCrop));
        }
    }
}
