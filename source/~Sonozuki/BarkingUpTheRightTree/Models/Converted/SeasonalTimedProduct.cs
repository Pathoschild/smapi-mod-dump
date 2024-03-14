/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace BarkingUpTheRightTree.Models.Converted
{
    /// <summary>Represents a product that a tree drops within specified seasons.</summary>
    public class SeasonalTimedProduct : TimedProduct
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The seasons the product can drop from (<see langword="null"/> for all seasons).</summary>
        public string[] Seasons { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="daysBetweenProduce">The number of days inbetween the product dropping.</param>
        /// <param name="product">The product that will drop.</param>
        /// <param name="amount">The amount of product that will be produced.</param>
        /// <param name="seasons">The seasons the product can drop from (<see langword="null"/> for all seasons).</param>
        public SeasonalTimedProduct(int daysBetweenProduce, int product, int amount, string[] seasons)
            : base(daysBetweenProduce, product, amount)
        {
            if (seasons == null || seasons.Length == 0)
                seasons = new[] { "spring", "summer", "fall", "winter" };

            Seasons = seasons;
        }
    }
}
