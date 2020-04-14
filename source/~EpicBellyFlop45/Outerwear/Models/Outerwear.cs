using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Outerwear.Models
{
    /// <summary>Represents an in-game outerwear item.</summary>
    public class Outerwear : Item
    {
        /*********
        ** Fields
        *********/
        /// <summary>The metadata corresponding with the current outerwear item.</summary>
        private OuterwearData Data;


        /*********
        ** Accessors
        *********/
        /// <summary>The display name of the outerwear item.</summary>
        public override string DisplayName 
        {
            get
            {
                return Data.DisplayName;
            }
            set
            {
                Data.DisplayName = value;
            }
        }

        /// <summary>The max number of outerwear items the can be in an item stack.</summary>
        public override int Stack
        {
            get
            {
                return 1;
            }
            set { }
        }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="data">The metadata corresponding with the current outerwear item.</param>
        public Outerwear(OuterwearData data)
        {
            this.ParentSheetIndex = data.Id;
            Data = data;
        }

        /// <summary>Draw the item in the menm</summary>
        /// <param name="spriteBatch">The sprite batch to draw the menu sprite to.</param>
        /// <param name="location">The position to draw the sprite to.</param>
        /// <param name="scaleSize">The sprite scale.</param>
        /// <param name="transparency">The amount of transparency the sprite should have.</param>
        /// <param name="layerDepth">What layer to draw the sprite to.</param>
        /// <param name="drawStackNumber">The stack number text to draw.</param>
        /// <param name="color">The colour the sprite should be.</param>
        /// <param name="drawShadow">Whether the sprite should be drawn with a shadow.</param>
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            spriteBatch.Draw(
                texture: Data.MenuIcon, 
                position: location + new Vector2(32f, 32f) * scaleSize, 
                sourceRectangle: new Rectangle(0, 0, 16, 16), 
                color: color * transparency, 
                rotation: 0.0f, 
                origin: new Vector2(8f, 8f) * scaleSize, 
                scale: scaleSize * 4f, 
                effects: SpriteEffects.None, 
                layerDepth: layerDepth);
        }

        /// <summary>Get a new instance of the outerwear object.</summary>
        /// <returns>A new instance of the item.</returns>
        public override Item getOne()
        {
            var outerwearData = ModEntry.Api.GetOuterwearDataById(this.ParentSheetIndex);

            if (outerwearData == null)
                return null;

            return new Outerwear(outerwearData);
        }

        /// <summary>Try to add the passed item to the item stack.</summary>
        /// <param name="stack">The item to add to the stack.</param>
        /// <returns>The number of items that couldn't be added to the stack (This will always be one as the max stack for outerwear is only 1, meaning the item can't be added to the stack).</returns>
        public override int addToStack(Item stack) => 1;

        /// <summary>Get the item description.</summary>
        /// <returns>The item description.</returns>
        public override string getDescription() => Data.Description;

        /// <summary>Get the category name of the item.</summary>
        /// <returns>The category name of the item.</returns>
        public override string getCategoryName() => "Outerwear";

        /// <summary>Whether the item is placable in the world.</summary>
        /// <returns>Whether the player can place the item.</returns>
        public override bool isPlaceable() => false;

        /// <summary>The maximum number of this object can stack into the same inventory slot.</summary>
        /// <returns>The number of items that can stack.</returns>
        public override int maximumStackSize() => 1;

        /// <summary>The price of the item.</summary>
        /// <returns>The cost of the item in the shop menu.</returns>
        public override int salePrice() => Data.BuyPrice;

        /// <summary>Whether the player is allowed to purchase the item.</summary>
        /// <param name="who">The farmer to check who is able to buy the item.</param>
        /// <returns>Whether the item should be able to be sold to the farmer.</returns>
        public override bool CanBuyItem(Farmer who) => true;
    }
}
