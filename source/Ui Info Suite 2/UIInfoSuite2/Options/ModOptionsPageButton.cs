/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UiInfoSuite2
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace UIInfoSuite2.Options
{
    internal class ModOptionsPageButton
    {
        public int xPositionOnScreen;
        public int yPositionOnScreen;

        public void draw(SpriteBatch b)
        {
            Game1.spriteBatch.Draw(Game1.mouseCursors,
                new Vector2(xPositionOnScreen, yPositionOnScreen),
                new Rectangle(16, 368, 16, 16),
                Color.White,
                0.0f,
                Vector2.Zero,
                Game1.pixelZoom,
                SpriteEffects.None,
                1f);

            b.Draw(Game1.mouseCursors,
                new Vector2(xPositionOnScreen + 8, yPositionOnScreen + 14),
                new Rectangle(32, 672, 16, 16),
                Color.White,
                0.0f,
                Vector2.Zero,
                3f,
                SpriteEffects.None,
                1f);
        }
    }
}
