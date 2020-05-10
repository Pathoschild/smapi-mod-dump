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
    }
}
