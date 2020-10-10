/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/Server-Browser
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerBrowser
{
	class ListBox
	{
		public int x, y, width, height, rows;
		public const int rowHeight = 32;

		int currentIndex = 0;
		Dictionary<string, Color> items;
		IEnumerable<KeyValuePair<string,Color>> VisibleItems => items.Skip((int)currentIndex).Take(rows);

		ClickableTextureComponent upButton;
		ClickableTextureComponent downButton;

		public ListBox(int x, int y, int width, int rows, Dictionary<string, Color> items)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = rows * rowHeight + 30;
			this.rows = rows;
			this.items = items;
			
			upButton = new ClickableTextureComponent(new Rectangle(x + width, y - 39, 40, 44), Game1.mouseCursors, new Rectangle(421, 459, 12, 11), 4f, false);
			downButton = new ClickableTextureComponent(new Rectangle(x + width, y + height, 40, 44), Game1.mouseCursors, new Rectangle(421, 472, 12, 11), 4f, false);

		}

		public void Draw(SpriteBatch spriteBatch)
		{
			DrawTools.DrawBox(spriteBatch, x, y, width, height);

			int i = 0;
			foreach (var pair in VisibleItems)
			{
				spriteBatch.DrawString(Game1.smallFont, pair.Key, new Vector2(x + 15, y + 10 + i * rowHeight), pair.Value);
				i++;
			}

			if (currentIndex + rows < items.Count)
				downButton.draw(spriteBatch);

			if (currentIndex - 1 >= 0)
				upButton.draw(spriteBatch);
		}

		public void ReleaseClick(int x, int y)
		{
			if (currentIndex + rows < items.Count && downButton.containsPoint(x, y))
				currentIndex++;

			if (currentIndex - 1 >= 0 && upButton.containsPoint(x, y))
				currentIndex--;
		}

		public void TryHover(int x, int y)
		{
			downButton.tryHover(x, y, 0.2f);
			upButton.tryHover(x, y, 0.2f);
		}
	}
}
