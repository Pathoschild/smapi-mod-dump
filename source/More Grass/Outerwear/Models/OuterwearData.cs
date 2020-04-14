using Microsoft.Xna.Framework.Graphics;

namespace Outerwear.Models
{
    /// <summary>Metadata about an outerwear item.</summary>
    public class OuterwearData
    {
        /// <summary>The id of the outerwear.</summary>
        public int Id { get; set; }

        /// <summary>The display name of the outerwear.</summary>
        public string DisplayName { get; set; }

        /// <summary>The description of the outerwear.</summary>
        public string Description { get; set; }

        /// <summary>Where you can buy the outerwear (NPC name).</summary>
        public string BuyFrom { get; set; }

        /// <summary>The amount the outwear costs to buy.</summary>
        public int BuyPrice { get; set; }

        /// <summary>The icon to render when the item is in a menu.</summary>
        public Texture2D MenuIcon { get; set; }

        /// <summary>The sprite sheet of the outerwear on the farmer when it's equipped.</summary>
        public Texture2D EquippedTexture { get; set; }

        /// <summary>Construct an instance.</summary>
        /// <param name="id">The id of the outerwear.</param>
        /// <param name="displayName">The display name of the outerwear.</param>
        /// <param name="description">The description of the outerwear.</param>
        /// <param name="buyFrom">Where you can buy the outerwear (NPC name).</param>
        /// <param name="buyPrice">The amount the outwear costs to buy.</param>
        /// <param name="menuIcon">The icon to render when the item is in a menu.</param>
        /// <param name="equippedTexture">The sprite sheet of the outerwear on the farmer when it's equipped.</param>
        public OuterwearData(int id, string displayName, string description, string buyFrom, int buyPrice, Texture2D menuIcon, Texture2D equippedTexture)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            BuyFrom = buyFrom;
            BuyPrice = buyPrice;
            MenuIcon = menuIcon;
            EquippedTexture = equippedTexture;
        }
    }
}
