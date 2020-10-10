/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace FarmAnimalVarietyRedux.Models
{
    /// <summary>Metadata abount an animal related to the animal shop.</summary>
    public class AnimalShopInfo
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The description of the animal.</summary>
        public string Description { get; set; }

        /// <summary>The amount the animal costs.</summary>
        public int BuyPrice { get; set; }

        /// <summary>The shop icon for the animal.</summary>
        public Texture2D ShopIcon { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="description">The description of the animal.</param>
        /// <param name="buyPrice">The amount the animal costs.</param>
        /// <param name="shopIcon">The shop icon for the animal.</param>
        public AnimalShopInfo(string description, int buyPrice, Texture2D shopIcon)
        {
            Description = description;
            BuyPrice = buyPrice;
            ShopIcon = shopIcon;
        }
    }
}
