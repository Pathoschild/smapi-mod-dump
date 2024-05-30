/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace HappyHomeDesigner.Menus
{
	internal class ItemEntry : IGridItem
	{
		private const int CELL_SIZE = 80;
		private static readonly Rectangle background = new(128, 128, 64, 64);

		public Item item;

		public ItemEntry(Item item)
		{
			this.item = item;
		}

		public void Draw(SpriteBatch b, int x, int y)
		{
			IClickableMenu.drawTextureBox(b, Game1.menuTexture, background, x, y, CELL_SIZE, CELL_SIZE, Color.White, 1f, false);
			item.drawInMenu(b, new(x + 8, y + 8), 1f);
		}

		/// <inheritdoc/>
		public string GetName()
		{
			return item.DisplayName;
		}

		public override string ToString()
		{
			return item.DisplayName + '|' + item.ItemId;
		}

		public bool ToggleFavorite(bool playSound)
		{
			return false;
		}
	}
}
