/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/CustomTracker
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;

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

            float scale = Config.TrackerPixelScale; //get the intended scale of the sprite
            Rectangle bounds = Game1.graphics.GraphicsDevice.Viewport.Bounds; //get the boundaries of the screen

            //define relative minimum and maximum sprite positions
            float minX = 8f;
            float minY = 8f;
            float maxX = bounds.Right - 8;
            float maxY = bounds.Bottom - 8;

            Vector2 renderSize = new Vector2((float)SpriteSource.Width * scale, (float)SpriteSource.Height * scale); //get the render size of the sprite

            Vector2 centerOfObject = new Vector2((targetTile.X * 64) + 32, (targetTile.Y * 64) + 32); //get the center pixel of the object
            Vector2 targetPixel = new Vector2(centerOfObject.X - (renderSize.X / 2), centerOfObject.Y - (renderSize.Y / 2)); //get the top left pixel of the custom tracker's "intended" location

            Vector2 trackerRenderPosition = Game1.GlobalToLocal(Game1.viewport, targetPixel); //get the target pixel's position relative to the viewport
            trackerRenderPosition = Utility.ModifyCoordinatesForUIScale(trackerRenderPosition); //adjust for UI scaling and/or zoom
            trackerRenderPosition.X = Utility.Clamp(trackerRenderPosition.X, minX, maxX); //limit X to min/max
            trackerRenderPosition.Y = Utility.Clamp(trackerRenderPosition.Y, minY, maxY); //limit Y to min/max

            //define offsets to adjust for rotation 
            float offsetX = 0;
            float offsetY = 0;

            float rotation = 0f; //the rotation of the tracker sprite

            if (trackerRenderPosition.X == minX) //if the tracker is on the LEFT
            {
                if (trackerRenderPosition.Y == minY) //if the tracker is on the TOP LEFT
                {
                    offsetY = renderSize.X / 2f; //offset down by 1/2 sprite width
                    rotation = (float)Math.PI * 1.75f; //315 degrees
                }
                else if (trackerRenderPosition.Y == maxY) //if the tracker is on the BOTTOM LEFT
                {
                    offsetX = renderSize.X / 2f; //offset right by 1/2 sprite width
                    offsetY = renderSize.X; //offset down by 1 sprite width
                    rotation = (float)Math.PI * 1.25f; //225 degrees
                }
                else
                {
                    offsetY = renderSize.X; //offset down by 1 sprite width
                    rotation = (float)Math.PI * 1.5f; //270 degrees
                }
            }
            else if (trackerRenderPosition.X == maxX) //if the tracker is on the RIGHT
            {
                if (trackerRenderPosition.Y == minY) //if the tracker is on the TOP RIGHT
                {
                    offsetX = -renderSize.X / 2f; //offset left by 1/2 sprite width
                    rotation = (float)Math.PI * 0.25f; //45 degrees
                }
                else if (trackerRenderPosition.Y == maxY) //if the tracker is on the BOTTOM RIGHT
                {
                    offsetX = renderSize.X; //offset right by 1 sprite width
                    offsetY = -renderSize.X / 2f; //offset up by 1/2 sprite width
                    rotation = (float)Math.PI * 0.75f; //135 degrees
                }
                else
                {
                    rotation = (float)Math.PI * 0.5f; //90 degrees
                }
            }
            else if (trackerRenderPosition.Y == maxY) //if the tracker is on the BOTTOM
            {
                offsetX = renderSize.X; //offset right by 1 sprite width
                rotation = (float)Math.PI; //180 degrees
            }

            trackerRenderPosition.X = Utility.Clamp(trackerRenderPosition.X + offsetX, minX, maxX); //add offset to X (limited to min/max)
            trackerRenderPosition.Y = Utility.Clamp(trackerRenderPosition.Y + offsetY, minY, maxY); //add offset to Y (limited to min/max)

            if (ForageIconMode && Background != null) //if a background should be drawn
            {
                Game1.spriteBatch.Draw(Background, trackerRenderPosition, new Rectangle?(BackgroundSource), Color.White, rotation, new Vector2(2f, 2f), scale, SpriteEffects.None, 1f); //draw the background on the game's main sprite batch (note: this will be off-center if spritesheet and background are different sizes)
            }

            Game1.spriteBatch.Draw(Spritesheet, trackerRenderPosition, new Rectangle?(SpriteSource), Color.White, rotation, new Vector2(2f, 2f), scale, SpriteEffects.None, 1f); //draw the spritesheet on the game's main sprite batch
        }
    }
}
