/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace Igorious.StardewValley.DynamicAPI.Menu
{
    public sealed class TextMenuCell : MenuCell
    {
        public TextMenuCell(int row, int column, Aligment aligment, string text) : base(row, column, aligment)
        {
            var measure = Game1.smallFont.MeasureString(text);
            Width = (int)measure.X;
            Height = (int)measure.Y;
            Draw = r => Utility.drawTextWithShadow(
                Game1.spriteBatch,
                text,
                Game1.smallFont,
                new Vector2(r.X, r.Y),
                Game1.textColor);
        }
    }
}