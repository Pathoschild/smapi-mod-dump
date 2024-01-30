/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/6135/StardewValley.ProfitCalculator
**
*************************************************/

using System.Collections.Generic;

namespace ProfitCalculator.main
{
    /// <summary>
    ///    Abstract class for parsing crop data from a file.
    /// </summary>
    public interface ICropParser
    {
        /// <summary>
        ///  Builds a dictionary of crops from a file or code.
        /// </summary>
        /// <returns> Dictionary of crops. </returns>
        public abstract Dictionary<string, Crop> BuildCrops();
    }
}