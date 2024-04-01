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

		internal int filter_count;
		internal int current_filter;

		public abstract ClickableTextureComponent GetTab();
		public virtual void Exit() { }

		public abstract int Count();

		public virtual void Resize(Rectangle region)
		{
			width = region.Width;
			height = region.Height;
			xPositionOnScreen = region.X;
			yPositionOnScreen = region.Y;
		}

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
