/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;

using Leclair.Stardew.Common.UI;

using Microsoft.Xna.Framework;

using StardewValley;
using StardewValley.BellsAndWhistles;

namespace Leclair.Stardew.ThemeManager.Models;


public class BaseTheme : IBaseTheme {

	#region Generic Colors

	public Color? TextColor { get; set; }
	public Color? TextShadowColor { get; set; }
	public Color? TextShadowAltColor { get; set; }

	public Color? ErrorTextColor { get; set; }

	public Color? HoverColor { get; set; }
	public Color? ButtonHoverColor { get; set; }

	public Color? UnselectedOptionColor { get; set; }

	public Dictionary<int, Color> SpriteTextColors { get; set; } = new();

	#endregion

	#region Billboard

	public Color? CalendarDimColor { get; set; }
	public Color? CalendarTodayColor { get; set; }
	public Color? BillboardHoverColor { get; set; }
	public Color? BillboardTextColor { get; set; }

	#endregion

	#region CarpenterMenu

	public Color? CarpenterMagicBackgroundColor { get; set; }
	public Color? CarpenterMagicTextColor { get; set; }

	#endregion

	#region CoopMenu

	public Color? CoopSmallTabSelectedColor { get; set; }
	public Color? CoopSmallTabSelectedShadowColor { get; set; }
	public Color? CoopTabSelectedColor { get; set; }
	public Color? CoopTabSelectedShadowColor { get; set; }

	public Color? CoopTabHoverColor { get; set; }
	public Color? CoopTabHoverShadowColor { get; set; }
	public Color? CoopHoverColor { get; set; }

	#endregion

	#region DayTimeMoneyBox

	public Alignment? DayTimeAlignment { get; set; }
	public int? DayTimeOffsetX { get; set; }
	public int? DayTimeOffsetY { get; set; }

	public Color? DayTimeTextColor { get; set; }

	public Color? DayTimeAfterMidnightColor { get; set; }

	#endregion

	#region ExitPage

	public Color? ExitPageHoverColor { get; set; }

	#endregion

	#region ForgeMenu

	public Color? ForgeMenuBackgroundColor { get; set; }

	#endregion

	#region IClickableMenu

	public Color? HoverTextTextColor { get; set; }
	public Color? HoverTextShadowColor { get; set; }

	public Color? HoverTextInsufficientTextColor { get; set; }
	public Color? HoverTextModifiedStatTextColor { get; set; }
	public Color? HoverTextEnchantmentTextColor { get; set; }
	public Color? HoverTextForgeCountTextColor { get; set; }
	public Color? HoverTextForgedTextColor { get; set; }

	#endregion

	#region LetterViewerMenu

	public Color? LetterViewerHoverColor { get; set; }

	#endregion

	#region LevelUpMenu

	public Color? LevelUpHoverTextColor { get; set; }

	#endregion

	#region LoadGameMenu

	public Color? LoadGameErrorTextColor { get; set; }
	public Color? LoadGameHoverColor { get; set; }

	#endregion

	#region MineElevatorMenu

	public Color? ElevatorCurrentFloorTextColor { get; set; }
	public Color? ElevatorFloorTextColor { get; set; }

	#endregion

	#region MoneyDial

	public Color? MoneySparkleColor { get; set; }

	public Color? MoneyTextColor { get; set; }

	#endregion

	#region OptionDropDown

	public Color? DropDownTextColor { get; set; }
	public Color? DropDownHoverColor { get; set; }

	#endregion

	#region QuestLog

	public Color? QuestHoverColor { get; set; }
	public Color? QuestObjectiveTextColor { get; set; }
	public Color? QuestBarIncompleteColor { get; set; }
	public Color? QuestBarIncompleteDarkColor { get; set; }
	public Color? QuestBarCompleteColor { get; set; }
	public Color? QuestBarCompleteDarkColor { get; set; }

	#endregion

	#region ShopMenu

	public Color? ShopSelectedColor { get; set; }
	public Color? ShopQiSelectedColor { get; set; }

	#endregion

	#region SkillsPage

	public Color? SkillsPageTextColor { get; set; }
	public Color? SkillsPageModifiedNumberColor { get; set; }
	public Color? SkillsPageNumberColor { get; set; }

	#endregion

	#region SpecialOrdersBoard

	public Color? SpecialOrdersHoverColor { get; set; }

	#endregion

	#region TutorialMenu

	public Color? TutorialHoverColor { get; set; }

	#endregion

	internal static BaseTheme GetDefaultTheme() {
		var theme = new BaseTheme() {
			// Generic Colors
			TextColor = Game1.textColor,
			TextShadowColor = Game1.textShadowColor,
			TextShadowAltColor = new Color(221, 148, 84),

			ErrorTextColor = Color.Red,
			HoverColor = Color.Wheat,
			ButtonHoverColor = Color.LightPink,

			UnselectedOptionColor = Game1.unselectedOptionColor,
		};

		for(int i = -1; i <= 8; i++)
			theme.SpriteTextColors.Add(i, SpriteText.getColorFromIndex(i));

		return theme;
	}

}
