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
	public partial class UndoRedoButton<T> : ClickableComponent
	{
		public const int WIDTH = 128;
		public const int HEIGHT = 64;

		public UndoRedoButton(Rectangle bounds, string name) : base(bounds, name)
		{
		}

		public UndoRedoButton(Rectangle bounds, Item item) : base(bounds, item)
		{
		}

		public UndoRedoButton(Rectangle bounds, string name, string label) : base(bounds, name, label)
		{
		}

		public void recieveLeftClick(int x, int y, bool playSound)
		{
			int relX = x - (bounds.Left + (bounds.Width - WIDTH) / 2);
			int relY = y - (bounds.Top + (bounds.Height - HEIGHT) / 2);

			if (relY is >= 0 and <= HEIGHT)
			{
				switch (relX)
				{
					case > WIDTH:
					case < 0:
						break;
					case <= WIDTH / 2:
						Undo(playSound);
						break;
					default:
						Redo(playSound);
						break;
				}
			}
		}

		public void Draw(SpriteBatch batch)
		{
			int bx = bounds.Left + (bounds.Width - WIDTH) / 2;
			int by = bounds.Top + (bounds.Height - HEIGHT) / 2;

			// shadow
			batch.Draw(Catalog.MenuTexture,
				new Rectangle(bx - 4, by + 4, WIDTH, HEIGHT),
				new Rectangle(96, 24, WIDTH / 4, HEIGHT / 4),
				Color.Black * .4f
			);

			// bg
			batch.Draw(Catalog.MenuTexture,
				new Rectangle(bx, by, WIDTH, HEIGHT),
				new Rectangle(96, 24, 32, 16),
				Color.White
			);

			// undo
			batch.Draw(Catalog.MenuTexture,
				new Rectangle(bx, by, WIDTH / 2, HEIGHT),
				new Rectangle(96, 40, (WIDTH / 2) / 4, HEIGHT / 4),
				backwards.Count is not 0 ? Color.White : Color.White * .4f
			);

			// redo
			batch.Draw(Catalog.MenuTexture,
				new Rectangle(bx + (WIDTH / 2), by, WIDTH / 2, HEIGHT),
				new Rectangle(112, 40, (WIDTH / 2) / 4, HEIGHT / 4),
				forwards.Count is not 0 ? Color.White : Color.White * .4f
			);
		}
	}
}
