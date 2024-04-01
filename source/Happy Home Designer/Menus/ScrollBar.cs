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
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;

namespace HappyHomeDesigner.Menus
{
	public class ScrollBar
	{
		public const int WIDTH = 40;

		public int Rows
		{
			get => rows;
			set
			{
				Offset = Math.Max(0, Math.Min(rows is 0 ? 0 : value * Offset / rows, value - visibleRows));
				CellOffset = Offset * Columns;
				rows = value;
			}
		}
		private int rows = 0;
		public int Columns
		{
			get => columns;
			set
			{
				columns = value;
				CellOffset = Offset * columns;
			}
		}
		private int columns = 1;
		public int VisibleRows
		{
			get => visibleRows;
			set
			{
				visibleRows = value;
				Offset = Math.Min(Offset, Math.Max(0, rows - visibleRows));
				CellOffset = Offset * columns;
			}
		}
		private int visibleRows = 0;
		public int Offset { get; private set; }
		public int CellOffset { get; private set; }

		private int height = 1;
		private Rectangle scroller;

		private readonly ClickableTextureComponent UpArrow =
			new(new(0, 0, 44, 48), Game1.mouseCursors, new(421, 459, 11, 12), 4f);

		private readonly ClickableTextureComponent DownArrow =
			new(new(0, 0, 44, 48), Game1.mouseCursors, new(421, 472, 11, 12), 4f);

		private static readonly Rectangle BackgroundSource = new(403, 383, 6, 6);
		private static readonly Rectangle ThumbSource = new(435, 463, 6, 10);

		public void Draw(SpriteBatch batch)
		{
			// debug
			//batch.Draw(Game1.staminaRect, scroller, Color.Blue);

			if (VisibleRows < Rows)
			{
				if (height is >= 256)
				{
					// shadow
					DrawStrip(batch, Game1.mouseCursors, BackgroundSource, new(scroller.X - 4, scroller.Y + 4, scroller.Width, scroller.Height), 4, Color.Black * .4f);

					// bar
					DrawStrip(batch, Game1.mouseCursors, BackgroundSource, scroller, 4, Color.White);
					DrawStrip(batch, Game1.mouseCursors, ThumbSource,
						new(scroller.X, scroller.Y + scroller.Height * Offset / Rows, 
							scroller.Width, Math.Max(scroller.Height * VisibleRows / Rows, ThumbSource.Height * 4)),
						4, Color.White);
				}

				UpArrow.draw(batch);
				DownArrow.draw(batch);
			}
		}

		public void AdvanceRows(int count)
		{
			int oldOffset = Offset;
			Offset = Math.Max(0, Math.Min(Offset + count, Rows - VisibleRows));
			CellOffset = Offset * Columns;

			if (oldOffset != Offset)
				Game1.playSound("shiny4");
		}

		public void Resize(int height, int x, int y)
		{
			height = Math.Max(1, height);
			this.height = height;

			UpArrow.setPosition(x, y);
			DownArrow.setPosition(x, y + height - 48);
			scroller = new(x + 4, y + 52, WIDTH, height - 108);
		}

		public void Reset()
		{
			Offset = 0;
			CellOffset = 0;
		}

		public void Click(int x, int y)
		{
			if (UpArrow.containsPoint(x, y))
				AdvanceRows(-1);
			else if (DownArrow.containsPoint(x, y))
				AdvanceRows(1);
		}

		public void Hover(int x, int y)
		{
			if (scroller.Contains(x, y) && Game1.input.GetMouseState().LeftButton is ButtonState.Pressed)
			{
				int relY = y - scroller.Y;

				int oldOffset = Offset;

				Offset = Math.Max(0, Math.Min(Rows * relY / scroller.Height - visibleRows / 2, rows - visibleRows));
				CellOffset = Offset * columns;
				if (oldOffset != Offset)
					Game1.playSound("shiny4");
			}
		}

		public static void DrawStrip(SpriteBatch batch, Texture2D texture, Rectangle source, Rectangle dest, int scale, Color color)
		{
			// buffer is how much from each end to use as end
			// 1px gap for odd sizes and 2px gap for even sizes

			int buffer = source.Height / 2 - (1 - (source.Height & 1));
			int x = (dest.Width / 2) - (source.Width * scale / 2);
			x = x / scale * scale;
			x += dest.X;

			batch.Draw(texture, 
				new Rectangle(x, dest.Y, source.Width * scale, buffer * scale),
				new Rectangle(source.X, source.Y, source.Width, buffer),
				color);
			batch.Draw(texture,
				new Rectangle(x, dest.Y + buffer * scale, source.Width * scale, dest.Height - buffer * scale * 2),
				new Rectangle(source.X, source.Y + buffer, source.Width, source.Height - buffer * 2),
				color);
			batch.Draw(texture,
				new Rectangle(x, dest.Y + dest.Height - buffer * scale, scale * source.Width, buffer * scale),
				new Rectangle(source.X, source.Y + source.Height - buffer, source.Width, buffer),
				color);
		}
	}
}
