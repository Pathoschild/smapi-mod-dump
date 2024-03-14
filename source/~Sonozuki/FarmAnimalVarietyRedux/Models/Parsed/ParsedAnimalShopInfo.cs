/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using FarmAnimalVarietyRedux.Models.Converted;

namespace FarmAnimalVarietyRedux.Models.Parsed
{
    /// <summary>Represents the shop information of an animal.</summary>
    /// <remarks>This is a version of <see cref="CustomAnimal"/> that doesn't contain the shop icon that will be used for parsing content packs.</remarks>
    public class ParsedAnimalShopInfo
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
        public ParsedAnimalShopInfo() { }

        /// <summary>Constructs an instance.</summary>
        /// <param name="description">The description of the animal.</param>
        /// <param name="buyPrice">The cost of the animal.</param>
        public ParsedAnimalShopInfo(string description, int buyPrice)
        {
            Description = description;
            BuyPrice = buyPrice;
        }
    }
}
