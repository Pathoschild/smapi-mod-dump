/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-MachineAugmentors
**
*************************************************/

using MachineAugmentors.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Object = StardewValley.Object;
using Size = xTile.Dimensions.Size;

namespace MachineAugmentors.Items
{
    [XmlType("Mods_MachineAugmentors_Augmentor")]
    [XmlRoot(ElementName = "Augmentor", Namespace = "")]
    [KnownType(typeof(OutputAugmentor))]
    [KnownType(typeof(SpeedAugmentor))]
    [KnownType(typeof(EfficiencyAugmentor))]
    [KnownType(typeof(QualityAugmentor))]
    [KnownType(typeof(ProductionAugmentor))]
    [KnownType(typeof(DuplicationAugmentor))]
    [XmlInclude(typeof(OutputAugmentor))]
    [XmlInclude(typeof(SpeedAugmentor))]
    [XmlInclude(typeof(EfficiencyAugmentor))]
    [XmlInclude(typeof(QualityAugmentor))]
    [XmlInclude(typeof(ProductionAugmentor))]
    [XmlInclude(typeof(DuplicationAugmentor))]
    public abstract class Augmentor : Object
    {
        public UserConfig UserConfig { get { return MachineAugmentorsMod.UserConfig; } }
        public AugmentorConfig AugmentorConfig { get { return UserConfig.GetConfig(AugmentorType); } }
        public AugmentorType AugmentorType { get; }

        public static readonly Random Randomizer = new Random();
        /// <summary>Returns true if a randomly generated number between 0.0 and 1.0 is less than or equal to the given ChanceOfSuccess</summary>
        /// <param name="ChanceOfSuccess">A value between 0.0 and 1.0. EX: if 0.7, there is a 70% chance that this function returns true.</param>
        /// <returns></returns>
        public static bool RollDice(double ChanceOfSuccess)
        {
            return Randomizer.NextDouble() <= ChanceOfSuccess;
        }

        /// <summary>Returns a random number between the given Minimum and Maximum values.</summary>
        public static double GetRandomNumber(double Minimum, double Maximum)
        {
            return Randomizer.NextDouble() * (Maximum - Minimum) + Minimum;
        }

        /// <summary>Rounds the given double up or down to an integer. The result is more likely to be rounded to whichever is closer.<para/>
        /// EX: If Value=4.3, there is a 70% chance of rounding down to 4, 30% chance of rounding up to 5.</summary>
        public static int WeightedRound(double Value)
        {
            int BaseAmount = (int)Value;
            double RoundUpChance = Value - BaseAmount;
            int NewValue = BaseAmount + Convert.ToInt32(RollDice(RoundUpChance));
            return NewValue;
        }

        protected Augmentor(AugmentorType Type)
            : base(22763 + (int)Type, 1, false, -1, 0)
        {
            this.AugmentorType = Type;
        }

        //other possible icons: 
        //Hammer (cursors, near center)
        //lightning bolt (emojis, center)
        //bell (emojis, bottomright)
        //dice (cursors, left-center)
        //blueprint diagram (cursors, center)
        //temporary_sprites_1 - the artifact trove box animation
        //springobjects topleft, computer next to dwarven helmet

        public Augmentor() : this(AugmentorType.Speed) { }

        public static Augmentor CreateInstance(AugmentorType Type, int Quantity = 1)
        {
            if (Type == AugmentorType.Output)
                return new OutputAugmentor() { Stack = Quantity };
            else if (Type == AugmentorType.Speed)
                return new SpeedAugmentor() { Stack = Quantity };
            else if (Type == AugmentorType.Efficiency)
                return new EfficiencyAugmentor() { Stack = Quantity };
            else if (Type == AugmentorType.Quality)
                return new QualityAugmentor() { Stack = Quantity };
            else if (Type == AugmentorType.Production)
                return new ProductionAugmentor() { Stack = Quantity };
            else if (Type == AugmentorType.Duplication)
                return new DuplicationAugmentor() { Stack = Quantity };
            else
                throw new NotImplementedException(string.Format("Unrecognized AugmentorType: {0}", Type.ToString()));
        }

