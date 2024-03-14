/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

namespace FarmAnimalVarietyRedux.Models.Converted
{
    /// <summary>Represents the shop information of an animal.</summary>
    public class AnimalShopInfo
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The description of the animal.</summary>
        public string Description { get; set; }

        /// <summary>The cost of the animal.</summary>
        public int BuyPrice { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="description">The description of the animal.</param>
        /// <param name="buyPrice">The cost of the animal.</param>
        public AnimalShopInfo(string description, int buyPrice)
        {
            Description = description;
            BuyPrice = buyPrice;
        }

        /// <inheritdoc/>
        public override string ToString() => $"BuyPrice: {BuyPrice}, Description: {Description}";
    }
}
