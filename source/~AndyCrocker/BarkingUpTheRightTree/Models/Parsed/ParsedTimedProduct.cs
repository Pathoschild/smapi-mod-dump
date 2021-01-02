/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

namespace BarkingUpTheRightTree.Models.Parsed
{
    /// <summary>Represents a product that a tree drops with a number of days between each production.</summary>
    /// <remarks>This is a version of <see cref="BarkingUpTheRightTree.Models.Converted.TimedProduct"/> that has <see cref="BarkingUpTheRightTree.Models.Converted.TimedProduct.Product"/> as <see langword="string"/>.<br/>The reason this is done is so content packs can have tokens in place of the ids to call mod APIs to get the id (so JsonAsset items can be used for example).</remarks>
    public class ParsedTimedProduct
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The number of days inbetween the product dropping.</summary>
        public int DaysBetweenProduce { get; set; }

        /// <summary>The product that will drop.</summary>
        public string Product { get; set; }

        /// <summary>The amount of product that will be produced.</summary>
        public int Amount { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="daysBetweenProduce">The number of days inbetween the product dropping.</param>
        /// <param name="product">The product that will drop.</param>
        /// <param name="amount">The amount of product that will be produced.</param>
        public ParsedTimedProduct(int daysBetweenProduce, string product, int amount)
        {
            DaysBetweenProduce = daysBetweenProduce;
            Product = product ?? "-1";
            Amount = amount;
        }
    }
}
