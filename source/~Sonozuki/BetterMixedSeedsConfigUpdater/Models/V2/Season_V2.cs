/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace BetterMixedSeedsConfigUpdater.Models.V2
{
    /// <summary>A wrapper for a list of crops.</summary>
    public class Season_V2
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The list of crops that are present in this season.</summary>
        public List<Crop_V2> Crops { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="crops">The list of crops that will be added to the season.</param>
        public Season_V2(List<Crop_V2> crops)
        {
            Crops = crops;
        }
    }
}
