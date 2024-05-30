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
	public abstract class ScreenPage : IClickableMenu
	{
		internal const int CELL_SIZE = 80;
		internal const int FILTER_WIDTH = 72;
		internal const int FILTER_HEIGHT = 42;
		internal const int FILTER_SCALE = 3;

		protected int filter_count;
		protected int current_filter;

		/// <returns>The tab representing this page</returns>
		public abstract ClickableTextureComponent GetTab();

		/// <summary>Called when the page is destroyed</summary>
		public virtual void Exit() { }

		/// <returns>The number of items this page contains</returns>
		public abstract int Count();

		public virtual void Resize(Rectangle region)
		{
			width = region.Width;
			height = region.Height;
			xPositionOnScreen = region.X;
			yPositionOnScreen = region.Y;
		}

		/// <summary>Draws on top of the whole menu.</summary>
		public virtual void DrawTooltip(SpriteBatch batch)
		{

		}

		/// <summary>Draw the magnifier preview for an item</summary>
		protected static void DrawMagnified(SpriteBatch b, Item hovered)
		{
			float scale = ModEntry.config.MagnifyScale;
			int boxSize = (int)(64f * scale);
			int itemOffset = (int)(32f * (scale - 1f));
			const int BORDER = 16;
			const int CURSOR = 48;

			var mouse = Game1.getMousePosition(true);

			if (mouse.X < boxSize)
			{
				mouse.X += CURSOR;
				mouse.Y -= boxSize + BORDER - 24;
			} 
			else
			{
				mouse.X -= boxSize + BORDER;
				mouse.Y += CURSOR;
			}

			drawTextureBox(b, mouse.X - BORDER, mouse.Y - BORDER, boxSize + (BORDER * 2), boxSize + (BORDER * 2), Color.White);
			hovered.drawInMenu(b, new(mouse.X + itemOffset, mouse.Y + itemOffset), scale);
		}

		/// <summary>Activate the clicked filter if possible</summary>
		/// <param name="x">Mouse X</param>
		/// <param name="y">Mouse Y</param>
		/// <param name="playSound">Whether or not to play sound</param>
		/// <returns>True if a filter was selected, otherwise False</returns>
		public bool TrySelectFilter(int x, int y, bool playSound)
		{
			int relX = x - xPositionOnScreen;
			int relY = y - yPositionOnScreen;

			if (relX is > FILTER_WIDTH)
				return false;

			int which = relY / (FILTER_HEIGHT - FILTER_SCALE);

			if (which >= filter_count || which == current_filter)
				return false;

			if (playSound)
				Game1.playSound("shwip");

			current_filter = which;
			return true;
		}

		/// <summary>Draws the filter tabs for this page. Icons are treated as a row of 24x24 sprites in the UI texture.</summary>
		/// <param name="textureRow">The Y pixel coordinate in the UI texture to use for the tab icons</param>
		/// <param name="ribbonCount">How many of the filters should use the ribbon (favorites) background</param>
		public void DrawFilters(SpriteBatch batch, int textureRow, int ribbonCount, int x, int y)
		{
			int sx = 0;

			int i = 0;
			var shadow = Color.Black * .4f;
			while(i < filter_count - ribbonCount)
			{
				int nx = i == current_filter ? x - 16 : x;

				// shadow
				batch.Draw(Catalog.MenuTexture,
					new Rectangle(nx - 4, y + 4, FILTER_WIDTH, FILTER_HEIGHT),
					new Rectangle(0, 24, FILTER_WIDTH / FILTER_SCALE, FILTER_HEIGHT / FILTER_SCALE),
					shadow);

				// standard bg
				batch.Draw(Catalog.MenuTexture,
					new Rectangle(nx, y, FILTER_WIDTH, FILTER_HEIGHT),
					new Rectangle(0, 24, FILTER_WIDTH / FILTER_SCALE, FILTER_HEIGHT / FILTER_SCALE),
					Color.White);

				// icon
				batch.Draw(
					Catalog.MenuTexture,
					new Rectangle(nx + (i == current_filter ? 6 * FILTER_SCALE : 3 * FILTER_SCALE), y + 3 * FILTER_SCALE, 30, 24), 
					new Rectangle(sx, textureRow, 10, 8), 
					Color.White);

				y += FILTER_HEIGHT - FILTER_SCALE;
				sx += 10;
				i++;
			}
			while(i < filter_count)
			{
				int nx = i == current_filter ? x - 16 : x;

				// shadow
				batch.Draw(Catalog.MenuTexture,
					new Rectangle(nx - 4, y + 4, FILTER_WIDTH, FILTER_HEIGHT),
					new Rectangle(24, 24, FILTER_WIDTH / FILTER_SCALE, FILTER_HEIGHT / FILTER_SCALE),
					shadow);

				// ribbon bg
				batch.Draw(Catalog.MenuTexture,
					new Rectangle(nx, y, FILTER_WIDTH, FILTER_HEIGHT),
					new Rectangle(24, 24, FILTER_WIDTH / FILTER_SCALE, FILTER_HEIGHT / FILTER_SCALE),
					Color.White);

				// icon
				batch.Draw(
					Catalog.MenuTexture,
					new Rectangle(nx + (i == current_filter ? 16 : 8), y + 8, 30, 24),
					new Rectangle(sx, textureRow, 10, 8),
					Color.White);

				y += FILTER_HEIGHT - FILTER_SCALE;
				sx += 10;
				i++;
			}
		}
	}
}
