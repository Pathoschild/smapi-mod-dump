using System;
using DeepWoodsMod.API.Impl;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using static DeepWoodsMod.DeepWoodsGlobals;
using static DeepWoodsMod.DeepWoodsSettings;

namespace DeepWoodsMod
{
    public class EasterEggItem : StardewValley.Object
    {
        public const int PARENT_SHEET_INDEX = 1;

        public readonly int eggTileIndex;

        public EasterEggItem()
            : base()
        {
            this.displayName = I18N.EasterEggDisplayName;
            this.name = UNIQUE_NAME_FOR_EASTER_EGG_ITEMS;
            this.Category = StardewValley.Object.EggCategory;
            this.ParentSheetIndex = PARENT_SHEET_INDEX;
            this.eggTileIndex = 67;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("SMAPI.CommonErrors", "AvoidNetField")]
        public override int Stack
        {
            get
            {
                return Math.Max(0, this.stack.Value);
            }
            set
            {
                this.stack.Value = Math.Min(Math.Max(0, value), this.maximumStackSize());
            }
        }

        public override string DisplayName
        {
            get
            {
                return this.displayName;
            }
            set
            {
                this.displayName = value;
            }
        }

        public override string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("SMAPI.CommonErrors", "AvoidNetField")]
        public override int addToStack(int amount)
        {
            int newStackValue = this.stack.Value + amount;
            if (newStackValue > maximumStackSize())
            {
                this.stack.Value = maximumStackSize();
                return maximumStackSize() - newStackValue;
            }
            else
            {
                this.stack.Value = newStackValue;
            }
            return 0;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("SMAPI.CommonErrors", "AvoidNetField")]
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {
            if (drawShadow)
            {
                spriteBatch.Draw(Game1.shadowTexture, location + new Vector2(32f, 48f), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), color * 0.5f, 0.0f, new Vector2((float)Game1.shadowTexture.Bounds.Center.X, (float)Game1.shadowTexture.Bounds.Center.Y), 3f, SpriteEffects.None, layerDepth - 0.0001f);
            }
            spriteBatch.Draw(DeepWoodsTextures.Textures.Festivals, location + new Vector2(32 * scaleSize, 32 * scaleSize), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(DeepWoodsTextures.Textures.Festivals, this.eggTileIndex, 16, 16)), color * transparency, 0.0f, new Vector2(8f, 8f) * scaleSize, 4f * scaleSize, SpriteEffects.None, layerDepth);
            if (drawStackNumber && scaleSize > 0.3 && this.Stack > 1)
            {
                Utility.drawTinyDigits(this.Stack, spriteBatch, location + new Vector2(64 - Utility.getWidthOfTinyDigitString(this.stack.Value, 3 * scaleSize) + 3 * scaleSize, 64 - 18 * scaleSize + 2), 3 * scaleSize, 1, color);
            }
        }

        public override void drawWhenHeld(SpriteBatch b, Vector2 objectPosition, Farmer who)
        {
            Rectangle destinationRectangle = new Rectangle((int)objectPosition.X, (int)objectPosition.Y, 64, 64);
            Rectangle sourceRectangle = Game1.getSourceRectForStandardTileSheet(DeepWoodsTextures.Textures.Festivals, this.eggTileIndex, 16, 16);

            b.Draw(DeepWoodsTextures.Textures.Festivals, destinationRectangle, sourceRectangle, Color.White, 0, Vector2.Zero, SpriteEffects.None, Math.Max(0, (who.getStandingY() + 2) / 10000f));
        }

        public override string getDescription()
        {
            return "It's an Easter Egg ^_^";
        }

        public override Item getOne()
        {
            return new EasterEggItem();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("SMAPI.CommonErrors", "AvoidNetField")]
        public override int getStack()
        {
            return this.stack.Value;
        }

        public override bool isPlaceable()
        {
            return false;
        }

        public override int maximumStackSize()
        {
            return 999;
        }

        public override bool canStackWith(Item other)
        {
            return other is EasterEggItem;
        }

        public override int sellToStorePrice()
        {
            return 1000;
        }

    }
}
