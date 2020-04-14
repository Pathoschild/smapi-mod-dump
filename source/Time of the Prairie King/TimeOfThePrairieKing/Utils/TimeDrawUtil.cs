using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TimeOfThePrairieKingMod.Utils
{
    /// <summary>
    /// Utility to draw the in-game time on the screen
    /// </summary>
    public static class TimeDrawUtil
    {
        /// <summary>
        /// Possible screen corners to draw the time at
        /// </summary>
        public enum ScreenCorner
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        /// <summary>
        /// Draws the in-game time at a configured location on the screen
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw on</param>
        public static void DrawTime(SpriteBatch spriteBatch)
        {
            try
            {
                if (TimeOfThePrairieKing.Config.MinigameHud.Show)
                {
                    Vector2 hudDestination = DestinationUtil.GetHudDestination();
                    hudDestination = hudDestination * Game1.options.zoomLevel;
                    Color timeColor = GetColorFromHex(TimeOfThePrairieKing.Config.MinigameHud.HexColor);
                    DrawTimeAtDestination(spriteBatch, hudDestination, timeColor);
                }

                if (TimeOfThePrairieKing.Config.TopLeft.Show)
                {
                    Vector2 topLeftDestination = DestinationUtil.GetCornerDestination(ScreenCorner.TopLeft);
                    Color timeColor = GetColorFromHex(TimeOfThePrairieKing.Config.TopLeft.HexColor);
                    DrawTimeAtDestination(spriteBatch, topLeftDestination, timeColor);
                }

                if (TimeOfThePrairieKing.Config.TopRight.Show)
                {
                    Vector2 topRightDestination = DestinationUtil.GetCornerDestination(ScreenCorner.TopRight);
                    Color timeColor = GetColorFromHex(TimeOfThePrairieKing.Config.TopRight.HexColor);
                    DrawTimeAtDestination(spriteBatch, topRightDestination, timeColor);
                }

                if (TimeOfThePrairieKing.Config.BottomLeft.Show)
                {
                    Vector2 bottomLeftDestination = DestinationUtil.GetCornerDestination(ScreenCorner.BottomLeft);
                    Color timeColor = GetColorFromHex(TimeOfThePrairieKing.Config.BottomLeft.HexColor);
                    DrawTimeAtDestination(spriteBatch, bottomLeftDestination, timeColor);
                }

                if (TimeOfThePrairieKing.Config.BottomRight.Show)
                {
                    Vector2 bottomRightDestination = DestinationUtil.GetCornerDestination(ScreenCorner.BottomRight);
                    Color timeColor = GetColorFromHex(TimeOfThePrairieKing.Config.BottomRight.HexColor);
                    DrawTimeAtDestination(spriteBatch, bottomRightDestination, timeColor);
                }
            }
            catch (Exception e)
            {
                TimeOfThePrairieKing.Logger.Log("Error drawing time:" + e.ToString());
            }
        }

        /// <summary>
        /// Draws the time in a given destination
        /// </summary>
        /// <param name="spriteBatch">Sprite batch to draw on</param>
        /// <param name="destination">Location to draw the time in</param>
        /// <param name="color">Color of the font to use</param>
        private static void DrawTimeAtDestination(SpriteBatch spriteBatch, Vector2 destination, Color color)
        {
            spriteBatch.DrawString(spriteFont: Game1.smallFont,
                                         text: Game1.getTimeOfDayString(Game1.timeOfDay),
                                     position: destination,
                                        color: color,
                                     rotation: 0f,
                                       origin: Vector2.Zero,
                                        scale: 1f * Game1.options.zoomLevel,
                                      effects: SpriteEffects.None,
                                   layerDepth: 0f);
        }

        /// <summary>
        /// Converts a hex color string into an XNA Framework Color object
        /// </summary>
        /// <param name="hexColor">String of a hex color</param>
        /// <returns>A new Color object</returns>
        private static Color GetColorFromHex(string hexColor)
        {
            Regex hexValidator = new Regex("^#?(?:[0-9a-fA-F]{3}){2}$");

            if (!hexValidator.IsMatch(hexColor))
                return Color.Gray;

            if (hexColor.StartsWith("#"))
                hexColor = hexColor.Substring(1);


            int r = int.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier);
            int g = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
            int b = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);

            return new Color(r, g, b);
        }
    }
}
