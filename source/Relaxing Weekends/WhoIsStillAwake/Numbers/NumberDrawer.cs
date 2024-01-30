/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-ModCollection
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using WhoIsStillAwakeMod.Helpers;

namespace WhoIsStillAwakeMod.Numbers
{
    public static class NumberDrawer
    {
        /// <summary>
        /// Draws the number of farmers in bed vs not in bed
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw on</param>
        public static void DrawNumberOfFarmersInBed(SpriteBatch spriteBatch)
        {
            try
            {
                Vector2 topLeftDestination = GetDrawDestination();
                Color timeColor = GeneralUtils.ConvertHexToColor(WhoIsStillAwake.Config.HexColor);
                string formattedFarmersInBed = GeneralUtils.GetRatioOfFarmersInBed();
                DrawTextAtDestination(spriteBatch, topLeftDestination, timeColor, formattedFarmersInBed);
            }
            catch (Exception e)
            {
                WhoIsStillAwake.Logger.Log("Error drawing time:" + e.ToString());
            }
        }

        /// <summary>
        /// Draws the time in a given destination
        /// </summary>
        /// <param name="spriteBatch">Sprite batch to draw on</param>
        /// <param name="destination">Location to draw the time in</param>
        /// <param name="color">Color of the font to use</param>
        private static void DrawTextAtDestination(SpriteBatch spriteBatch, Vector2 destination, Color color, string textToDraw)
        {
            spriteBatch.DrawString(spriteFont: Game1.smallFont,
                                         text: textToDraw,
                                     position: destination,
                                        color: color,
                                     rotation: 0f,
                                       origin: Vector2.Zero,
                                        scale: 1f * Game1.options.zoomLevel,
                                      effects: SpriteEffects.None,
                                   layerDepth: 0f);
        }

        /// <summary>
        /// Gets the location of a corner to draw the time in
        /// </summary>
        /// <param name="corner">The corner of the screen to draw the time in</param>
        /// <returns>The calculated destination the time will be drawn at</returns>
        private static Vector2 GetDrawDestination()
        {
            // User configured about to indent
            float indentation = WhoIsStillAwake.Config.Indentation;

            // The string measurements only align with the rectangle, and not the pixels inside
            //     This means we have to adjust the target coordinates with the adjusted offsets
            //     The string itself starts 5 pixels below, and 1 pixel to the right of where the rectangle starts
            float topOffset = indentation - Constants.TopOffset;
            float leftOffset = indentation - Constants.LeftOffset;

            return new Vector2(leftOffset, topOffset); ;
        }
    }
}
