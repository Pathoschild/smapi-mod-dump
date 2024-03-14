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
    /// <summary>Represents a product that a tree drops with a number of days between each production.</summary>
    public class TimedProduct
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The number of days inbetween the product dropping.</summary>
        public int DaysBetweenProduce { get; }

        /// <summary>The id of the product that will drop.</summary>
        public int ProductId { get; }

        /// <summary>The amount of product that will be produced.</summary>
        public int Amount { get; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="daysBetweenProduce">The number of days inbetween the product dropping.</param>
        /// <param name="productId">The id of the product that will drop.</param>
        /// <param name="amount">The amount of product that will be produced.</param>
        public TimedProduct(int daysBetweenProduce, int productId, int amount)
        {
            DaysBetweenProduce = daysBetweenProduce;
            ProductId = productId;
            Amount = amount;
        }
    }
}
