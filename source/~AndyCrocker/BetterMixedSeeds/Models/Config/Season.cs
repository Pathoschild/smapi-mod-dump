/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace BetterMixedSeeds.Models.Config
{
    /// <summary>A wrapper for a list of crops.</summary>
    public class Season
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The list of crops that are present in this season.</summary>
        public List<Crop> Crops { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="crops">The list of crops that are present in this season.</param>
        public Season(List<Crop> crops)
        {
            Crops = crops;
        }
    }
}
