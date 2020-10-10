/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

namespace BFAVToFAVRModConverter.Models
{
    /// <summary>Metadata abount an animal related to the animal shop.</summary>
    public class FavrAnimalShopInfo
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The description of the animal.</summary>
        public string Description { get; set; }

        /// <summary>The amount the animal costs.</summary>
        public int BuyPrice { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="description">The description of the animal.</param>
        /// <param name="buyPrice">The amount the animal costs.</param>
        public FavrAnimalShopInfo(string description, int buyPrice)
        {
            Description = description;
            BuyPrice = buyPrice;
        }
    }
}
