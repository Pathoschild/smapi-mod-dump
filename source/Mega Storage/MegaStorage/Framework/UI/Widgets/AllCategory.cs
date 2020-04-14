using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.IO;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class AllCategory : ChestCategory
    {
        public AllCategory(string name, CustomInventoryMenu parentMenu, Vector2 offset, Rectangle sourceRect)
            : base(name, parentMenu, offset, MegaStorageMod.Instance.Helper.Content.Load<Texture2D>(Path.Combine("assets", "AllTab.png")), sourceRect, null) { }
        protected override bool BelongsToCategory(Item i) => true;
    }
}
