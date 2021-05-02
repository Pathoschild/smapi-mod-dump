/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dtomlinson-ga/EarlyCommunityUpgrades
**
*************************************************/

namespace EarlyCommunityUpgrades
{
	/// <summary>
	/// Stores config values.
	/// </summary>
	public class ModConfig
	{

		public CostsConfig Costs = new CostsConfig();
		public RequirementsConfig Requirements = new RequirementsConfig();
		public InstantUnlocksConfig InstantUnlocks = new InstantUnlocksConfig();

		public class CostsConfig
		{
			public int pamCostGold = 500000;
			public int pamCostWood = 950;
			public int shortcutCostGold = 300000;
		}

		public class RequirementsConfig
		{
			public int numFarmhouseUpgrades = 3;
			public int numRoomsCompleted = 6;
			public int numFriendshipHeartsGained = 0;
		}

		public class InstantUnlocksConfig
		{
			public bool pamsHouse;
			public bool shortcuts;
		}
	}
}