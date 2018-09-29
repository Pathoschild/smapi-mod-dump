using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Igorious.StardewValley.DynamicAPI.Menu
{
    public sealed class IconMenuCell : MenuCell
    {
        public IconMenuCell(int row, int column, Aligment aligment, Texture2D texture, Rectangle sourceRect, int width, int? height = null, bool shadow = false) : base(row, column, aligment)
        {
            Width = width;
            Height = height ?? width;
            Draw = r => Game1.spriteBatch.Draw(
                texture,
                r,
                sourceRect,
                shadow? (Color.Black * 0.3f) : Color.White);
        }
    }
}