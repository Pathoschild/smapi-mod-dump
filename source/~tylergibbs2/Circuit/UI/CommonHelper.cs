/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using Microsoft.Xna.Framework;

// Code from Pathoschild, MIT license
// https://github.com/Pathoschild/StardewMods

namespace Circuit.UI
{
    internal class CommonHelper
    {
        /// <summary>The width of the borders drawn by <see cref="DrawTab"/>.</summary>
        public const int ButtonBorderWidth = 4 * Game1.pixelZoom;

        /// <summary>Draw a tab texture to the screen.</summary>
        /// <param name="spriteBatch">The sprite batch to which to draw.</param>
        /// <param name="x">The X position at which to draw.</param>
        /// <param name="y">The Y position at which to draw.</param>
        /// <param name="innerWidth">The width of the button's inner content.</param>
        /// <param name="innerHeight">The height of the button's inner content.</param>
        /// <param name="innerDrawPosition">The position at which the content should be drawn.</param>
        /// <param name="align">The button's horizontal alignment relative to <paramref name="x"/>. The possible values are 0 (left), 1 (center), or 2 (right).</param>
        /// <param name="alpha">The button opacity, as a value from 0 (transparent) to 1 (opaque).</param>
        /// <param name="forIcon">Whether the button will contain an icon instead of text.</param>
        /// <param name="drawShadow">Whether to draw a shadow under the tab.</param>
        public static void DrawTab(SpriteBatch spriteBatch, int x, int y, int innerWidth, int innerHeight, out Vector2 innerDrawPosition, int align = 0, float alpha = 1, bool forIcon = false, bool drawShadow = true)
        {
            // calculate outer coordinates
            int outerWidth = innerWidth + CommonHelper.ButtonBorderWidth * 2;
            int outerHeight = innerHeight + Game1.tileSize / 3;
            int offsetX = align switch
            {
                1 => -outerWidth / 2,
                2 => -outerWidth,
                _ => 0
            };

            // calculate inner coordinates
            {
                int iconOffsetX = forIcon ? -Game1.pixelZoom : 0;
                int iconOffsetY = forIcon ? 2 * -Game1.pixelZoom : 0;
                innerDrawPosition = new Vector2(x + CommonHelper.ButtonBorderWidth + offsetX + iconOffsetX, y + CommonHelper.ButtonBorderWidth + iconOffsetY);
            }

            // draw texture
            IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x + offsetX, y, outerWidth, outerHeight + Game1.tileSize / 16, Color.White * alpha, drawShadow: drawShadow);
        }
    }
}
