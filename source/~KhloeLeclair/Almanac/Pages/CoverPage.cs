/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using Leclair.Stardew.Almanac.Menus;

using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.BellsAndWhistles;

namespace Leclair.Stardew.Almanac.Pages {
	public class CoverPage : BasePage {

		private string[] words;
		private int wordHeight;

		#region Lifecycle

		public static CoverPage GetPage(AlmanacMenu menu, ModEntry mod) {
			return new(menu, mod);
		}

		public CoverPage(AlmanacMenu menu, ModEntry mod) : base(menu, mod) {

		}

		#endregion

		#region ITab

		public override int SortKey => int.MinValue;

		#endregion

		#region IAlmanacPage

		public override PageType Type => PageType.Cover;

		public override void Activate() {
			base.Activate();

			// Cache the string when we activate the page.
			// We do this here rather than when the class
			// is instantiated to allow for reloading i18n.
			words = (Game1.player.eventsSeen.Contains(ModEntry.Event_Island) ?
				I18n.Almanac_CoverIsland() : I18n.Almanac_Cover()
			).Split('\n');
			wordHeight = 0;
			foreach (string word in words) {
				int height = SpriteText.getHeightOfString(word);
				if (height > wordHeight)
					wordHeight = height;
			};
		}

		public override void Draw(SpriteBatch b) {
			if (words == null)
				return;

			int center = Menu.xPositionOnScreen + (Menu.width / 2);
			int titleHeight = words.Length * wordHeight;
			int y = Menu.yPositionOnScreen + (Menu.height - (titleHeight + 60 + wordHeight)) / 2;

			foreach (string word in words) {
				SpriteText.drawStringHorizontallyCenteredAt(
					b,
					word,
					center,
					y
				);

				y += wordHeight;
			}

			SpriteText.drawStringHorizontallyCenteredAt(
				b,
				Game1.content.LoadString("Strings\\UI:Billboard_Year", Menu.Year),
				center,
				y + 60,
				color: 2
				);
		}

		#endregion
	}
}
