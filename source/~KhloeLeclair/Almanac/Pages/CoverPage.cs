/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leclair.Stardew.Common;

using StardewValley;
using StardewValley.BellsAndWhistles;

using Leclair.Stardew.Almanac.Menus;

namespace Leclair.Stardew.Almanac.Pages {
	public class CoverPage : BasePage<BaseState> {

		public static readonly Color DEFAULT_COLOR = CommonHelper.ParseColor("#974E24").Value;

		private readonly string[] words;
		private readonly int wordHeight;

		#region Lifecycle

		public static CoverPage GetPage(AlmanacMenu menu, ModEntry mod) {
			return new(menu, mod);
		}

		public CoverPage(AlmanacMenu menu, ModEntry mod) : base(menu, mod) {
			// Cache the string.
			words = (Mod.HasIsland(Game1.player) ?
				I18n.Almanac_CoverIsland() : I18n.Almanac_Cover()
			).Split('\n');
			wordHeight = 0;
			foreach (string word in words) {
				int height = SpriteText.getHeightOfString(word);
				if (height > wordHeight)
					wordHeight = height;
			};
		}

		#endregion

		#region ITab

		public override int SortKey => int.MinValue;

		#endregion

		#region IAlmanacPage

		public override PageType Type => PageType.Cover;

		public override void Activate() {
			base.Activate();
		}

		public override void Draw(SpriteBatch b) {
			if (words == null)
				return;

			int center = Menu.xPositionOnScreen + (Menu.width / 2);
			int titleHeight = words.Length * wordHeight;
			int y = Menu.yPositionOnScreen + (Menu.height - (titleHeight + 60 + wordHeight)) / 2;

			foreach (string word in words) {
				RenderHelper.DrawCenteredSpriteText(
					b,
					word,
					center,
					y,
					color: Mod.Theme?.CoverTextColor ?? DEFAULT_COLOR
				);

				y += wordHeight;
			}

			RenderHelper.DrawCenteredSpriteText(
				b,
				Game1.content.LoadString("Strings\\UI:Billboard_Year", Menu.Year),
				center,
				y + 60,
				color: Mod.Theme?.CoverYearColor ?? SpriteText.getColorFromIndex(2)
			);
		}

		#endregion
	}
}
