/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.Almanac.Pages {
	public interface ICalendarPage {

		bool ShouldDimPastCells { get; }

		bool ShouldHighlightToday { get; }

		void DrawUnderCell(SpriteBatch b, WorldDate date, Rectangle bounds);

		void DrawOverCell(SpriteBatch b, WorldDate date, Rectangle bounds);

		bool ReceiveCellLeftClick(int x, int y, WorldDate date, Rectangle bounds);

		bool ReceiveCellRightClick(int x, int y, WorldDate date, Rectangle bounds);

		void PerformCellHover(int x, int y, WorldDate date, Rectangle bounds);

		//string GetCellHoverText(WorldDate date);

		//ISimpleNode GetCellHoverNode(WorldDate date);

	}
}