        public static double ComputeEffect(AugmentorType AttachedType, int AttachedQuantity, bool RequiresInput, int ProcessingTime = 60)
        {
            if (AttachedType == AugmentorType.Output)
                return OutputAugmentor.ComputeEffect(AttachedQuantity, RequiresInput);
            else if (AttachedType == AugmentorType.Speed)
                return SpeedAugmentor.ComputeEffect(AttachedQuantity, RequiresInput);
            else if (AttachedType == AugmentorType.Efficiency)
                return EfficiencyAugmentor.ComputeEffect(AttachedQuantity, RequiresInput);
            else if (AttachedType == AugmentorType.Quality)
                return QualityAugmentor.ComputeEffect(AttachedQuantity, RequiresInput);
            else if (AttachedType == AugmentorType.Production)
                return ProductionAugmentor.ComputeEffect(AttachedQuantity, RequiresInput);
            else if (AttachedType == AugmentorType.Duplication)
                return DuplicationAugmentor.ComputeEffect(AttachedQuantity, RequiresInput, ProcessingTime);
            else
                throw new NotImplementedException(string.Format("Unrecognized AugmentorType: {0}", AttachedType.ToString()));
        }

        public static double GetDefaultEffect(AugmentorType AttachedType)
        {
            if (AttachedType == AugmentorType.Output)
                return 1.0;
            else if (AttachedType == AugmentorType.Speed)
                return 1.0;
            else if (AttachedType == AugmentorType.Efficiency)
                return 1.0;
            else if (AttachedType == AugmentorType.Quality)
                return 0.0;
            else if (AttachedType == AugmentorType.Production)
                return 1.0;
            else if (AttachedType == AugmentorType.Duplication)
                return 0.0;
            else
                throw new NotImplementedException(string.Format("Unrecognized AugmentorType: {0}", AttachedType.ToString()));
        }

        public static void DrawIcon(SpriteBatch SB, AugmentorType Type, Vector2 PrimaryIconBottomLeft, float BaseScale, float Transparency, float LayerDepth)
        {
            if (TryGetIconDetails(Type, out Texture2D Texture, out Rectangle SourceRect, out float IconScale, out SpriteEffects Effects))
            {
                Vector2 Destination = PrimaryIconBottomLeft + new Vector2(4f * BaseScale, -4f * BaseScale - SourceRect.Height * IconScale * BaseScale);
                SB.Draw(Texture, Destination, SourceRect, Color.White * Transparency, 0f, new Vector2(0, 0), BaseScale * IconScale, Effects, LayerDepth);
            }
        }

        public static bool TryGetIconDetails(AugmentorType Type, out Texture2D SourceTexture, out Rectangle SourceRect, out float BaseScale, out SpriteEffects Effects)
        {
            if (Type == AugmentorType.Output)
            {
                SourceTexture = TextureHelpers.EmojiSpritesheet;
                SourceRect = new Rectangle(55, 90, 7, 9); // Lightbulb
                BaseScale = 3f;
                Effects = SpriteEffects.None;
                return true;
            }
            else if (Type == AugmentorType.Speed)
            {
                SourceTexture = Game1.mouseCursors;
                SourceRect = new Rectangle(410, 501, 9, 9); // Clock
                BaseScale = 3f;
                Effects = SpriteEffects.None;
                return true;
            }
            else if (Type == AugmentorType.Efficiency)
            {
                SourceTexture = Game1.mouseCursors;
                SourceRect = new Rectangle(80, 428, 10, 10); // E^
                BaseScale = 3f;
                Effects = SpriteEffects.None;
                return true;
            }
            else if (Type == AugmentorType.Quality)
            {
                SourceTexture = TextureHelpers.PlayerStatusList;
                SourceRect = new Rectangle(16, 48, 16, 16); // Star
                BaseScale = 2f;
                Effects = SpriteEffects.None;
                return true;
            }
            else if (Type == AugmentorType.Production)
            {
                SourceTexture = Game1.mouseCursors;
                SourceRect = new Rectangle(130, 430, 10, 7); // Weapon speed icon
                BaseScale = 3f;
                Effects = SpriteEffects.FlipHorizontally;
                return true;
            }
            else if (Type == AugmentorType.Duplication)
            {
                SourceTexture = Game1.mouseCursors;
                SourceRect = new Rectangle(395, 497, 6, 8); // Blue/Yellow exclamation marks
                BaseScale = 4f;
                Effects = SpriteEffects.None;
                return true;
            }
            else
            {
                SourceTexture = null;
                SourceRect = new Rectangle();
                BaseScale = 1f;
                Effects = SpriteEffects.None;
                return false;
            }
        }

