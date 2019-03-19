using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using SObject = StardewValley.Object;

namespace QualityProducts.Processors
{
    internal class Loom : Processor
    {
        /****************
         * Public methods
         ****************/

        public Loom() : base(ProcessorType.LOOM)
        {
        }

        /***
         * From StardewValley.Object.minutesElapsed
         **/
        /// <summary>
        /// Minutes elapsed.
        /// </summary>
        /// <returns><c>false</c></returns>
        /// <param name="minutes">Minutes.</param>
        /// <param name="environment">Environment.</param>
        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            if (heldObject.Value != null)
            {
                if (Game1.IsMasterGame)
                {
                    MinutesUntilReady -= minutes;
                }
                if (MinutesUntilReady <= 0)
                {
                    if (!(bool)readyForHarvest)
                    {
                        environment.playSound("dwop");
                    }

                    readyForHarvest.Value = true;
                    MinutesUntilReady = 0;
                    showNextIndex.Value = true;

                    if (lightSource != null)
                    {
                        environment.removeLightSource(lightSource.identifier);
                        lightSource = null;
                    }
                }
                if (!(bool)readyForHarvest && Game1.random.NextDouble() < 0.33)
                {
                    addWorkingAnimation(environment);
                }
            }

            return false;
        }

        /***
         * From StardewValley.Object.getScale
         ***/
        /// <summary>
        /// Gets the scale.
        /// </summary>
        /// <returns>The scale.</returns>
        private new Vector2 getScale()
        {
            if ((heldObject.Value != null || (int)minutesUntilReady > 0) && !(bool)readyForHarvest)
            {
                scale.X = (float)((scale.X + 0.04f) % 6.2831853071795862);
            }
            return Vector2.Zero;
        }

