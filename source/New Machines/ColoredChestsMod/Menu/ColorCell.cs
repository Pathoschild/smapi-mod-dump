using System;
using Igorious.StardewValley.ColoredChestsMod.Utils;
using Igorious.StardewValley.DynamicAPI.Menu;
using Microsoft.Xna.Framework;
using StardewValley;

namespace Igorious.StardewValley.ColoredChestsMod.Menu
{
    public class ColorCell : MenuCell
    {
        public Color Color { get; }

        public ColorCell(int row, int column, Color color) : base(row, column, Aligment.Center, Game1.tileSize / 2, Game1.tileSize / 2, DrawAction(color))
        {
            Color = color;
        }

        private static Action<Rectangle> DrawAction(Color color)
        {
            return rect =>
            {
                Game1.spriteBatch.Draw(Textures.ColorIcon, rect, new Rectangle(0, 0, 16, 16), Color.White);
                if (color != Color.White)
                {
                    Game1.spriteBatch.Draw(Textures.ColorIcon, rect, new Rectangle(16, 0, 16, 16), color);
                }
                else
                {
                    Game1.spriteBatch.Draw(Textures.ColorIcon, rect, new Rectangle(32, 0, 16, 16), Color.White);
                }
            };
        }
    }
}