        protected bool BaseIsAugmentable(Object Item)
        {
            if (Item == null || !Item.bigCraftable.Value || !Item.isPlaceable())
            {
                return false;
            }
            else
            {
                return IsAugmentable(Item);
            }
        }

        //public abstract double sComputeEffect(int AugmentorQuantity, bool TargetRequiresInput);
        public abstract string GetEffectDescription();
        public abstract bool IsAugmentable(Object Item);
        public abstract int GetPurchasePrice();
        public abstract int GetSellPrice();
        public abstract string GetDisplayName();
        public abstract string GetDescription();
        public abstract Augmentor CreateSingle();
        public abstract bool CanStackWith(ISalable Other);
        public abstract Color GetPrimaryIconColor();

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1) { base.draw(spriteBatch, x, y, alpha); }
        public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1) { base.draw(spriteBatch, xNonTile, yNonTile, layerDepth, alpha); }

        /// <summary>Returns the position of the main texture used for this item</summary>
        private static Rectangle GetPrimaryIconSourceRect(int Padding = 2)
        {
            //Cursors.xnb 162,324,13,9
            int TextureWidth = 13;
            int TextureHeight = 9;
            Rectangle SourceRect = new Rectangle(162 - Padding, 324 - Padding, TextureWidth + Padding * 2, TextureHeight + Padding * 2);
            return SourceRect;
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            float Scale = 1f;
            spriteBatch.Draw(Game1.mouseCursors, objectPosition + new Vector2(32f, 32f), GetPrimaryIconSourceRect(2), GetPrimaryIconColor(), 0f, new Vector2(8f, 8f), 4f * Scale, SpriteEffects.None, 0.9f);

            float Offset = (64f - 64f * Scale) / 2f;
            Vector2 BottomLeftPosition = new Vector2(objectPosition.X + Offset, objectPosition.Y + 64f + Offset);
            DrawIcon(spriteBatch, AugmentorType, BottomLeftPosition, Scale, 1f, 1f);
        }

        /// <summary>Returns the layerDepth value to use when calling <see cref="SpriteBatch.Draw(Texture2D, Vector2, Rectangle?, Color, float, Vector2, float, SpriteEffects, float)"/> so that 
        /// the drawn texture at the given tile appears at the correct layer. (Objects placed on tiles at higher Y-values should appear over-top of this tile's textures)</summary>
        private static float GetTileDrawLayerDepth(int TileX, int TileY)
        {
            // Copied from decompiled code: StardewValley.Object.cs
            // public virtual void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
            // when it's drawing the bigCraftable spritesheet's texture
            float draw_layer = Math.Max(0f, (float)((TileY + 1) * 64 - 24) / 10000f) + (float)TileX * 1E-05f;
            return draw_layer;
        }

        /// <summary>Draws the Augmentor textures on the given tile of the game world</summary>
        public static void DrawOnTile(SpriteBatch b, AugmentorType Type, int TileX, int TileY, float Scale)
        {
            float draw_layer = GetTileDrawLayerDepth(TileX, TileY);
            float DrawLayerOffset = 1E-05f; // The SpriteBatch LayerDepth needs to be slightly larger than the layer depth used for the bigCraftable texture, because if they're equal, the textures will flicker, changing order every few game ticks at random

            Vector2 Position = Game1.GlobalToLocal(Game1.viewport, new Vector2(TileX * Game1.tileSize, TileY * Game1.tileSize));
            b.Draw(Game1.mouseCursors, Position, GetPrimaryIconSourceRect(2), Color.White, 0f, Vector2.Zero, 4f * Scale, SpriteEffects.None, draw_layer + DrawLayerOffset);

            Vector2 BottomLeftPosition = new Vector2(Position.X, Position.Y + 64f * Scale);
            DrawIcon(b, Type, BottomLeftPosition, Scale, 1f, draw_layer + DrawLayerOffset + 1E-04f);
        }

        /// <summary>Draws the icons associated with each AugmentorType onto the given tile of the game world's current GameLocation</summary>
        public static void DrawIconsOnTile(SpriteBatch b, List<AugmentorType> Types, int TileX, int TileY, float Scale)
        {
            double IconTransparency = MachineAugmentorsMod.UserConfig.IconOpacity;
            if (IconTransparency <= 0.0)
                return;
            Color RenderColor = Color.White * (float)IconTransparency;

            float draw_layer = GetTileDrawLayerDepth(TileX, TileY);
            float DrawLayerOffset = 1E-05f; // The SpriteBatch LayerDepth needs to be slightly larger than the layer depth used for the bigCraftable texture, because if they're equal, the textures will flicker, changing order every few game ticks at random

            Vector2 TopLeftTilePosition = Game1.GlobalToLocal(Game1.viewport, new Vector2(TileX * Game1.tileSize, TileY * Game1.tileSize));

            //  Compute the size of each icon
            List<Vector2> IconSizes = new List<Vector2>();
            foreach (AugmentorType Type in Types)
            {
                if (TryGetIconDetails(Type, out Texture2D Texture, out Rectangle SourceRect, out float BaseScale, out SpriteEffects Effects))
                {
                    float ActualScale = BaseScale * Scale;
                    float IconWidth = SourceRect.Width * ActualScale;
                    float IconHeight = SourceRect.Height * ActualScale;
                    IconSizes.Add(new Vector2(IconWidth, IconHeight));
                }
            }
            if (!IconSizes.Any())
                return;

            //  Compute the size of each row
            int Margin = 2; // Margin between each icon, both vertically and horizontally
            int LeftRightPadding = 4;
            List<Vector2> LineSizes = new List<Vector2>();
            float CurrentLineWidth = 0f;
            float CurrentLineHeight = 0f;
            foreach (Vector2 Size in IconSizes)
            {
                if (CurrentLineWidth + Size.X > Game1.tileSize - LeftRightPadding * 2)
                {
                    //  Wrap to new line
                    LineSizes.Add(new Vector2(CurrentLineWidth, CurrentLineHeight));
                    CurrentLineWidth = 0f;
                    CurrentLineHeight = 0f;
                }

                if (CurrentLineWidth != 0f)
                    CurrentLineWidth += Margin;
                CurrentLineWidth += Size.X;
                CurrentLineHeight = Math.Max(CurrentLineHeight, Size.Y);
            }
            LineSizes.Add(new Vector2(CurrentLineWidth, CurrentLineHeight));

            float TotalWidth = LineSizes.Max(Line => Line.X);
            float TotalHeight = LineSizes.Sum(Line => Line.Y) + (LineSizes.Count - 1) * Margin;

            float StartX = TopLeftTilePosition.X + (Game1.tileSize - TotalWidth) / 2;
            float StartY = TopLeftTilePosition.Y + (Game1.tileSize - TotalHeight) / 2;

            int CurrentLineIndex = 0;
            float CurrentX = StartX;
            float CurrentY = StartY;
            foreach (AugmentorType Type in Types)
            {
                if (TryGetIconDetails(Type, out Texture2D Texture, out Rectangle SourceRect, out float BaseScale, out SpriteEffects Effects))
                {
                    float ActualScale = BaseScale * Scale;
                    float IconWidth = SourceRect.Width * ActualScale;
                    float IconHeight = SourceRect.Height * ActualScale;

                    if (CurrentX + IconWidth > StartX + Game1.tileSize)
                    {
                        //  Wrap to new line
                        CurrentX = StartX;
                        CurrentY += LineSizes[CurrentLineIndex].Y + Margin;
                        CurrentLineIndex++;
                    }

                    Vector2 CurrentPosition = new Vector2(CurrentX, CurrentY + (LineSizes[CurrentLineIndex].Y - IconHeight) / 2);
                    b.Draw(Texture, CurrentPosition, SourceRect, RenderColor, 0f, Vector2.Zero, ActualScale, Effects, draw_layer + DrawLayerOffset);
                    
                    CurrentX += IconWidth + Margin;
                }
            }
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(32f, 32f), GetPrimaryIconSourceRect(2), GetPrimaryIconColor() * transparency, 0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);

            if (drawStackNumber != StackDrawType.Hide)
            {
                float QuantityScale = 2.7f * scaleSize;
                int NumDigits = DrawHelpers.GetNumDigits(Stack);
                Vector2 BottomRightPosition = new Vector2(location.X + 64f - DrawHelpers.MeasureNumber(Stack, QuantityScale),
                    location.Y + 64f - DrawHelpers.TinyDigitBaseHeight * QuantityScale);
                Utility.drawTinyDigits(Stack, spriteBatch, BottomRightPosition, QuantityScale, 0f, Color.White);
            }

            float Offset = (64f - 64f * scaleSize ) / 2f;
            Vector2 BottomLeftPosition = new Vector2(location.X + Offset, location.Y + 64f + Offset);
            DrawIcon(spriteBatch, AugmentorType, BottomLeftPosition, scaleSize, transparency, 1f);
        }

        public override string DisplayName {
            get { return GetDisplayName(); }
            set { /*base.DisplayName = value;*/ }
        }

        public override bool canBePlacedHere(GameLocation location, Vector2 tile)
        {
            if (Context.IsMultiplayer && !Context.IsMainPlayer)
            {
                return false;
            }
            else
            {
                Object Item = location.getObjectAtTile((int)tile.X, (int)tile.Y);
                if (Item != null && BaseIsAugmentable(Item))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            KeyboardState KeyState = Game1.GetKeyboardState();
            bool IsShiftHeld = KeyState.IsKeyDown(Keys.LeftShift) || KeyState.IsKeyDown(Keys.RightShift);
            bool IsControlHeld = KeyState.IsKeyDown(Keys.LeftControl) || KeyState.IsKeyDown(Keys.RightControl);

            int QtyToPlace = Math.Min(Stack, IsControlHeld ? Math.Max(1, Stack / 2) : IsShiftHeld ? 1 : Stack);
            PlacedAugmentorsManager.Instance.OnAugmentorPlaced(location.NameOrUniqueName, AugmentorType, QtyToPlace, true, x / Game1.tileSize, y / Game1.tileSize, this);

            location.playSound("woodyStep");
            return false;
        }

        private static readonly Color CategoryColor = new Color(156, 122, 37);
        public override Color getCategoryColor() { return CategoryColor; }
        public override string getHoverBoxText(Item hoveredItem) { return base.getHoverBoxText(hoveredItem); }
        public override void hoverAction() { base.hoverAction(); }
        public override bool performToolAction(Tool t, GameLocation location) { return base.performToolAction(t, location); }
        protected override string loadDisplayName() { return GetDisplayName(); }
        public override bool canBeDropped() { return true; }
        public override bool canBeGivenAsGift() { return false; }
        public override int attachmentSlots() { return 0; }
        public override bool canBePlacedInWater() { return false; }
        public override bool canBeShipped() { return false; }
        public override bool canBeTrashed() { return true; }
        public override bool canStackWith(ISalable other) { return CanStackWith(other); }
        public override string getCategoryName() { return MachineAugmentorsMod.Translate("AugmentorCategoryName"); }
        public override string getDescription() { return GetDescription(); }
        public override Item getOne() { return CreateSingle(); }
        public override bool isPlaceable() { return true; }
        public override int maximumStackSize() { return 999; }
        public override bool isPassable() { return true; }
        public override bool ShouldSerializeparentSheetIndex() { return false; }
        public override int salePrice() { return GetPurchasePrice(); }
        public override int sellToStorePrice(long specificPlayerID = -1) { return GetSellPrice(); }
    }
}