        /***
         * From StardewValley.Object.drawAsProp
         **/
        /// <summary>
        /// Draw prop in sprite batch.
        /// </summary>
        /// <param name="b">The SpriteBatch to draw to.</param>
        public override void drawAsProp(SpriteBatch b)
        {
            int x = (int)tileLocation.X;
            int y = (int)tileLocation.Y;

            Vector2 value = 4f * getScale();

            Vector2 vector = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));

            Rectangle destinationRectangle = new Rectangle(
                (int)(vector.X - value.X / 2f),
                (int)(vector.Y - value.Y / 2f),
                (int)(64f + value.X),
                (int)(128f + value.Y / 2f)
                );

            b.Draw(
                Game1.bigCraftableSpriteSheet,
                destinationRectangle,
                getSourceRectForBigCraftable(((bool)showNextIndex)
                    ? (ParentSheetIndex + 1) : ParentSheetIndex),
                Color.White, 0f, Vector2.Zero, SpriteEffects.None,
                Math.Max(0f, ((y + 1) * 64 - 1) / 10000f)
                );

            if (MinutesUntilReady > 0)
            {
                b.Draw(
                    Game1.objectSpriteSheet,
                    getLocalPosition(Game1.viewport) + new Vector2(32f, 0f),
                    Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 435, -1, -1),
                    Color.White, scale.X, new Vector2(32f, 32f), 1f, SpriteEffects.None,
                    Math.Max(0f, (((TileLocation.Y + 1) * 64) - 1) / 10000f + 0.0001f)
                    );
            }
        }

        /***
         * From StardewValley.Object.draw
         ***/
        /// <summary>
        /// Draw this instance with transparency alpha at x, y in the specified sprite batch.
        /// </summary>
        /// <param name="spriteBatch">Sprite batch.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="alpha">Alpha.</param>
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            Vector2 value = 4f * getScale();

            Vector2 vector = Game1.GlobalToLocal(
                Game1.viewport, new Vector2(x * 64, (y * 64) - 64)
                );

            Rectangle destinationRectangle = new Rectangle(
                (int)(vector.X - (value.X / 2f)) + ((shakeTimer > 0)
                    ? Game1.random.Next(-1, 2) : 0),
                (int)(vector.Y - (value.Y / 2f)) + ((shakeTimer > 0)
                    ? Game1.random.Next(-1, 2) : 0),
                (int)(64f + value.X),
                (int)(128f + (value.Y / 2f))
                );

            spriteBatch.Draw(
                Game1.bigCraftableSpriteSheet,
                destinationRectangle,
                getSourceRectForBigCraftable(
                    ((bool)showNextIndex) ? (ParentSheetIndex + 1) : ParentSheetIndex
                    ),
                Color.White * alpha, 0f, Vector2.Zero, SpriteEffects.None,
                Math.Max(0f, (((y + 1) * 64) - 24) / 10000f) + (x * 1E-05f)
                );

            if (MinutesUntilReady > 0)
            {
                spriteBatch.Draw(
                    Game1.objectSpriteSheet,
                    getLocalPosition(Game1.viewport) + new Vector2(32f, 0f),
                    Game1.getSourceRectForStandardTileSheet(
                        Game1.objectSpriteSheet, 435, 16, 16
                        ),
                    Color.White * alpha, scale.X, new Vector2(8f, 8f), 4f, SpriteEffects.None, 
                    Math.Max(0f, ((y + 1) * 64 / 10000f) + 0.0001f + (x * 1E-05f))
                );
            }

            if ((bool)readyForHarvest)
            {
                TimeSpan timeSpan = DateTime.UtcNow.TimeOfDay;
                float num6 = 4f * (float)Math.Round(
                    Math.Sin(timeSpan.TotalMilliseconds / 250.0), 2
                    );

                spriteBatch.Draw(
                    Game1.mouseCursors,
                    Game1.GlobalToLocal(
                        Game1.viewport,
                        new Vector2((x * 64) - 8, (y * 64) - 96 - 16 + num6)
                        ),
                    new Rectangle(141, 465, 20, 24),
                    Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None,
                    ((y + 1) * 64 / 10000f) + 1E-06f + (TileLocation.X / 10000f)
                    );

                if (heldObject.Value != null)
                {
                    spriteBatch.Draw(
                        Game1.objectSpriteSheet,
                        Game1.GlobalToLocal(
                            Game1.viewport,
                            new Vector2((x * 64) + 32, (y * 64) - 64 - 8 + num6)
                            ),
                        Game1.getSourceRectForStandardTileSheet(
                            Game1.objectSpriteSheet,
                            heldObject.Value.parentSheetIndex,
                            16, 16
                            ),
                        Color.White * 0.75f, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None,
                        ((y + 1) * 64 / 10000f) + 1E-05f + (TileLocation.X / 10000f)
                        );
                }
            }
        }

        /***
         * From StardewValley.Object.draw
         ***/
        /// <summary>
        /// Draw this instance with transparency alpha at x, y in the specified layer depth of the specified sprite batch
        /// </summary>
        /// <param name="spriteBatch">Sprite batch to draw to.</param>
        /// <param name="xNonTile">X (not tile coordinates).</param>
        /// <param name="yNonTile">Y (not tile coordinates).</param>
        /// <param name="layerDepth">Layer depth.</param>
        /// <param name="alpha">Alpha.</param>
        public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1f)
        {
            Vector2 value = 4f * getScale();

            Vector2 vector = Game1.GlobalToLocal(Game1.viewport, new Vector2(xNonTile, yNonTile));

            Rectangle destinationRectangle = new Rectangle(
                (int)(vector.X - value.X / 2f) + ((shakeTimer > 0)
                    ? Game1.random.Next(-1, 2) : 0),
                (int)(vector.Y - value.Y / 2f) + ((shakeTimer > 0)
                    ? Game1.random.Next(-1, 2) : 0),
                (int)(64f + value.X),
                (int)(128f + value.Y / 2f)
                );

            spriteBatch.Draw(
                Game1.bigCraftableSpriteSheet,
                destinationRectangle,
                getSourceRectForBigCraftable(((bool)showNextIndex)
                    ? (ParentSheetIndex + 1) : ParentSheetIndex
                    ),
                Color.White * alpha, 0f, Vector2.Zero, SpriteEffects.None,
                layerDepth
                );

            if (MinutesUntilReady > 0)
            {
                spriteBatch.Draw(
                    Game1.objectSpriteSheet,
                    Game1.GlobalToLocal(vector) + new Vector2(32f, 0f),
                    Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 435, 16, 16),
                    Color.White * alpha, scale.X, new Vector2(8f, 8f), 4f, SpriteEffects.None,
                    layerDepth
                    );
            }
        }


        /*******************
         * Protected methods
         *******************/

        /***
         * From StardewValley.Object.performObjectDropInAction
         ***/
        /// <summary>
        /// Performs item processing.
        /// </summary>
        /// <returns><c>true</c> if started processing, <c>false</c> otherwise.</returns>
        /// <param name="object">Object to be processed.</param>
        /// <param name="probe">If set to <c>true</c> probe.</param>
        /// <param name="who">Farmer that initiated processing.</param>
        protected override bool PerformProcessing(SObject @object, bool probe, Farmer who)
        {
            if (@object.ParentSheetIndex == 440)
            {
                heldObject.Value = new SObject(Vector2.Zero, 428, null, false, true, false, false);
                if (!probe)
                {
                    minutesUntilReady.Value = 240;
                    who.currentLocation.playSound("Ship");
                }
                return true;
            }
            return false;
        }
    }
}
