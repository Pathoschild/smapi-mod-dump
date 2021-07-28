/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

namespace LoveOfCooking
{
	public class Config
	{
		public bool AddCookingMenu { get; set; } = true;
		public bool AddCookingCommunityCentreBundles { get; set; } = false;
		public bool AddCookingSkillAndRecipes { get; set; } = true;
		public bool AddCookingToolProgression { get; set; } = true;
		//public bool AddCookingQuestline { get; set; } = true;
		public bool AddNewCropsAndStuff { get; set; } = true;
		public bool AddRecipeRebalancing { get; set; } = true;
		public bool AddBuffReassigning { get; set; } = false;
		public bool PlayCookingAnimation { get; set; } = true;
		public bool HideFoodBuffsUntilEaten { get; set; } = true;
		public bool FoodHealingTakesTime { get; set; } = false;
		public bool FoodCanBurn { get; set; } = false;
		public bool DebugMode { get; set; } = false;
		public bool DebugRegenTracker { get; set; } = false;
		public string ConsoleCommandPrefix { get; set; } = "cac";
		public bool ResizeKoreanFonts { get; set; } = true;
	}
}
