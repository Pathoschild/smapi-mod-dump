using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace PregnancyRole
{
	internal class SkillsPageOverlay : DropdownOverlay
	{
		public SkillsPageOverlay ()
		{
			Point offset = Config.PlayerDropdownOrigin;
			if (offset.Equals (Point.Zero))
			{
				// Count the number of skills shown, considering mods.
				int skillCount = 5;
				if (Helper.ModRegistry.IsLoaded ("spacechase0.LuckSkill"))
					++skillCount;
				var spaceCore = Helper.ModRegistry.GetApi ("spacechase0.SpaceCore");
				var skills = spaceCore?.GetType ()?.Assembly?.GetType ("SpaceCore.Skills");
				if (skills != null)
				{
					skillCount += Helper.Reflection.GetMethod (skills,
						"GetSkillList").Invoke<string[]> ().Length;
				}

				// Align the label and dropdown below the other skills.
				bool altAlign =
					LocalizedContentManager.CurrentLanguageCode ==
						LocalizedContentManager.LanguageCode.ru ||
					LocalizedContentManager.CurrentLanguageCode ==
						LocalizedContentManager.LanguageCode.it;
				offset.X = altAlign
					? 800 + IClickableMenu.borderWidth * 2 + 64 - 448 - 48
					: IClickableMenu.borderWidth +
						IClickableMenu.spaceToClearTopBorder + 256 - 8;
				offset.Y = IClickableMenu.borderWidth +
					IClickableMenu.spaceToClearTopBorder - 8 - 4 + skillCount * 56;
				if (IsAndroid)
					offset.Y += 32 - 72;
			}
			setOffset (offset);
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

		protected override bool shouldRender => Config.ShowPlayerDropdown &&
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
