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
    ///   Hover details for each crop in the profit calculator.
    /// </summary>
    public class CropHoverBox : IDisposable, IDrawable
    {
        private bool isOpen;
        private readonly int windowWidth;
        private readonly int windowHeight;
        private int x;
        private int y;
        private int hoverDelay;
        private readonly int hoverDelayDefault;
        private Rectangle drawBox;
        private readonly CropInfo cropInfo;
        private readonly SpriteFont font;

        /// <summary>
        /// Creates a new CropHoverBox.
        /// </summary>
        /// <param name="cropInfo"></param>
        public CropHoverBox(CropInfo cropInfo)
        {
            font = Game1.smallFont;
            isOpen = false;
            windowWidth = 400;
            windowHeight = 600;
            x = 0;
            y = 0;
            drawBox = new(x, y, windowWidth, windowHeight);
            this.cropInfo = cropInfo;
            ModConfig config = Utils.Helper.ReadConfig<ModConfig>();
            this.hoverDelay = config?.ToolTipDelay ?? 30;
            this.hoverDelayDefault = config?.ToolTipDelay ?? 30;
        }

        /// <inheritdoc/>

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            isOpen = false;
        }

        /// <inheritdoc/>

        public void Draw(SpriteBatch b)
        {
            if (isOpen && hoverDelay <= 0)
            {
                //Top Panel
                this.DrawMainBox(b);
                //Bottom Panel
                this.DrawSecondaryBox(b);
            }
            else if (isOpen) hoverDelay--;
        }

        private void DrawMainBox(SpriteBatch b)
        {
            IClickableMenu.drawTextureBox(
                b,
                Game1.menuTexture,
                new Rectangle(0, 256, 60, 60),
                drawBox.X,
                drawBox.Y,
                windowWidth,
                windowHeight / 2,
                Color.White,
                1f,
                draw_layer: 0.7f
            );
            Vector3 currentTextPosition = new(drawBox.X, drawBox.Y, drawBox.X + drawBox.Width - (Game1.tileSize / 4));

            #region Crop Value

            //Total profit: Total Profit
            //Total Profit Per Day: P/D
            string totalProfit = $"{Utils.Helper.Translation.Get("total-p")}:";
            string totalProfitValue = $"{Math.Round(cropInfo.TotalProfit)} {Utils.Helper.Translation.Get("g")}";

            currentTextPosition.X += (float)Game1.tileSize / 4;
            currentTextPosition.Y += (float)Game1.tileSize / 4;
            b.DrawString(
                font,
                totalProfit,
                new Vector2(
                    currentTextPosition.X,
                    currentTextPosition.Y),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );
            b.DrawString(
                font,
                totalProfitValue,
                new Vector2(
                    currentTextPosition.Z - font.MeasureString(totalProfitValue).X,
                    currentTextPosition.Y),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );

            string pricePerDay = $"{Utils.Helper.Translation.Get("total-p-day")}:";
            string pricePerDayValue = $"{cropInfo.ProfitPerDay:0.00} {Utils.Helper.Translation.Get("g")}/{Utils.Helper.Translation.Get("day")}";

            currentTextPosition.Y += font.MeasureString(totalProfit).Y;
            b.DrawString(
                font,
                pricePerDay,
                new Vector2(
                    currentTextPosition.X,
                    currentTextPosition.Y
                ),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );
            b.DrawString(
                font,
                pricePerDayValue,
                new Vector2(
                    currentTextPosition.Z - font.MeasureString(pricePerDayValue).X,
                    currentTextPosition.Y),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );

            #endregion Crop Value

            #region Seed Loss

            string totalSeedLoss = $"{Utils.Helper.Translation.Get("total-s-loss")}:";
            string totalSeedLossValue = $"{Math.Round(cropInfo.TotalSeedLoss)} {Utils.Helper.Translation.Get("g")}";

            currentTextPosition.Y += font.MeasureString(pricePerDay).Y + 16;
            b.DrawString(
                font,
                totalSeedLoss,
                new Vector2(
                    currentTextPosition.X,
                    currentTextPosition.Y
                ),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );
            b.DrawString(
                font,
                totalSeedLossValue,
                new Vector2(
                    currentTextPosition.Z - font.MeasureString(totalSeedLossValue).X,
                    currentTextPosition.Y),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );

            string seedLossPerDay = $"{Utils.Helper.Translation.Get("total-s-loss-day")}:";
            string seedLossPerDayValue = $"{cropInfo.SeedLossPerDay:0.00} {Utils.Helper.Translation.Get("g")}/{Utils.Helper.Translation.Get("day")}";

            currentTextPosition.Y += font.MeasureString(totalSeedLoss).Y;
            b.DrawString(
                font,
                seedLossPerDay,
                new Vector2(
                    currentTextPosition.X,
                    currentTextPosition.Y
                ),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );
            b.DrawString(
                font,
                seedLossPerDayValue,
                new Vector2(
                    currentTextPosition.Z - font.MeasureString(seedLossPerDayValue).X,
                    currentTextPosition.Y),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );

            #endregion Seed Loss

            #region Crop details

            string grow = $"{Utils.Helper.Translation.Get("grow-time")}:";
            string growValue = $"{cropInfo.GrowthTime} {Utils.Helper.Translation.Get("days")}";
            currentTextPosition.Y += font.MeasureString(seedLossPerDay).Y + 16;
            b.DrawString(
                font,
                grow,
                new Vector2(
                    currentTextPosition.X,
                    currentTextPosition.Y
                ),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );
            b.DrawString(
                font,
                growValue,
                new Vector2(
                    currentTextPosition.Z - font.MeasureString(growValue).X,
                    currentTextPosition.Y),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );

            string reGrow = $"{Utils.Helper.Translation.Get("regrow-time")}:";
            string reGrowValue = $"{cropInfo.RegrowthTime} {Utils.Helper.Translation.Get("days")}";
            if (cropInfo.RegrowthTime <= 0) reGrowValue = Utils.Helper.Translation.Get("no");
            currentTextPosition.Y += font.MeasureString(seedLossPerDay).Y;
            b.DrawString(
                font,
                reGrow,
                new Vector2(
                    currentTextPosition.X,
                    currentTextPosition.Y
                ),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );
            b.DrawString(
                font,
                reGrowValue,
                new Vector2(
                    currentTextPosition.Z - font.MeasureString(reGrowValue).X,
                    currentTextPosition.Y),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );

            string harvests = $"{Utils.Helper.Translation.Get("harvest-count")}:";
            string harvestsValue = $"#{cropInfo.TotalHarvests}";
            currentTextPosition.Y += font.MeasureString(seedLossPerDay).Y;
            b.DrawString(
                font,
                harvests,
                new Vector2(
                    currentTextPosition.X,
                    currentTextPosition.Y
                ),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );
            b.DrawString(
                font,
                harvestsValue,
                new Vector2(
                    currentTextPosition.Z - font.MeasureString(harvestsValue).X,
                    currentTextPosition.Y),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );

            #endregion Crop details
        }

        private void DrawSecondaryBox(SpriteBatch b)
        {
            IClickableMenu.drawTextureBox(
                b,
                Game1.menuTexture,
                new Rectangle(0, 256, 60, 60),
                drawBox.X,
                drawBox.Y + (windowHeight / 2) - (Game1.tileSize / 4),
                windowWidth,
                windowHeight - (windowHeight / 2),
                Color.White,
                1f,
                draw_layer: 0.71f
            );

            Vector3 currentTextPosition = new(
                drawBox.X,
                drawBox.Y + +(windowHeight / 2) - (Game1.tileSize / 4),
                drawBox.X + drawBox.Width - (Game1.tileSize / 4));

            #region Crop Value

            //Total profit: Total Profit
            //Total Profit Per Day: P/D
            string minHarvest = $"{Utils.Helper.Translation.Get("min-harvests")}:";
            string minHarvestValue = $"#{cropInfo.Crop.MinHarvests}";
            currentTextPosition.X += (float)Game1.tileSize / 4;
            currentTextPosition.Y += (float)Game1.tileSize / 4;
            b.DrawString(
                font,
                minHarvest,
                new Vector2(
                    currentTextPosition.X,
                    currentTextPosition.Y
                ),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );
            b.DrawString(
                font,
                minHarvestValue,
                new Vector2(
                    currentTextPosition.Z - font.MeasureString(minHarvestValue).X,
                    currentTextPosition.Y),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );

            string maxHarvest = $"{Utils.Helper.Translation.Get("max-harvests")}:";
            string maxHarvestValue = $"#{cropInfo.Crop.MaxHarvests}";

            currentTextPosition.Y += font.MeasureString(maxHarvest).Y;

            b.DrawString(
                font,
                maxHarvest,
                new Vector2(
                    currentTextPosition.X,
                    currentTextPosition.Y
                ),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );
            b.DrawString(
                font,
                maxHarvestValue,
                new Vector2(
                    currentTextPosition.Z - font.MeasureString(maxHarvestValue).X,
                    currentTextPosition.Y),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );

            if (cropInfo.Crop.MaxHarvestIncreasePerFarmingLevel != 0)
            {
                string maxHarvestIncLevl = $"{Utils.Helper.Translation.Get("max-harvests-level")}:";
                string maxHarvestIncLevlValue = $"#{cropInfo.Crop.MaxHarvestIncreasePerFarmingLevel}";

                currentTextPosition.Y += font.MeasureString(maxHarvestIncLevl).Y;

                b.DrawString(
                    font,
                    maxHarvestIncLevl,
                    new Vector2(
                        currentTextPosition.X,
                        currentTextPosition.Y
                    ),
                    Color.Black,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0.75f
                );
                b.DrawString(
                    font,
                    maxHarvestIncLevlValue,
                    new Vector2(
                        currentTextPosition.Z - font.MeasureString(maxHarvestIncLevlValue).X,
                        currentTextPosition.Y),
                    Color.Black,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0.75f
                );
            }
            if (cropInfo.Crop.ChanceForExtraCrops != 0)
            {
                string extraChance = $"{Utils.Helper.Translation.Get("extra-harvest-chance")}:";
                string extraChanceValue = $"{cropInfo.Crop.ChanceForExtraCrops * 100}%";

                currentTextPosition.Y += font.MeasureString(extraChance).Y;

                b.DrawString(
                    font,
                    extraChance,
                    new Vector2(
                        currentTextPosition.X,
                        currentTextPosition.Y
                    ),
                    Color.Black,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0.75f
                );
                b.DrawString(
                    font,
                    extraChanceValue,
                    new Vector2(
                        currentTextPosition.Z - font.MeasureString(extraChanceValue).X,
                        currentTextPosition.Y),
                    Color.Black,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0.75f
                );
            }

            #endregion Crop Value

            #region Probabilities

            currentTextPosition.Y += 8;

            string chanceOf = $"{Utils.Helper.Translation.Get("value-normal")}:";
            string chanceOfValue = $"{(cropInfo.ChanceOfNormalQuality * 100):0.00}%";
            if (cropInfo.ChanceOfNormalQuality != 0)
            {
                currentTextPosition.Y += font.MeasureString(chanceOf).Y;

                b.DrawString(
                    font,
                    chanceOf,
                    new Vector2(
                        currentTextPosition.X,
                        currentTextPosition.Y
                    ),
                    Color.Black,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0.75f
                );
                b.DrawString(
                    font,
                    chanceOfValue,
                    new Vector2(
                        currentTextPosition.Z - font.MeasureString(chanceOfValue).X,
                        currentTextPosition.Y),
                    Color.Black,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0.75f
                );
            }
            chanceOf = $"{Utils.Helper.Translation.Get("value-silver")}:";
            chanceOfValue = $"{(cropInfo.ChanceOfSilverQuality * 100):0.00}%";

            currentTextPosition.Y += font.MeasureString(chanceOf).Y;

            b.DrawString(
                font,
                chanceOf,
                new Vector2(
                    currentTextPosition.X,
                    currentTextPosition.Y
                ),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );
            b.DrawString(
                font,
                chanceOfValue,
                new Vector2(
                    currentTextPosition.Z - font.MeasureString(chanceOfValue).X,
                    currentTextPosition.Y),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );
            chanceOf = $"{Utils.Helper.Translation.Get("value-gold")}:";
            chanceOfValue = $"{(cropInfo.ChanceOfGoldQuality * 100):0.00}%";

            currentTextPosition.Y += font.MeasureString(chanceOf).Y;

            b.DrawString(
                font,
                chanceOf,
                new Vector2(
                    currentTextPosition.X,
                    currentTextPosition.Y
                ),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );
            b.DrawString(
                font,
                chanceOfValue,
                new Vector2(
                    currentTextPosition.Z - font.MeasureString(chanceOfValue).X,
                    currentTextPosition.Y),
                Color.Black,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.75f
            );
            chanceOf = $"{Utils.Helper.Translation.Get("value-iridium")}:";
            chanceOfValue = $"{(cropInfo.ChanceOfIridiumQuality * 100):0.0}%";

            if (cropInfo.ChanceOfIridiumQuality != 0)
            {
                currentTextPosition.Y += font.MeasureString(chanceOf).Y;

                b.DrawString(
                    font,
                    chanceOf,
                    new Vector2(
                        currentTextPosition.X,
                        currentTextPosition.Y
                    ),
                    Color.Black,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0.75f
                );
                b.DrawString(
                    font,
                    chanceOfValue,
                    new Vector2(
                        currentTextPosition.Z - font.MeasureString(chanceOfValue).X,
                        currentTextPosition.Y),
                    Color.Black,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0.75f
                );
            }

            #endregion Probabilities
        }

        /// <inheritdoc/>

        public void Reset()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>

        public void Update()
        {
            //x and y set to near the mouse
            //if mouse is near the edge of the screen, move the box to the other side of the mouse
            Rectangle safeArea = Utility.getSafeArea();

            int mouseX = Game1.getMouseX() + Game1.tileSize;
            int mouseY = Game1.getMouseY();

            if (mouseX + windowWidth > safeArea.Right)
                x = mouseX - windowWidth;
            else
                x = mouseX;

            if (mouseY + windowHeight > safeArea.Bottom)
                y = mouseY - windowHeight;
            else
                y = mouseY;

            //if the box is off the screen, move it back on
            if (x < safeArea.Left)
                x = safeArea.Left + Game1.tileSize / 4;
            if (y < safeArea.Top)
                y = safeArea.Top + Game1.tileSize / 4;

            drawBox = new(
                x,
                y,
                windowWidth,
                windowHeight
            );
        }

        /// <inheritdoc/>

        public void GameWindowSizeChanged()
        {
        }

        /// <summary>
        ///  Opens or closes the hover box.
        /// </summary>
        /// <param name="open"> Whether to open or close the box.</param>
        public void Open(bool open = false)
        {
            isOpen = open;
            if (!open) hoverDelay = hoverDelayDefault;
        }
    }
}