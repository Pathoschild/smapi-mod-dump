using StardewValley;
using StardewValley.Menus;

namespace PregnancyRole
{
	internal class SkillsPageOverlay : DropdownOverlay
	{
		public SkillsPageOverlay ()
		{
			// Align the label and dropdown below the standard skills.
			bool altAlign =
				LocalizedContentManager.CurrentLanguageCode ==
					LocalizedContentManager.LanguageCode.ru ||
				LocalizedContentManager.CurrentLanguageCode ==
					LocalizedContentManager.LanguageCode.it;
			int xOffset = altAlign
				? 800 + IClickableMenu.borderWidth * 2 + 64 - 448 - 48
				: IClickableMenu.borderWidth +
					IClickableMenu.spaceToClearTopBorder + 256 - 8;
			int yOffset = IClickableMenu.borderWidth +
				IClickableMenu.spaceToClearTopBorder - 8 - 4 + 5 * 56;
			if (IsAndroid)
				yOffset += 32 - 72;
			setOffset (xOffset, yOffset);
		}

		protected override int roleIndex
		{
			get
			{
				return (int) Model.GetPregnancyRole (Game1.player);
			}
			set
			{
				Model.SetPregnancyRole (Game1.player, (Role) value);
			}
		}

		protected override bool shouldRender =>
			Game1.activeClickableMenu is GameMenu gm &&
				gm.currentTab == GameMenu.skillsTab;

		protected override IClickableMenu trueMenu
		{
			get
			{
				if (!(Game1.activeClickableMenu is GameMenu gm))
					return null;
				return gm.pages[gm.currentTab];
			}
		}
	}
}
