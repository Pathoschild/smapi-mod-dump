/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace BetterMixedSeeds.Config
{
    /// <summary>A wrapper for a list of crops.</summary>
    public class Season
    {
        /// <summary>The list of crops that are present in this season.</summary>
        public List<Crop> Crops { get; set; }

        /// <summary>Construct an instance.</summary>
        /// <param name="crops">The list of crops that will be added to the season.</param>
        public Season(List<Crop> crops)
        {
            Crops = crops;
        }
    }
}
