/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

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