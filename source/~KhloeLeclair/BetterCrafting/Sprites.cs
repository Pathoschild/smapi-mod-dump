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

using Leclair.Stardew.Common.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Leclair.Stardew.BetterCrafting;

public static class Sprites {

	public static class Other {
		public static readonly Rectangle SLOT_BORDERS = new(32, 32, 16, 16);
		public static readonly Rectangle SLOT_DISABLED = new(48, 64, 16, 16);

		public static readonly Rectangle TAB_BLANK = new(0, 112, 16, 16);

		public static readonly Rectangle NEW_LABEL = new(0, 128, 16, 8);

		public static readonly Rectangle BTN_MINUS = new(0, 136, 8, 8);
		public static readonly Rectangle BTN_PLUS = new(8, 136, 8, 8);

		public static readonly Rectangle BTN_HAMMER = new(16, 112, 16, 16);
		public static readonly Rectangle BTN_OK = new(32, 112, 16, 16);
		public static readonly Rectangle BTN_CANCEL = new(48, 112, 16, 16);
	}

	public static class CustomScroll {
		public static readonly Rectangle SCROLL_AREA = new(16, 80, 6, 6);
		public static readonly Rectangle SCROLL_BAR = new(16, 86, 6, 10);
		public static readonly Rectangle SCROLL_BAR_H = new(22, 80, 10, 6);
		public static readonly Rectangle PAGE_UP = new(0, 80, 16, 16);
		public static readonly Rectangle PAGE_DOWN = new(0, 96, 16, 16);
		public static readonly Rectangle PAGE_LEFT = new(32, 80, 16, 16);
		public static readonly Rectangle PAGE_RIGHT = new(48, 80, 16, 16);

		public static void ApplyToScrollableFlow(ScrollableFlow flow, Texture2D? texture) {
			flow.ScrollAreaTexture = texture ?? flow.ScrollAreaTexture;
			flow.ScrollAreaSource = SCROLL_AREA;

			flow.ScrollBar.texture = texture ?? flow.ScrollBar.texture;
			flow.ScrollBar.sourceRect = SCROLL_BAR;

			flow.btnPageUp.texture = texture ?? flow.btnPageUp.texture;
			flow.btnPageUp.baseScale = 3.2f;
			flow.btnPageUp.sourceRect = PAGE_UP;

			flow.btnPageDown.texture = texture ?? flow.btnPageDown.texture;
			flow.btnPageDown.baseScale = 3.2f;
			flow.btnPageDown.sourceRect = PAGE_DOWN;
		}

	}

	public static class Buttons {

		internal static Func<Texture2D>? _TexLoader;

		internal static Texture2D? _TexCache;

		public static Texture2D? Texture {
			get {
				_TexCache ??= _TexLoader?.Invoke();
				return _TexCache;
			}
		}

		public readonly static Rectangle UNIFORM_OFF = new(0, 0, 16, 16);
		public readonly static Rectangle UNIFORM_ON = new(0, 16, 16, 16);

		public readonly static Rectangle SEASONING_ON = new(16, 0, 16, 16);
		public readonly static Rectangle SEASONING_OFF = new(16, 16, 16, 16);
		public readonly static Rectangle SEASONING_LOCAL = new(48, 16, 16, 16);

		public readonly static Rectangle FAVORITE_ON = new(32, 0, 16, 16);
		public readonly static Rectangle FAVORITE_OFF = new(32, 16, 16, 16);

		public readonly static Rectangle WRENCH = new(48, 0, 16, 16);
		public readonly static Rectangle SETTINGS = new(64, 0, 16, 16);

		public readonly static Rectangle SEARCH_OFF = new(64, 32, 16, 16);
		public readonly static Rectangle SEARCH_ON = new(64, 16, 16, 16);

		public readonly static Rectangle QUALITY_0 = new(0, 32, 16, 16);
		public readonly static Rectangle QUALITY_1 = new(16, 32, 16, 16);
		public readonly static Rectangle QUALITY_2 = new(32, 32, 16, 16);
		public readonly static Rectangle QUALITY_3 = new(48, 32, 16, 16);

		public readonly static Rectangle TO_INVENTORY = new(0, 48, 16, 16);
		public readonly static Rectangle FROM_INVENTORY = new(16, 48, 16, 16);

		public readonly static Rectangle FILTER_OFF = new(32, 48, 16, 16);
		public readonly static Rectangle FILTER_ON = new(48, 48, 16, 16);

		public readonly static Rectangle INCLUDE_MISC_OFF = new(16, 64, 16, 16);
		public readonly static Rectangle INCLUDE_MISC_ON = new(0, 64, 16, 16);

		public readonly static Rectangle COPY = new(0, 64, 16, 16);
		public readonly static Rectangle PASTE = new(32, 64, 16, 16);
		public readonly static Rectangle TRASH = new(48, 64, 16, 16);

		public readonly static Rectangle SELECT_BG = new(64, 48, 16, 16);
	}

}
