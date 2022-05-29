/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;

using Microsoft.Xna.Framework;

namespace Leclair.Stardew.Almanac;

public static class Sprites {

	// Pages
	public static readonly Rectangle OPEN_PAGE = new(0, 0, 320, 185);
	public static readonly Rectangle COVER_PAGE = new(0, 185, 160, 185);
	public static readonly Rectangle MAGIC_PAGE = new(320, 0, 320, 185);

	// Calendar Grids
	public static readonly Rectangle CALENDAR = new(160, 192, 144, 160);
	public static readonly Rectangle MAGIC_CALENDAR = new(304, 192, 144, 160);

	// Tabs
	public static readonly Rectangle[] TABS = new Rectangle[] {
		new(448, 192, 16, 16),
		new(448, 208, 16, 16),
		new(448, 224, 16, 16),
		new(448, 240, 16, 16)
	};

	public static readonly Rectangle[] MAGIC_TABS = new Rectangle[] {
		new(464, 192, 16, 16),
		new(464, 208, 16, 16),
		new(464, 224, 16, 16),
		new(464, 240, 16, 16)
	};

	// Vanilla Scrollbars
	public static readonly Rectangle SCROLL_BG = new(403, 383, 6, 6);
	public static readonly Rectangle SCROLL_THUMB = new(435, 463, 6, 10);

	public static readonly Rectangle LEFT_ARROW = new(0, 256, 64, 64);
	public static readonly Rectangle RIGHT_ARROW = new(0, 192, 64, 64);
	public static readonly Rectangle UP_ARROW = new(64, 64, 64, 64);
	public static readonly Rectangle DOWN_ARROW = new(0, 64, 64, 64);

	// Custom Scrollbars
	public static readonly ScrollbarTheme STANDARD_SCROLL = new(
		scrollBG: new(170, 352, 6, 6),
		scrollThumb: new(170, 358, 6, 10),
		upArrow: new(480, 192, 16, 16),
		downArrow: new(480, 208, 16, 16),
		leftArrow: new(480, 224, 16, 16),
		rightArrow: new(480, 240, 16, 16)
	);

	public static readonly ScrollbarTheme MAGIC_SCROLL = new(
		scrollBG: new(160, 352, 6, 6),
		scrollThumb: new(160, 358, 6, 10),
		upArrow: new(176, 352, 16, 16),
		downArrow: new(192, 352, 16, 16),
		leftArrow: new(208, 352, 16, 16),
		rightArrow: new(224, 352, 16, 16)
	);

	// Custom Tooltips
	public static readonly TooltipTheme STANDARD_TOOLTIP = new(
		background: new(480, 288, 15, 15),
		horizontalLine: new(480, 304, 16, 16),
		verticalLine: new(480, 320, 16, 16)
	);

	public static readonly TooltipTheme MAGIC_TOOLTIP = new(
		background: new(288, 352, 15, 15),
		horizontalLine: new(304, 352, 16, 16),
		verticalLine: new(320, 352, 16, 16)
	);

	// Custom Mouse
	public static readonly Rectangle MOUSE_NORMAL = new(480, 256, 16, 16);
	public static readonly Rectangle MOUSE_SNAP = new(480, 272, 16, 16);

	// Icons
	public readonly static Rectangle ICON_BOOK = new(480, 336, 16, 16);

	// Books
	public readonly static Rectangle BOOK_BLANK = new(496, 192, 32, 32);
	public readonly static Rectangle BOOK_UNCOLORED = new(496, 224, 32, 32);
}


public class TooltipTheme {
	public readonly Rectangle BACKGROUND;
	public readonly Rectangle HORIZONTAL_LINE;
	public readonly Rectangle VERTICAL_LINE;

	public TooltipTheme(Rectangle background, Rectangle horizontalLine, Rectangle verticalLine) {
		BACKGROUND = background;
		HORIZONTAL_LINE = horizontalLine;
		VERTICAL_LINE = verticalLine;
	}
}


public class ScrollbarTheme {

	public readonly Rectangle SCROLL_BG;
	public readonly Rectangle SCROLL_THUMB;

	public readonly Rectangle UP_ARROW;
	public readonly Rectangle DOWN_ARROW;
	public readonly Rectangle LEFT_ARROW;
	public readonly Rectangle RIGHT_ARROW;

	public ScrollbarTheme(Rectangle scrollBG, Rectangle scrollThumb, Rectangle upArrow, Rectangle downArrow, Rectangle leftArrow, Rectangle rightArrow) {
		SCROLL_BG = scrollBG;
		SCROLL_THUMB = scrollThumb;
		UP_ARROW = upArrow;
		DOWN_ARROW = downArrow;
		LEFT_ARROW = leftArrow;
		RIGHT_ARROW = rightArrow;
	}
}
