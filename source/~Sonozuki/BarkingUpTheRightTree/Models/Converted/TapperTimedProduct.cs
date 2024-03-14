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
    /// <summary>Represents a product that a tree drops (through tapping) with a number of days between each production.</summary>
    public class TapperTimedProduct : TimedProduct
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The number of days inbetween the product dropping.</summary>
        public new float DaysBetweenProduce { get; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="daysBetweenProduce">The number of days inbetween the product dropping.</param>
        /// <param name="product">The product that will drop.</param>
        /// <param name="amount">The amount of product that will be produced.</param>
        public TapperTimedProduct(float daysBetweenProduce, int product, int amount)
            : base(0, product, amount)
        {
            DaysBetweenProduce = daysBetweenProduce;
        }
    }
}
