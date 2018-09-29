using Microsoft.Xna.Framework;
using StardewValley;

namespace Igorious.StardewValley.DynamicAPI.Menu
{
    public sealed class ItemMenuCell : MenuCell
    {
        public ItemMenuCell(int row, int column, Aligment aligment, Item o) 
            : base(row, column, aligment, Game1.tileSize, Game1.tileSize, r => o.drawInMenu(Game1.spriteBatch, new Vector2(r.X, r.Y), 1)) {}
    }
}