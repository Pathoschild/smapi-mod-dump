/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/6135/StardewValley.ProfitCalculator
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProfitCalculator.main;
using StardewValley;
using StardewValley.Menus;
using System;

namespace ProfitCalculator.ui
{
    /// <summary>
    /// A box that displays a crop and its information
    /// </summary>
    public class CropBox : BaseOption
    {
        /// <summary> The crop info to display. <see cref="CropInfo"/> </summary>
        public readonly CropInfo cropInfo;

        /// <summary> The hover box to display when the mouse is over the box. <see cref="CropHoverBox"/> </summary>
        public readonly CropHoverBox cropHoverBox;

        private readonly SpriteFont Font = Game1.smallFont;
        private readonly string mainText;

        /// <summary>
        /// Creates a new CropBox
        /// </summary>
        /// <param name="x"> The x position of the box</param>
        /// <param name="y"> The y position of the box</param>
        /// <param name="w"> The width of the box</param>
        /// <param name="h"> The height of the box</param>
        /// <param name="crop"> The cropInfo to display. <see cref="CropInfo"/> </param>
        public CropBox(int x, int y, int w, int h, CropInfo crop) : base(x, y, w, h, () => crop.Crop.Name, () => crop.Crop.Name, () => crop.Crop.Name)
        {
            this.mainText = crop.Crop.Name;
            if (this.mainText.Length < 1)
            {
                this.mainText = "PlaceHolder";
            }
            cropInfo = crop;
            cropHoverBox = new CropHoverBox(cropInfo);
        }

        /// <summary>
        /// Called when the left mouse button is pressed. Executes before the action of the button is performed
        /// </summary>
        /// <param name="x"> The x position of the mouse</param>
        /// <param name="y"> The y position of the mouse</param>
        public override void BeforeReceiveLeftClick(int x, int y)
        {
        }

        /// <inheritdoc/>
        public override void Draw(SpriteBatch b)
        {
            IClickableMenu.drawTextureBox(
                b,
                Game1.menuTexture,
                new(0, 256, 60, 60),
                (int)this.Position.X,// - 16,
                (int)this.Position.Y,// - 8 - 4,
                this.ClickableComponent.bounds.Width,// + 32,
                this.ClickableComponent.bounds.Height,// + 16 + 8,
                Color.White,
                1.2f,
                false,
                0.5f
             );
            //draw crop sprite in the middle of the box aligned to the left
            int spriteSize = 16;
            int spriteDisplaySize = (int)(spriteSize * 3.25f);

            b.Draw(
                cropInfo.Crop.Sprite.Item1,
                new Rectangle(
                    (int)this.Position.X + (3 * Game1.tileSize) / 8,
                    (int)this.Position.Y + (this.ClickableComponent.bounds.Height / 2) - (Game1.tileSize / 2) + 6,
                    spriteDisplaySize,
                    spriteDisplaySize
                ),
                cropInfo.Crop.Sprite.Item2,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.6f
            );

            //draw string in middle of box, aligned to the left with a spacing of 2xtilesize from the left
            //But if string size is too big, draw, reduce font size, and draw again
            float fontSizeModifier = 1.3f;

            float fontSize = (Font.MeasureString(this.mainText).X * fontSizeModifier);

            float rightSideTextMaxSize = Font.MeasureString(cropInfo.ProfitPerDay.ToString("0.00")).X + Font.MeasureString($" {Utils.Helper.Translation.Get("g")}/{Utils.Helper.Translation.Get("day")}").X;
            rightSideTextMaxSize *= 1.8f;

            float boxWidth = (this.ClickableComponent.bounds.Width) - ((3 * Game1.tileSize) / 8) - rightSideTextMaxSize;

            while (fontSize > boxWidth)
            {
                fontSizeModifier -= 0.005f;
                fontSize = (Font.MeasureString(this.mainText).X * fontSizeModifier);
            }

            b.DrawString(
                Font,
                this.mainText,
                new Vector2(
                    this.Position.X + (3 * Game1.tileSize) / 2,
                    this.Position.Y + (this.ClickableComponent.bounds.Height / 2) - (Font.MeasureString(this.mainText).Y / 2)
                ),
                Color.Black,
                0f,
                Vector2.Zero,
                fontSizeModifier,
                SpriteEffects.None,
                0.6f
            );

            string price = Math.Round(cropInfo.TotalProfit).ToString();
            string g = $" {Utils.Helper.Translation.Get("g")}";
            Color color;
            if (cropInfo.TotalProfit < 0)
            {
                color = Color.Red;
            }
            else
            {
                color = Color.DarkGreen;
            }
            b.DrawString(
                Font,
                price,
                new Vector2(
                    this.Position.X + (69 * (Game1.tileSize / 8)) - Font.MeasureString(price).X - Font.MeasureString(g).X,
                    this.Position.Y + (this.ClickableComponent.bounds.Height / 2) + 3 - (Font.MeasureString(price).Y)
                ),
                color,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.6f
            );
            b.DrawString(
                Font,
                g,
                new Vector2(
                    this.Position.X + (69 * (Game1.tileSize / 8)) - Font.MeasureString(g).X,
                    this.Position.Y + (this.ClickableComponent.bounds.Height / 2) + 3 - (Font.MeasureString(g).Y)
                ),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.6f
            );
            //further left, draw the price per day of the crop in the box, rounded to the nearest two decimal places, with G/Day at the end
            string pricePerDay = (cropInfo.ProfitPerDay).ToString("0.00");
            string ppd = $" {Utils.Helper.Translation.Get("g")}/{Utils.Helper.Translation.Get("day")}";
            b.DrawString(
                Font,
                pricePerDay,
                new Vector2(
                    this.Position.X + (69 * (Game1.tileSize / 8)) - Font.MeasureString(pricePerDay).X - Font.MeasureString(ppd).X,
                    this.Position.Y + (this.ClickableComponent.bounds.Height / 2) + 3
                ),
                color,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.6f
            );
            b.DrawString(
                Font,
                ppd,
                new Vector2(
                    this.Position.X + (69 * (Game1.tileSize / 8)) - Font.MeasureString(ppd).X,
                    this.Position.Y + (this.ClickableComponent.bounds.Height / 2) + 3
                ),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.6f
            );

            cropHoverBox.Draw(b);
        }

        /// <summary>
        /// The update event.
        /// </summary>
        public override void Update()
        {
        }

        ///<inheritdoc/>
        public override void PerformHoverAction(int x, int y)
        {
            base.PerformHoverAction(x, y);
            if (this.ClickableComponent.containsPoint(x, y))
            {
                cropHoverBox.Update();
                cropHoverBox.Open(true);
            }
            else
            {
                cropHoverBox.Open(false);
            }
        }
    }
}