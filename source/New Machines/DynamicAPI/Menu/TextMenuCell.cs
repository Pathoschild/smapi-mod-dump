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