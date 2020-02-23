using MegaStorage.Framework.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;

namespace MegaStorage.Framework.Interface
{
    public class ChestCategory : ClickableComponent
    {
        protected const int StartY = -40;
        protected const int Height = 60;

        protected readonly int Index;

        private readonly string _name;
        private readonly Vector2 _spritePos;
        private readonly int[] _categoryIds;

        public ChestCategory(int index, string name, Vector2 spritePos, int[] categoryIds, int x, int y) : base(new Rectangle(x - 72, y + StartY + index * Height, 64, Height), name)
        {
            Index = index;
            _name = name;
            _spritePos = spritePos;
            _categoryIds = categoryIds;
        }

        public virtual void Draw(SpriteBatch b, int x, int y)
        {
            if (!(b is null) && ModConfig.Instance.EnableCategories)
            {
                b.Draw(Game1.mouseCursors,
                    new Vector2(x - 72, y + StartY + Index * Height),
                    new Rectangle((int)_spritePos.X, (int)_spritePos.Y, 16, 16),
                    Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            }
        }

        public void DrawTooltip(SpriteBatch b)
        {
            IClickableMenu.drawHoverText(b, _name, Game1.smallFont);
        }

        public List<Item> Filter(IList<Item> items)
        {
            return items.Where(BelongsToCategory).ToList();
        }

        protected virtual bool BelongsToCategory(Item i)
        {
            return !(i is null) && _categoryIds.Contains(i.Category);
        }
    }
}
