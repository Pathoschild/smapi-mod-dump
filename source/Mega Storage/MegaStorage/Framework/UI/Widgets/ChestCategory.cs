using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class ChestCategory : CustomClickableTextureComponent
    {
        private const int SelectedOffset = 8;
        private readonly IList<int> _categoryIds;
        public ChestCategory(string name, CustomInventoryMenu parentMenu, Vector2 offset, Rectangle sourceRect, IList<int> categoryIds)
            : this(name, parentMenu, offset, Game1.mouseCursors, sourceRect, categoryIds) { }
        public ChestCategory(string name, CustomInventoryMenu parentMenu, Vector2 offset, Texture2D texture, Rectangle sourceRect, IList<int> categoryIds)
            : base(
                    name,
                    parentMenu,
                    offset,
                    texture,
                    sourceRect,
                    MegaStorageMod.ModHelper.Translation.Get($"category.{name}"))
        {
            _categoryIds = categoryIds;
        }
        public void Draw(SpriteBatch b, bool selected = false)
        {
            bounds.X = ParentMenu.xPositionOnScreen + (int)Offset.X + (selected ? SelectedOffset : 0);
            bounds.Y = ParentMenu.yPositionOnScreen + (int)Offset.Y;
            draw(b);
        }
        public List<Item> Filter(IList<Item> items) => items.Where(BelongsToCategory).ToList();
        protected virtual bool BelongsToCategory(Item i) => !(i is null) && _categoryIds.Contains(i.Category);
    }
}