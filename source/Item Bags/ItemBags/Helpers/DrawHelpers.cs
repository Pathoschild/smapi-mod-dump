/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using ItemBags.Bags;
using ItemBags.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace ItemBags.Helpers
{
    public static class DrawHelpers
    {
        /// <summary>The width of a single digit when rendered via <see cref="Utility.drawTinyDigits(int, SpriteBatch, Vector2, float, float, Color)"/> with a base scale of 1.0f</summary>
        public const int TinyDigitBaseWidth = 5;
        /// <summary>The height of a single digit when rendered via <see cref="Utility.drawTinyDigits(int, SpriteBatch, Vector2, float, float, Color)"/> with a base scale of 1.0f</summary>
        public const int TinyDigitBaseHeight = 7;

        /// <summary>Returns the width of a number drawn via <see cref="Utility.drawTinyDigits(int, SpriteBatch, Vector2, float, float, Color)"/></summary>
        public static float MeasureNumber(int Number, float Scale)
        {
            int NumDigits = GetNumDigits(Number);
            return TinyDigitBaseWidth * NumDigits * Scale;
        }

        public static int GetNumDigits(int Value)
        {
            if (Value == 0)
                return 1;
            else
                return (int)Math.Floor(Math.Log10(Value) + 1);
        }

        public static void DrawBox(SpriteBatch b, Rectangle Destination) { DrawBox(b, Destination.X, Destination.Y, Destination.Width, Destination.Height); }
        public static void DrawBox(SpriteBatch b, int X, int Y, int Width, int Height)
        {
            //  Fill in the box
            //b.Draw(Game1.menuTexture, new Rectangle(X + 12, Y + 16, Width - 12 - 16, Height - 12 - 16), new Rectangle(12, 272, 1, 32), Color.White); // non-smooth gradient but still looks decent
            b.Draw(Game1.menuTexture, new Rectangle(X + 12, Y + 16, Width - 12 - 16, Height - 12 - 16), new Rectangle(64, 128, 64, 64), Color.White); // smoother gradient tile
            //  Draw top-left corner
            b.Draw(Game1.menuTexture, new Vector2(X, Y), new Rectangle(0, 256, 16, 16), Color.White);
            //  Draw top center
            b.Draw(Game1.menuTexture, new Rectangle(X + 16, Y, Width - 16 * 2, 16), new Rectangle(16, 256, 16, 16), Color.White);
            //  Draw top-right corner
            b.Draw(Game1.menuTexture, new Vector2(X + Width - 16, Y), new Rectangle(44, 256, 16, 16), Color.White);
            //  Draw left center
            b.Draw(Game1.menuTexture, new Rectangle(X, Y + 16, 12, Height - 16 - 12), new Rectangle(0, 272, 12, 12), Color.White);
            //  Draw right center
            b.Draw(Game1.menuTexture, new Rectangle(X + Width - 16, Y + 16, 16, Height - 16 - 12), new Rectangle(44, 272, 16, 16), Color.White);
            //  Draw bottom-left corner
            b.Draw(Game1.menuTexture, new Vector2(X, Y + Height - 12), new Rectangle(0, 304, 12, 12), Color.White);
            //  Draw bottom center
            b.Draw(Game1.menuTexture, new Rectangle(X + 12, Y + Height - 12, Width - 12 * 2, 12), new Rectangle(12, 304, 12, 12), Color.White);
            //  Draw bottom-right corner
            b.Draw(Game1.menuTexture, new Vector2(X + Width - 12, Y + Height - 12), new Rectangle(48, 304, 12, 12), Color.White);
        }

        public static void DrawHorizontalSeparator(SpriteBatch b, Rectangle Position) { DrawHorizontalSeparator(b, Position.X, Position.Y, Position.Width, Position.Height); }
        /// <summary>Draws a horizontal separator (I.E. a horizontal line) using the same menu textures as the horizontal separator in the main GameMenu inventory tab</summary>
        public static void DrawHorizontalSeparator(SpriteBatch b, int X, int Y, int Width, int Height = 24)
        {
            int DefaultHeight = 24;
            int EdgeSize = (int)(32 * (Height * 1.0 / DefaultHeight));

            int XMargin = (int)(-4 * (Height * 1.0 / DefaultHeight));
            X += XMargin;
            Width -= 2 * XMargin;

            //  Draw the center portion
            b.Draw(Game1.menuTexture, new Rectangle(X + EdgeSize, Y, Width - EdgeSize * 2, Height), new Rectangle(44, 84, 1, 24), Color.White);
            //  Draw the left edge
            b.Draw(Game1.menuTexture, new Rectangle(X, Y, EdgeSize, Height), new Rectangle(12, 84, 32, 24), Color.White);
            //  Draw the right edge
            b.Draw(Game1.menuTexture, new Rectangle(X + Width - EdgeSize, Y, EdgeSize, Height), new Rectangle(212, 84, 32, 24), Color.White);
        }

        public static void DrawBorder(SpriteBatch b, Rectangle Destination, int Thickness, Color Color)
        {
            Texture2D TextureColor = TextureHelpers.GetSolidColorTexture(Game1.graphics.GraphicsDevice, Color);
            b.Draw(TextureColor, new Rectangle(Destination.X, Destination.Y, Thickness, Destination.Height), Color.White); // Left border
            b.Draw(TextureColor, new Rectangle(Destination.Right - Thickness, Destination.Y, Thickness, Destination.Height), Color.White); // Right border
            b.Draw(TextureColor, new Rectangle(Destination.X + Thickness, Destination.Y, Destination.Width - Thickness * 2, Thickness), Color.White); // Top border
            b.Draw(TextureColor, new Rectangle(Destination.X + Thickness, Destination.Bottom - Thickness, Destination.Width - Thickness * 2, Thickness), Color.White); // Bottom border
        }

        public static void DrawStringWithShadow(SpriteBatch b, SpriteFont Font, string Text, float XPosition, float YPosition, Color MainColor, Color ShadowColor, int ShadowXOffset, int ShadowYOffset)
        {
            Vector2 Position = new Vector2(XPosition, YPosition);
            b.DrawString(Font, Text, Position + new Vector2(ShadowXOffset, ShadowYOffset), ShadowColor);
            b.DrawString(Font, Text, Position, MainColor);
        }

        /// <param name="DrawStack">True if <see cref="Item.Stack"/> should be drawn</param>
        /// <param name="DrawQuality">True if <see cref="Object.Quality"/> should be drawn</param>
        /// <param name="IconScale">Does not affect the scaling for the quality, quantity, or other special rendered things like the capacity bar for a watering can</param>
        public static void DrawItem(SpriteBatch b, Rectangle Destination, Item Item, bool DrawStack, bool DrawQuality, float IconScale, float Transparency, Color Overlay, Color StackColor)
        {
            if (Destination.Width != Destination.Height)
                throw new InvalidOperationException("Can only call DrawItem on a perfect square Destination");

            //  Normally I'd just use Item.drawInMenu but that method doesn't scale stack size, qualities, or shadows correctly and thus will only display correctly at the default size of 64x64.
            //  Some items also render several textures to the destination point, and use hardcoded offsets for the sprites that also don't respect desired scaling. (Such as wallpapers or watering cans)
            //  So this method is my attempt at re-creating the Item.drawInMenu implementations in a way that will correctly scale to the desired Destination rectangle.
            //  It doesn't handle all cases correctly, but whatever, it's close enough

            DrawStack = DrawStack && (Item.maximumStackSize() > 1 || Item.Stack > 1);
            DrawQuality = DrawQuality && Item is Object Obj && Enum.IsDefined(typeof(ObjectQuality), Obj.Quality);

            int ScaledIconSize = (int)(Destination.Width * IconScale * 0.8f);
            Rectangle ScaledIconDestination = new Rectangle(Destination.Center.X - ScaledIconSize / 2, Destination.Center.Y - ScaledIconSize / 2, ScaledIconSize, ScaledIconSize);

            Texture2D SourceTexture = null;
            Rectangle? SourceTextureRectangle = null;
            if (Item is ItemBag Bag)
            {
                float BaseScale = Destination.Width / (float)BagInventoryMenu.DefaultInventoryIconSize;
                Vector2 OffsetDueToScaling = new Vector2((BaseScale - 1.0f) * BagInventoryMenu.DefaultInventoryIconSize * 0.5f); // I honestly forget why I needed this lmao. Maybe ItemBag.drawInMenu override was scaling around a different origin or some shit
                Bag.drawInMenu(b, new Vector2(Destination.X, Destination.Y) + OffsetDueToScaling, IconScale * Destination.Width / 64f * 0.8f, Transparency, 1.0f, StackDrawType.Hide, Overlay, false);
            }
            else if (Item is Tool Tool)
            {
                if (Item is MeleeWeapon Weapon)
                {
                    SourceTexture = MeleeWeapon.weaponsTexture;
                    SourceTextureRectangle = new Rectangle?(Game1.getSquareSourceRectForNonStandardTileSheet(SourceTexture, 16, 16, Tool.IndexOfMenuItemView));
                }
                else
                {
                    SourceTexture = Game1.toolSpriteSheet;
                    SourceTextureRectangle = new Rectangle?(Game1.getSquareSourceRectForNonStandardTileSheet(SourceTexture, 16, 16, Tool.IndexOfMenuItemView));
                }
            }
            else if (Item is Ring Ring)
            {
                SourceTexture = Game1.objectSpriteSheet;
                SourceTextureRectangle = new Rectangle?(Game1.getSourceRectForStandardTileSheet(SourceTexture, Ring.indexInTileSheet, 16, 16));
            }
            else if (Item is Hat Hat)
            {
                SourceTexture = FarmerRenderer.hatsTexture;
                SourceTextureRectangle = new Rectangle?(new Rectangle(Hat.which * 20 % SourceTexture.Width, Hat.which * 20 / SourceTexture.Width * 20 * 4, 20, 20));
            }
            else if (Item is Boots Boots)
            {
                SourceTexture = Game1.objectSpriteSheet;
                SourceTextureRectangle = new Rectangle?(Game1.getSourceRectForStandardTileSheet(SourceTexture, Boots.indexInTileSheet.Value, 16, 16));
            }
            else if (Item is Clothing Clothing)
            {
                Color clothes_color = Clothing.clothesColor;
                if (Clothing.isPrismatic.Value)
                {
                    clothes_color = Utility.GetPrismaticColor(0);
                }
                if (Clothing.clothesType.Value != 0)
                {
                    if (Clothing.clothesType.Value == 1)
                    {
                        b.Draw(FarmerRenderer.pantsTexture, Destination, 
                            new Rectangle?(new Rectangle(192 * (Clothing.indexInTileSheetMale.Value % (FarmerRenderer.pantsTexture.Width / 192)), 688 * (Clothing.indexInTileSheetMale.Value / (FarmerRenderer.pantsTexture.Width / 192)) + 672, 16, 16)), 
                            Utility.MultiplyColor(clothes_color, Color.White) * Transparency, 0f, Vector2.Zero, SpriteEffects.None, 1f);
                        //b.Draw(FarmerRenderer.pantsTexture, location + new Vector2(32f, 32f), 
                            //new Rectangle?(new Rectangle(192 * (this.indexInTileSheetMale.Value % (FarmerRenderer.pantsTexture.Width / 192)), 688 * (this.indexInTileSheetMale.Value / (FarmerRenderer.pantsTexture.Width / 192)) + 672, 16, 16)), 
                            //Utility.MultiplyColor(clothes_color, color) * transparency, 0f, new Vector2(8f, 8f), scaleSize * 4f, SpriteEffects.None, layerDepth);
                    }
                }
                else
                {
                    b.Draw(FarmerRenderer.shirtsTexture, Destination, 
                        new Rectangle?(new Rectangle(Clothing.indexInTileSheetMale.Value * 8 % 128, Clothing.indexInTileSheetMale.Value * 8 / 128 * 32, 8, 8)), 
                        Color.White * Transparency, 0f, Vector2.Zero, SpriteEffects.None, 1f);
                    b.Draw(FarmerRenderer.shirtsTexture, Destination, 
                        new Rectangle?(new Rectangle(Clothing.indexInTileSheetMale.Value * 8 % 128 + 128, Clothing.indexInTileSheetMale.Value * 8 / 128 * 32, 8, 8)), 
                        Utility.MultiplyColor(clothes_color, Color.White) * Transparency, 0f, Vector2.Zero, SpriteEffects.None, 1f);
                    //b.Draw(FarmerRenderer.shirtsTexture, location + new Vector2(32f, 32f), new Rectangle?(new Rectangle(this.indexInTileSheetMale.Value * 8 % 128, this.indexInTileSheetMale.Value * 8 / 128 * 32, 8, 8)), color * transparency, 0f, new Vector2(4f, 4f), scaleSize * 4f, SpriteEffects.None, layerDepth);
                    //b.Draw(FarmerRenderer.shirtsTexture, location + new Vector2(32f, 32f), new Rectangle?(new Rectangle(this.indexInTileSheetMale.Value * 8 % 128 + 128, this.indexInTileSheetMale.Value * 8 / 128 * 32, 8, 8)), Utility.MultiplyColor(clothes_color, color) * transparency, 0f, new Vector2(4f, 4f), scaleSize * 4f, SpriteEffects.None, layerDepth + dye_portion_layer_offset);
                }
            }
            else if (Item is Fence Fence)
            {
                int DrawSum = Fence.getDrawSum(Game1.currentLocation);
                int SourceRectPosition = Fence.fenceDrawGuide[DrawSum];
                SourceTexture = Fence.fenceTexture.Value;
                if (Fence.isGate)
                {
                    if (DrawSum == 110)
                    {
                        SourceTextureRectangle = new Rectangle?(new Rectangle(0, 512, 88, 24));
                    }
                    else if (DrawSum == 1500)
                    {
                        SourceTextureRectangle = new Rectangle?(new Rectangle(112, 512, 16, 64));
                    }
                }
                SourceTextureRectangle = new Rectangle?(Game1.getArbitrarySourceRect(SourceTexture, 64, 128, SourceRectPosition));
            }
            else if (Item is Furniture Furniture)
            {
                SourceTexture = Furniture.furnitureTexture;
                SourceTextureRectangle = Furniture.defaultSourceRect;
            }
            else if (Item is Wallpaper WP)
            {
                SourceTexture = null; // Wallpaper.wallpaperTexture; //Stardew Valley beta 1.5.5 no longer has a Wallpaper.wallpaperTexture field. No idea where it is now
                SourceTextureRectangle = WP.sourceRect;
            }
            else
            {
                if (Item is Object BigCraftable && BigCraftable.bigCraftable.Value)
                {
                    SourceTexture = Game1.bigCraftableSpriteSheet;
                    SourceTextureRectangle = Object.getSourceRectForBigCraftable(Item.ParentSheetIndex);
                    //ScaledIconDestination = new Rectangle(ScaledIconDestination.X + ScaledIconDestination.Width / 8, ScaledIconDestination.Y - ScaledIconDestination.Height / 8, 
                    //    ScaledIconDestination.Width - ScaledIconDestination.Width / 4, ScaledIconDestination.Height + ScaledIconDestination.Height / 4);
                    ScaledIconDestination = new Rectangle(ScaledIconDestination.X + ScaledIconDestination.Width / 4, ScaledIconDestination.Y,
                        ScaledIconDestination.Width - ScaledIconDestination.Width / 2, ScaledIconDestination.Height);
                    //From decompiled .exe code:
                    //spriteBatch.Draw(Game1.bigCraftableSpriteSheet, location + new Vector2(32f, 32f), new Rectangle?(sourceRect), color * transparency, 
                    //    0f, new Vector2(8f, 16f), 4f * ((double)scaleSize < 0.2 ? scaleSize : scaleSize / 2f), SpriteEffects.None, layerDepth);
                }
                else
                {
                    SourceTexture = Game1.objectSpriteSheet;
                    SourceTextureRectangle = new Rectangle?(Game1.getSourceRectForStandardTileSheet(SourceTexture, Item.parentSheetIndex, 16, 16));
                }
            }

            //  Draw the sprite
            if (SourceTexture != null)
                b.Draw(SourceTexture, ScaledIconDestination, SourceTextureRectangle, Overlay * Transparency);

            //  Draw Quality
            if (DrawQuality && Item is Object ItemObject && Enum.IsDefined(typeof(ObjectQuality), ItemObject.Quality))
            {
                int QualityIconSize = Math.Max(8, Destination.Width / 4);
                Rectangle QualityDestination = new Rectangle(Destination.X + Destination.Width / 16, Destination.Y + Destination.Height - QualityIconSize - Destination.Height / 16, QualityIconSize, QualityIconSize);
                Rectangle QualitySource = ItemBag.QualityIconTexturePositions[(ObjectQuality)ItemObject.Quality];
                b.Draw(Game1.mouseCursors, QualityDestination, QualitySource, Color.White);
            }

            //  Draw Quantity
            if (DrawStack)
            {
                float QuantityScale = 2.7f * Destination.Width / (float)BagInventoryMenu.DefaultInventoryIconSize;
                int NumDigits = GetNumDigits(Item.Stack);
                if (NumDigits > 3)
                {
                    QuantityScale -= (NumDigits - 3) * 0.3f;
                }
                Vector2 TopLeftPosition = new Vector2(
                    Destination.Right - MeasureNumber(Item.Stack, QuantityScale),
                    Destination.Bottom - TinyDigitBaseHeight * QuantityScale);
                Utility.drawTinyDigits(Item.Stack, b, TopLeftPosition, QuantityScale, 0f, StackColor);
            }

            //  Apparently the game's 'drawInMenu' doesn't respect the 'scaleSize' parameter so rendering to a smaller region than 64x64 doesn't actually work for most items.
            //  It's especially messed up for items with unique draw logic such as a watering can which needs to draw both the watering can's icon and it's capacity bar underneath the icon.
            //  So to fix this I was trying to render the 'drawInMenu' to a texture, then render that texture to the backbuffer with the desired scaling, but I haven't gotten it to work yet
            /*RenderTargetBinding[] PreviousRenderTargets = Game1.graphics.GraphicsDevice.GetRenderTargets();
            Game1.graphics.GraphicsDevice.SetRenderTarget(ItemRenderTarget);
            CurrentItem.drawInMenu(b, new Vector2(32, 32), Scale, Transparency, 1.0f, StackDrawType.Draw, Color.White, true);
            Game1.graphics.GraphicsDevice.SetRenderTargets(PreviousRenderTargets);
            b.Draw(ItemRenderTarget, Destination, Color.White);*/
        }

        public static void DrawToolTipInfo(SpriteBatch b, Rectangle AnchorPoint, Item Item, bool DrawName, bool DrawCategory, bool DrawDescription, bool DrawRecovery, bool DrawSingleItemValue, bool DrawStackValue, 
                int? MaximumCapacity = null, bool AlwaysShowCapacity = false, Color? CapacityColorOverride = null)
        {
            Object Obj = Item as Object;
            bool IsEdible = Obj != null && Obj.Edibility != -300;
            DrawRecovery = DrawRecovery && IsEdible;
            DrawStackValue = DrawStackValue && Obj != null && Obj.Stack > 1;
            DrawSingleItemValue = DrawSingleItemValue && Obj != null && !DrawStackValue;
            int SalePrice = ItemBag.GetSingleItemPrice(Item);

            int Margin = 16;
            int SeparatorHeight = 6;
            int SeparatorMargin = 3;
            int RecoveryIconSize = 24;
            float NameScale = 0.8f;
            float DescriptionScale = 0.6f;
            float OtherTextScale = 0.6f;
            float ValueTextScale = 2.7f;
            SpriteFont Font = Game1.dialogueFont;
            SpriteFont RecoveryAndBuffFont = Game1.smallFont;
            SpriteFont ValueFont = Game1.tinyFont;

            Texture2D MoneySpriteSheet = TextureHelpers.EmojiSpritesheet;
            Rectangle MoneyIconSourceRect = new Rectangle(117, 18, 9, 9);
            float MoneyIconScale = 2.0f;

            int PreviousLineSpacing = Font.LineSpacing;
            try
            {
                Font.LineSpacing = (int)(Font.LineSpacing * 0.9);

                //  Compute size of tooltip contents
                Vector2 RequiredSize = new Vector2();
                if (DrawName || DrawCategory)
                {
                    if (DrawName)
                    {
                        Vector2 NameSize = Font.MeasureString(Item.DisplayName) * NameScale;
                        RequiredSize.X = Math.Max(RequiredSize.X, NameSize.X);
                        RequiredSize.Y += NameSize.Y;
                    }
                    if (DrawCategory)
                    {
                        Vector2 CategorySize = Font.MeasureString(Item.getCategoryName()) * OtherTextScale;
                        RequiredSize.X = Math.Max(RequiredSize.X, CategorySize.X);
                        RequiredSize.Y += CategorySize.Y;
                    }
                    RequiredSize.Y += SeparatorHeight + SeparatorMargin * 2;
                }

                if (DrawDescription)
                {
                    Vector2 DescriptionSize = Font.MeasureString(Item.getDescription()) * DescriptionScale;
                    RequiredSize.X = Math.Max(RequiredSize.X, DescriptionSize.X);
                    RequiredSize.Y += DescriptionSize.Y;
                    RequiredSize.Y += SeparatorHeight + SeparatorMargin * 2;
                }

                if (DrawRecovery)
                {
                    RequiredSize.Y += 5;

                    int StaminaRecovery = Obj.staminaRecoveredOnConsumption();
                    if (StaminaRecovery >= 0)
                    {
                        RequiredSize.Y += RecoveryIconSize;
                        int HealthRecovery = Obj.healthRecoveredOnConsumption();
                        if (HealthRecovery > 0)
                        {
                            RequiredSize.Y += RecoveryIconSize;
                        }
                    }
                    else if (StaminaRecovery != -300)
                    {
                        RequiredSize.Y += RecoveryIconSize;
                    }

                    bool HasBuffs = Game1.objectInformation[Item.ParentSheetIndex].Split(new char[] { '/' }).Length > 7;
                    if (HasBuffs)
                    {
                        List<string> Buffs = Game1.objectInformation[Item.ParentSheetIndex].Split(new char[] { '/' })[7].Split(new char[] { ' ' }).Where(x => int.Parse(x) != 0).ToList();
                        RequiredSize.Y += Buffs.Count * RecoveryIconSize;
                    }

                    RequiredSize.Y += 5;
                }

                if (DrawSingleItemValue)
                {
                    RequiredSize.Y += 5;

                    Vector2 ValueSize = new Vector2(MeasureNumber(SalePrice, ValueTextScale), TinyDigitBaseHeight * ValueTextScale);
                    ValueSize.X += MoneyIconSourceRect.Width * MoneyIconScale + 8;
                    RequiredSize.X = Math.Max(RequiredSize.X, ValueSize.X);
                    RequiredSize.Y += Math.Max(ValueSize.Y, MoneyIconSourceRect.Height * MoneyIconScale);

                    RequiredSize.Y += 5;
                }

                if (DrawStackValue)
                {
                    RequiredSize.Y += 5;

                    Vector2 ValueSize = new Vector2();
                    ValueSize.X += MoneyIconSourceRect.Width * MoneyIconScale + 8; // width of the Gold icon and a few pixels of extra spacing between the icon and the text
                    //  The value is displayed as: <Price>*<Qty>=<Total>
                    ValueSize.X += MeasureNumber(SalePrice, ValueTextScale);
                    ValueSize.X += ValueFont.MeasureString("*").X;
                    ValueSize.X += MeasureNumber(Obj.Stack, ValueTextScale);
                    ValueSize.X += ValueFont.MeasureString("=").X;
                    ValueSize.X += MeasureNumber(Obj.Stack * SalePrice, ValueTextScale);
                    ValueSize.Y = TinyDigitBaseHeight * ValueTextScale;

                    RequiredSize.X = Math.Max(RequiredSize.X, ValueSize.X);
                    RequiredSize.Y += Math.Max(ValueSize.Y, MoneyIconSourceRect.Height * MoneyIconScale);
                    RequiredSize.Y += 5;
                }

                if (Obj != null && MaximumCapacity.HasValue && (AlwaysShowCapacity || Obj.Stack > 0))
                {
                    RequiredSize.Y += 10;
                    Vector2 CapacitySize = new Vector2(MeasureNumber(Obj.Stack, ValueTextScale) + ValueFont.MeasureString("/").X + MeasureNumber(MaximumCapacity.Value, ValueTextScale), TinyDigitBaseHeight * ValueTextScale);
                    RequiredSize.X = Math.Max(RequiredSize.X, CapacitySize.X);
                    RequiredSize.Y += CapacitySize.Y;
                }

                RequiredSize += new Vector2(Margin * 2);
                RequiredSize.Y -= Margin / 2;

                //  Ensure tooltip is fully visible on screen
                Rectangle Position = new Rectangle(AnchorPoint.Right, AnchorPoint.Top, (int)RequiredSize.X, (int)RequiredSize.Y);
                if (Position.Right > Game1.viewport.Size.Width)
                    Position = new Rectangle(AnchorPoint.Left - (int)RequiredSize.X, Position.Top, Position.Width, Position.Height);
                if (Position.Bottom > Game1.viewport.Size.Height)
                    Position = new Rectangle(Position.X, AnchorPoint.Bottom - (int)RequiredSize.Y, Position.Width, Position.Height);

                DrawBox(b, Position);

                Vector2 CurrentPosition = new Vector2(Position.X + Margin, Position.Y + Margin);
                if (DrawName || DrawCategory)
                {
                    if (DrawName)
                    {
                        Vector2 NameSize = Font.MeasureString(Item.DisplayName) * NameScale;
                        b.DrawString(Font, Item.DisplayName, CurrentPosition, Color.Black, 0f, Vector2.Zero, NameScale, SpriteEffects.None, 1f);
                        CurrentPosition.Y += NameSize.Y;
                    }
                    if (DrawCategory)
                    {
                        Vector2 CategorySize = Font.MeasureString(Item.getCategoryName()) * OtherTextScale;
                        b.DrawString(Font, Item.getCategoryName(), CurrentPosition, Item.getCategoryColor(), 0f, Vector2.Zero, OtherTextScale, SpriteEffects.None, 1f);
                        CurrentPosition.Y += CategorySize.Y;
                    }

                    CurrentPosition.Y += SeparatorMargin;
                    DrawHorizontalSeparator(b, Position.X, (int)(CurrentPosition.Y - SeparatorHeight / 2), Position.Width, SeparatorHeight);
                    CurrentPosition.Y += SeparatorMargin;
                }

                if (DrawDescription)
                {
                    Vector2 DescriptionSize = Font.MeasureString(Item.getDescription()) * DescriptionScale;
                    b.DrawString(Font, Item.getDescription(), CurrentPosition, Color.Black, 0f, Vector2.Zero, OtherTextScale, SpriteEffects.None, 1f);
                    CurrentPosition.Y += DescriptionSize.Y;

                    CurrentPosition.Y += SeparatorMargin;
                    DrawHorizontalSeparator(b, Position.X, (int)(CurrentPosition.Y - SeparatorHeight / 2), Position.Width, SeparatorHeight);
                    CurrentPosition.Y += SeparatorMargin;
                }

                if (DrawRecovery)
                {
                    CurrentPosition.Y += 5;

                    //  Draw health/stamina recovery
                    int StaminaRecovery = Obj.staminaRecoveredOnConsumption();
                    if (StaminaRecovery >= 0)
                    {
                        Rectangle StaminaIconSourcePosition = new Rectangle(StaminaRecovery < 0 ? 140 : 0, 428, 10, 10);
                        b.Draw(Game1.mouseCursors, CurrentPosition, StaminaIconSourcePosition, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);
                        string StaminaRecoveryText = Game1.content.LoadString("Strings\\UI:ItemHover_Energy", string.Concat(StaminaRecovery > 0 ? "+" : "", StaminaRecovery));
                        b.DrawString(Font, StaminaRecoveryText, new Vector2(CurrentPosition.X + RecoveryIconSize, CurrentPosition.Y), Color.Black, 0f, Vector2.Zero, OtherTextScale, SpriteEffects.None, 1f);
                        CurrentPosition.Y += RecoveryIconSize;

                        int HealthRecovery = Obj.healthRecoveredOnConsumption();
                        if (HealthRecovery > 0)
                        {
                            Rectangle HealthIconSourcePosition = new Rectangle(0, 438, 10, 10);
                            b.Draw(Game1.mouseCursors, CurrentPosition, HealthIconSourcePosition, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);
                            string HealthRecoveryText = Game1.content.LoadString("Strings\\UI:ItemHover_Health", string.Concat(HealthRecovery > 0 ? "+" : "", HealthRecovery));
                            b.DrawString(Font, HealthRecoveryText, new Vector2(CurrentPosition.X + RecoveryIconSize, CurrentPosition.Y), Color.Black, 0f, Vector2.Zero, OtherTextScale, SpriteEffects.None, 1f);

                            CurrentPosition.Y += RecoveryIconSize;
                        }
                    }
                    else if (StaminaRecovery != -300)
                    {
                        Rectangle IconSourcePosition = new Rectangle(140, 428, 10, 10);
                        b.Draw(Game1.mouseCursors, CurrentPosition, IconSourcePosition, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);
                        string RecoveryText = Game1.content.LoadString("Strings\\UI:ItemHover_Energy", string.Concat(StaminaRecovery));
                        b.DrawString(Font, RecoveryText, new Vector2(CurrentPosition.X + RecoveryIconSize, CurrentPosition.Y), Color.Black, 0f, Vector2.Zero, OtherTextScale, SpriteEffects.None, 1f);

                        CurrentPosition.Y += RecoveryIconSize;
                    }

                    //  Draw buff effects
                    bool HasBuffs = Game1.objectInformation[Item.ParentSheetIndex].Split(new char[] { '/' }).Length > 7;
                    if (HasBuffs)
                    {
                        string[] Buffs = Game1.objectInformation[Item.ParentSheetIndex].Split(new char[] { '/' })[7].Split(new char[] { ' ' });
                        for (int i = 0; i < Buffs.Length; i++)
                        {
                            int BuffAmount = Convert.ToInt32(Buffs[i]);
                            if (BuffAmount != 0)
                            {
                                Rectangle IconSourcePosition = new Rectangle(10 + i * 10, 428, 10, 10);
                                b.Draw(Game1.mouseCursors, CurrentPosition, IconSourcePosition, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);
                                string BuffText = string.Concat(BuffAmount > 0 ? "+" : "", BuffAmount);
                                if (i <= 11)
                                    BuffText = Game1.content.LoadString(string.Concat("Strings\\UI:ItemHover_Buff", i), BuffText);
                                b.DrawString(Font, BuffText, new Vector2(CurrentPosition.X + RecoveryIconSize, CurrentPosition.Y), Color.Black, 0f, Vector2.Zero, OtherTextScale, SpriteEffects.None, 1f);

                                CurrentPosition.Y += RecoveryIconSize;
                            }
                        }
                    }

                    CurrentPosition.Y += 5;
                }

                if (DrawSingleItemValue)
                {
                    CurrentPosition.Y += 5;

                    Vector2 ValueSize = new Vector2(MeasureNumber(SalePrice, ValueTextScale), TinyDigitBaseHeight * ValueTextScale);
                    int MoneyIconWidth = (int)(MoneyIconSourceRect.Width * MoneyIconScale);
                    int MoneyIconHeight = (int)(MoneyIconSourceRect.Height * MoneyIconScale);

                    Rectangle MoneyIconDestination = new Rectangle((int)CurrentPosition.X, (int)(CurrentPosition.Y + (ValueSize.Y - MoneyIconHeight) / 2), MoneyIconWidth, MoneyIconHeight);
                    if (MoneyIconHeight > ValueSize.Y)
                        MoneyIconDestination = new Rectangle((int)CurrentPosition.X, (int)(CurrentPosition.Y), MoneyIconWidth, MoneyIconHeight);
                    Vector2 ValueDestination = new Vector2(CurrentPosition.X + MoneyIconWidth + 8, CurrentPosition.Y);
                    if (MoneyIconHeight > ValueSize.Y)
                        ValueDestination.Y = CurrentPosition.Y + (MoneyIconHeight - ValueSize.Y) / 2;

                    b.Draw(MoneySpriteSheet, MoneyIconDestination, MoneyIconSourceRect, Color.White);
                    Utility.drawTinyDigits(SalePrice, b, ValueDestination, ValueTextScale, 1f, Color.White);

                    CurrentPosition.Y += Math.Max(ValueSize.Y, MoneyIconHeight);
                    CurrentPosition.Y += 5;
                }

                if (DrawStackValue)
                {
                    CurrentPosition.Y += 5;

                    Vector2 ValueSize = new Vector2();
                    ValueSize.X += MeasureNumber(SalePrice, ValueTextScale);
                    ValueSize.X += ValueFont.MeasureString("x").X;
                    ValueSize.X += MeasureNumber(Obj.Stack, ValueTextScale);
                    ValueSize.X += ValueFont.MeasureString("=").X;
                    ValueSize.X += MeasureNumber(Obj.Stack * SalePrice, ValueTextScale);
                    ValueSize.Y = TinyDigitBaseHeight * ValueTextScale;

                    int MoneyIconWidth = (int)(MoneyIconSourceRect.Width * MoneyIconScale);
                    int MoneyIconHeight = (int)(MoneyIconSourceRect.Height * MoneyIconScale);

                    Rectangle MoneyIconDestination = new Rectangle((int)CurrentPosition.X, (int)(CurrentPosition.Y + (ValueSize.Y - MoneyIconHeight) / 2), MoneyIconWidth, MoneyIconHeight);
                    if (MoneyIconHeight > ValueSize.Y)
                        MoneyIconDestination = new Rectangle((int)CurrentPosition.X, (int)(CurrentPosition.Y), MoneyIconWidth, MoneyIconHeight);
                    Vector2 ValueDestination = new Vector2(CurrentPosition.X + MoneyIconWidth + 8, CurrentPosition.Y);
                    if (MoneyIconHeight > ValueSize.Y)
                        ValueDestination.Y = CurrentPosition.Y + (MoneyIconHeight - ValueSize.Y) / 2;

                    b.Draw(MoneySpriteSheet, MoneyIconDestination, MoneyIconSourceRect, Color.White);

                    Utility.drawTinyDigits(SalePrice, b, ValueDestination, ValueTextScale, 1f, Color.White);
                    ValueDestination.X += MeasureNumber(SalePrice, ValueTextScale);
                    b.DrawString(ValueFont, "*", new Vector2(ValueDestination.X, ValueDestination.Y + ValueFont.LineSpacing / 4), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    ValueDestination.X += ValueFont.MeasureString("*").X;
                    Utility.drawTinyDigits(Obj.Stack, b, ValueDestination, ValueTextScale, 1f, Color.White);
                    ValueDestination.X += MeasureNumber(Obj.Stack, ValueTextScale);
                    b.DrawString(ValueFont, "=", new Vector2(ValueDestination.X, ValueDestination.Y - ValueFont.LineSpacing / 4), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    ValueDestination.X += ValueFont.MeasureString("=").X;
                    Utility.drawTinyDigits(Obj.Stack * SalePrice, b, ValueDestination, ValueTextScale, 1f, Color.White);

                    CurrentPosition.Y += Math.Max(ValueSize.Y, MoneyIconHeight);
                    CurrentPosition.Y += 5;
                }

                if (Obj != null && MaximumCapacity.HasValue && (AlwaysShowCapacity || Obj.Stack > 0))
                {
                    CurrentPosition.Y += 10;
                    Vector2 CapacitySize = new Vector2(MeasureNumber(Obj.Stack, ValueTextScale) + ValueFont.MeasureString("/").X + MeasureNumber(MaximumCapacity.Value, ValueTextScale), TinyDigitBaseHeight * ValueTextScale);
                    Vector2 CapacityPosition = new Vector2(CurrentPosition.X + (Position.Width - Margin * 2 - CapacitySize.X) / 2, CurrentPosition.Y);

                    Color CapacityColor = CapacityColorOverride.HasValue ? CapacityColorOverride.Value :
                        Obj.Stack >= MaximumCapacity.Value ? Color.Red : Obj.Stack >= MaximumCapacity.Value * 0.9 ? Color.Orange : Color.White;
                    Utility.drawTinyDigits(Obj.Stack, b, CapacityPosition, ValueTextScale, 1f, CapacityColor);
                    CapacityPosition.X += MeasureNumber(Obj.Stack, ValueTextScale);
                    Utility.drawTextWithShadow(b, "/", ValueFont, CapacityPosition, CapacityColor);
                    CapacityPosition.X += ValueFont.MeasureString("/").X;
                    Utility.drawTinyDigits(MaximumCapacity.Value, b, CapacityPosition, ValueTextScale, 1f, CapacityColor);

                    CurrentPosition.Y += CapacitySize.Y;
                }
            }
            finally
            {
                Font.LineSpacing = PreviousLineSpacing;
            }
        }
    }
}
