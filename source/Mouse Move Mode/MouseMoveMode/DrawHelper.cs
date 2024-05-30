/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ylsama/RightClickMoveMode
**
*************************************************/

using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MouseMoveMode
{
    class DrawHelper
    {
        /**
         * @brief Draw a red box in the wanted game location
         */
        public static void drawRedBox(SpriteBatch b, Rectangle boxPosistion)
        {
            var color = Color.Red;
            DrawHelper._drawBox(b, boxPosistion, color, textureRow: 210);
        }

        /**
         * @brief Draw a mini green box point in the wanted location
         */
        public static void drawBox(SpriteBatch b, Vector2 position)
        {
            var color = Color.White;
            DrawHelper._drawBox(b, new Rectangle((int)position.X, (int)position.Y, 32, 32), color);
        }

        /**
         * @brief Draw a mini box point in the wanted location with custom color
         *
         * @param b can access via Event Rendered input `e.SpriteBatch`
         * @param posistion X,Y point to be draw on screen
         */
        public static void drawBox(SpriteBatch b, Vector2 posistion, Color color)
        {
            var position = Game1.GlobalToLocal(Game1.viewport, posistion);
            DrawHelper._drawBox(b, new Rectangle((int)position.X, (int)position.Y, 32, 32), color);
        }


        /**
         * @brief This allow drawing box exactly along the rectangle position
         *
         * @param b 
         * @param boxPosistion 
         */
        public static void drawBox(SpriteBatch b, Rectangle boxPosistion)
        {
            var color = Color.White;
            DrawHelper._drawBox(b, boxPosistion, color);
        }

        /**
         * @brief Draw a box in the specified on current Game map location
         *
         * @param b can access via Event Rendered input `e.SpriteBatch`
         */
        public static void drawBox(SpriteBatch b, Rectangle boxPosistion, Color color)
        {
            DrawHelper._drawBox(b, boxPosistion, color);
        }

        /**
         * @brief Draw a box in the specified on current Game map location
         *
         * @param b can access via Event Rendered input `e.SpriteBatch`
         * @param posistion to be draw on screen
         * @param color (default will be green) be draw on screen
         * @param texture (default will be green box) texture index be draw on screen
         */
        private static void _drawBox(SpriteBatch b, Rectangle boxPosistion, Color color, int textureRow = 194, int textureCol = 388, int textureSize = 16)
        {
            // This have full texture2D
            var texture2D = Game1.mouseCursors;
            var position = Game1.GlobalToLocal(Game1.viewport, new Vector2(boxPosistion.X, boxPosistion.Y));
            // The size of rectangle that will contain the sprite texture for green tilte appreared when you
            // powering the tool
            var sourceRectangle = new Rectangle(textureRow, textureCol, textureSize, textureSize);
            var rotation = 0f;
            // Start at top-left
            var origin = new Vector2(0f, 0f);
            var scale = new Vector2(boxPosistion.Width / 64f * 4f * 16f / textureSize, boxPosistion.Height / 64f * 4f * 16f / textureSize);
            var effect = SpriteEffects.None;
            // Same layer with tool effective range indicator (green tilte appeared when you powering the tool)
            var layerDepth = 0.01f;

            b.Draw(texture2D, position, sourceRectangle, color, rotation, origin, scale, effect, layerDepth);
        }

        public static void drawCursorHelper(SpriteBatch b, Rectangle boxPosistion)
        {
            var box = Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 29);
            _drawBox(b, boxPosistion, Color.White, textureRow: box.X, textureCol: box.Y, textureSize: box.Width);
        }
    }
}
