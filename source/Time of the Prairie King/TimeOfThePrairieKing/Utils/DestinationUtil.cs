using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Minigames;
using TimeOfThePrairieKingMod.Config;
using static TimeOfThePrairieKingMod.Utils.TimeDrawUtil;

namespace TimeOfThePrairieKingMod.Utils
{
    /// <summary>
    /// Gets the destinations to draw the time of day within the Journey of the Prairie King minigame
    /// </summary>
    public static class DestinationUtil
    {
        /// <summary>
        /// Gets the location to draw the time in the minigame's HUD
        /// </summary>
        /// <returns>The destination on the minigame's HUD to draw to</returns>
        public static Vector2 GetHudDestination()
        {
            Vector2 topLeftGameCorner = AbigailGame.topLeftScreenCoordinate;
            float stringLength = Game1.dialogueFont.MeasureString(Game1.getTimeOfDayString(Game1.timeOfDay)).X;

            return topLeftGameCorner - new Vector2(stringLength * 0.75f, -160);
        }

        /// <summary>
        /// Gets the location of a corner to draw the time in
        /// </summary>
        /// <param name="corner">The corner of the screen to draw the time in</param>
        /// <returns>The calculated destination the time will be drawn at</returns>
        public static Vector2 GetCornerDestination(ScreenCorner corner)
        {
            // Corner to draw
            Vector2 destination = new Vector2();

            // User configured about to indent
            float indentation = TimeOfThePrairieKing.Config.Indentation;

            // Resolution zoom value from user's options menu
            float zoomAdjustment = 1f / Game1.options.zoomLevel;

            // Actual size of the game window itself
            int viewportWidth = Game1.graphics.GraphicsDevice.Viewport.Width;
            int viewportHeight = Game1.graphics.GraphicsDevice.Viewport.Height;

            // Dimensions of string, adjusted to hit the actual pixel size b/c it doesn't measure the string properly
            float stringWidth = Game1.dialogueFont.MeasureString(Game1.getTimeOfDayString(Game1.timeOfDay)).X;
            float stringHeight = Game1.dialogueFont.MeasureString(Game1.getTimeOfDayString(Game1.timeOfDay)).Y;

            // The string measurements only align with the rectangle, and not the pixels inside
            //     This means we have to adjust the target coordinates with the adjusted offsets
            //     The string itself starts 5 pixels below, and 1 pixel to the right of where the rectangle starts
            float topOffset = indentation - FontConstants.TopOffset;
            float leftOffset = indentation - FontConstants.LeftOffset;

            // We have to perform subtraction from the viewport endings to get back on screen
            float rightOffset = viewportWidth - indentation - (stringWidth - FontConstants.RightOffset);
            float bottomOffset = viewportHeight - indentation - (stringHeight - FontConstants.BottomOffset);

            // Based on the corner specified, we will need to draw accordingly
            switch (corner)
            {
                case ScreenCorner.TopLeft:
                    return new Vector2(leftOffset, topOffset);
                case ScreenCorner.TopRight:
                    return new Vector2(rightOffset, topOffset);
                case ScreenCorner.BottomLeft:
                    return new Vector2(leftOffset, bottomOffset);
                case ScreenCorner.BottomRight:
                    return new Vector2(rightOffset, bottomOffset);
            }

            return destination;
        }
    }
}
