/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

namespace MoreTrees.Models
{
    /// <summary>Represents a product that a tree drops with a number of days between each production.</summary>
    public class TimedProduct
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The number of days inbetween the product dropping.</summary>
        public int DaysBetweenProduce { get; set; }

        /// <summary>The product that will drop.</summary>
        public string Product { get; set; }
    }
}
