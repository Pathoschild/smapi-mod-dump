/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;

namespace Shockah.CommonModCode.UI
{
	public enum FlowDirection
	{
		LeftToRightAndTopToBottom,
		RightToLeftAndTopToBottom,
		LeftToRightAndBottomToTop,
		RightToLeftAndBottomToTop,

		TopToBottomAndLeftToRight,
		BottomToTopAndLeftToRight,
		TopToBottomAndRightToLeft,
		BottomToTopAndRightToLeft
	}

	public static class FlowDirectionExtensions
	{
		public static Orientation GetFirstOrientation(this FlowDirection self)
			=> self.IsHorizontalFirst() ? Orientation.Horizontal : Orientation.Vertical;

		public static Orientation GetSecondOrientation(this FlowDirection self)
			=> self.IsHorizontalSecond() ? Orientation.Horizontal : Orientation.Vertical;

		public static bool IsHorizontalFirst(this FlowDirection self)
			=> self is FlowDirection.LeftToRightAndTopToBottom or FlowDirection.LeftToRightAndBottomToTop or FlowDirection.RightToLeftAndTopToBottom or FlowDirection.RightToLeftAndBottomToTop;

		public static bool IsVerticalFirst(this FlowDirection self)
			=> self is FlowDirection.TopToBottomAndLeftToRight or FlowDirection.TopToBottomAndRightToLeft or FlowDirection.BottomToTopAndLeftToRight or FlowDirection.BottomToTopAndRightToLeft;

		public static bool IsHorizontalSecond(this FlowDirection self)
			=> self.IsVerticalFirst();

		public static bool IsVerticalSecond(this FlowDirection self)
			=> self.IsHorizontalFirst();

		public static bool IsLeftToRight(this FlowDirection self)
			=> self is FlowDirection.LeftToRightAndTopToBottom or FlowDirection.LeftToRightAndBottomToTop or FlowDirection.TopToBottomAndLeftToRight or FlowDirection.BottomToTopAndLeftToRight;

		public static bool IsRightToLeft(this FlowDirection self)
			=> self is FlowDirection.RightToLeftAndTopToBottom or FlowDirection.RightToLeftAndBottomToTop or FlowDirection.TopToBottomAndRightToLeft or FlowDirection.BottomToTopAndRightToLeft;

		public static bool IsTopToBottom(this FlowDirection self)
			=> self is FlowDirection.LeftToRightAndTopToBottom or FlowDirection.RightToLeftAndTopToBottom or FlowDirection.TopToBottomAndLeftToRight or FlowDirection.TopToBottomAndRightToLeft;

		public static bool IsBottomToTop(this FlowDirection self)
			=> self is FlowDirection.LeftToRightAndBottomToTop or FlowDirection.RightToLeftAndBottomToTop or FlowDirection.BottomToTopAndLeftToRight or FlowDirection.BottomToTopAndRightToLeft;

		public static (int x, int y) GetXYPositionFromZeroOrigin(this FlowDirection self, (int column, int row) position)
			=> self.GetXYPositionFromZeroOrigin(position.column, position.row);

		public static (int x, int y) GetXYPositionFromZeroOrigin(this FlowDirection self, int column, int row)
		{
			int columnDirection = (self.IsHorizontalFirst() ? self.IsLeftToRight() : self.IsTopToBottom()) ? 1 : -1;
			int rowDirection = (self.IsHorizontalSecond() ? self.IsLeftToRight() : self.IsTopToBottom()) ? 1 : -1;
			return self.IsHorizontalFirst()
				? (column * columnDirection, row * rowDirection)
				: (row * rowDirection, column * columnDirection);
		}

		public static IEnumerable<(int x, int y)> GetFlowXYPositionsFromZeroOrigin(this FlowDirection self, int columns)
		{
			int currentColumn = 0;
			int currentRow = 0;
			while (true)
			{
				for (int i = 0; i < columns; i++)
					yield return self.GetXYPositionFromZeroOrigin(currentColumn++, currentRow);
				currentColumn = 0;
				currentRow++;
			}
		}
	}
}