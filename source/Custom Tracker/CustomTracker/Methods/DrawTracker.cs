using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CustomTracker
{
    /// <summary>The mod's main class.</summary>
    public partial class ModEntry : Mod
    {




        /// <summary>Draw a tracker pointing to the provided tile of the player's current location.</summary>
        /// <param name="targetTile">The coordinates of the tile this tracker icon should point toward.</param>
        /// <remarks>
        /// This method imitates code from Stardew's Game1.drawHUD method.
        /// It draws a rotated, rescaled tracker icon to Game1.spriteBatch at the edge of the player's screen.
        /// </remarks>
        public void DrawTracker(Vector2 targetTile)
        {
            if (Utility.isOnScreen(targetTile * 64f + new Vector2(32f, 32f), 64)) //if the target tile is on the player's screen
                return; //do not track it

            float scale = MConfig.TrackerPixelScale; //get the intended scale of the sprite
            Rectangle bounds = Game1.graphics.GraphicsDevice.Viewport.Bounds; //get the boundaries of the screen

            //define relative minimum and maximum sprite positions
            float minX = 8f;
            float minY = 8f;
            float maxX = bounds.Right - 8;
            float maxY = bounds.Bottom - 8;

            Vector2 renderSize = new Vector2((float)SpriteSource.Width * scale, (float)SpriteSource.Height * scale); //get the render size of the sprite

            Vector2 trackerRenderPosition = new Vector2();
            float rotation = 0.0f;

            Vector2 centerOfObject = new Vector2((targetTile.X * 64) + 32, (targetTile.Y * 64) + 32); //get the center pixel of the object
            Vector2 targetPixel = new Vector2(centerOfObject.X - (renderSize.X / 2), centerOfObject.Y - (renderSize.Y / 2)); //get the top left pixel of the custom tracker's "intended" location

            if (targetPixel.X > (double)(Game1.viewport.MaxCorner.X - 64)) //if the object is RIGHT of the screen
            {
                trackerRenderPosition.X = maxX; //use the predefined max X
                rotation = 1.570796f;
                targetPixel.Y = centerOfObject.Y - (renderSize.X / 2); //adjust Y for rotation
            }
            else if (targetPixel.X < (double)Game1.viewport.X) //if the object is LEFT of the screen
            {
                trackerRenderPosition.X = minX; //use the predefined min X
                rotation = -1.570796f;
                targetPixel.Y = centerOfObject.Y + (renderSize.X / 2); //adjust Y for rotation
            }
            else
                trackerRenderPosition.X = targetPixel.X - (float)Game1.viewport.X; //use the target X (adjusted for viewport)

            if (targetPixel.Y > (double)(Game1.viewport.MaxCorner.Y - 64)) //if the object is DOWN from the screen
            {
                trackerRenderPosition.Y = maxY; //use the predefined max Y
                rotation = 3.141593f;
                if (trackerRenderPosition.X > minX) //if X is NOT min (i.e. this is NOT the bottom left corner)
                {
                    trackerRenderPosition.X = Math.Min(centerOfObject.X + (renderSize.X / 2) - (float)Game1.viewport.X, maxX); //adjust X for rotation (using renderPos, clamping to maxX, and adjusting for viewport)
                }
            }
            else
            {
                trackerRenderPosition.Y = targetPixel.Y >= (double)Game1.viewport.Y ? targetPixel.Y - (float)Game1.viewport.Y : minY; //if the object is UP from the screen, use the predefined min Y; otherwise, use the target Y (adjusted for viewport)
            }

            if (trackerRenderPosition.X == minX && trackerRenderPosition.Y == minY) //if X and Y are min (TOP LEFT corner)
            {
                trackerRenderPosition.Y += SpriteSource.Height; //adjust DOWN based on sprite size
                rotation += 0.7853982f;
            }
            else if (trackerRenderPosition.X == minX && trackerRenderPosition.Y == maxY) //if X is min and Y is max (BOTTOM LEFT corner)
            {
                trackerRenderPosition.X += SpriteSource.Width; //adjust RIGHT based on sprite size
                rotation += 0.7853982f;
            }
            else if (trackerRenderPosition.X == maxX && trackerRenderPosition.Y == minY) //if X is max and Y is min (TOP RIGHT corner)
            {
                trackerRenderPosition.X -= SpriteSource.Width; //adjust LEFT based on sprite size
                rotation -= 0.7853982f;
            }
            else if (trackerRenderPosition.X == maxX && trackerRenderPosition.Y == maxY) //if X and Y are max (BOTTOM RIGHT corner)
            {
                trackerRenderPosition.Y -= SpriteSource.Height; //adjust UP based on sprite size
                rotation -= 0.7853982f;
            }

            if (Background != null) //if a background was successfully loaded
            {
                Game1.spriteBatch.Draw(Background, trackerRenderPosition, new Rectangle?(BackgroundSource), Color.White, rotation, new Vector2(2f, 2f), scale, SpriteEffects.None, 1f); //draw the background on the game's main sprite batch (note: this will be off-center if spritesheet and background are different sizes)
            }

            Game1.spriteBatch.Draw(Spritesheet, trackerRenderPosition, new Rectangle?(SpriteSource), Color.White, rotation, new Vector2(2f, 2f), scale, SpriteEffects.None, 1f); //draw the spritesheet on the game's main sprite batch
        }
    }
}
