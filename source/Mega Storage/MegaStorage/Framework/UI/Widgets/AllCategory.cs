/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/Stardew-MegaStorage
**
*************************************************/

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
