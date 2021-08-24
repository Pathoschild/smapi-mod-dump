/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dtomlinson-ga/EarlyCommunityUpgrades
**
*************************************************/

using System;

namespace EarlyCommunityUpgrades
{
	/// <summary>
	/// Collection of methods to simplify the process of adding Generic Mod Config Menu options.
	/// </summary>
	class ModConfigMenuHelper
	{
		private static IGenericModConfigMenuAPI api;

		private static readonly string[] pamCostGoldOptions = { "500,000 (Vanilla)", "250,000", "100,000", "50,000", "25,000", "10,000", "5,000", "0", "1,000,000", "2,000,000" };
		private static readonly string[] pamCostWoodOptions = { "950 (Vanilla)", "750", "500", "250", "100", "0", "2000", };
		private static readonly string[] shortcutCostGoldOptions = { "300,000 (Vanilla)", "200,000", "100,000", "50,000", "25,000", "10,000", "5,000", "0", "500,000", "1,000,000" };

		private static readonly string[] numFarmhouseUpgradesOptions = { "3 (Vanilla)", "2", "1", "0" };
		private static readonly string[] numRoomsCompletedOptions = { "6 (Vanilla)", "5", "4", "3", "2", "1", "0" };
		private static readonly string[] numFriendshipHeartsGainedOptions = { "0 (Vanilla)", "5", "10", "15", "20" };

		public static bool TryLoadModConfigMenu()
		{
			try
			{
				// Check to see if Generic Mod Config Menu is installed
				if (!Globals.Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
				{
					Globals.Monitor.Log("GenericModConfigMenu not present - skipping mod menu setup");
					return false;
				}

				api = Globals.Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
				api.RegisterModConfig(Globals.Manifest,
					() => Globals.Config = new ModConfig(),
					() => Globals.Helper.WriteConfig(Globals.Config)
				);

				RegisterModOptions();
				return true;
			}
			catch (Exception e)
			{
				Globals.Monitor.Log("Failed to register GMCM menu - skipping mod menu setup", StardewModdingAPI.LogLevel.Error);
				Globals.Monitor.Log(e.Message, StardewModdingAPI.LogLevel.Error);
				return false;
			}
		}

		public static void RegisterModOptions()
		{
			AddLabel("Early Community Upgrades");

			AddParagraph("Note: You may need to scroll to see all options in some drop-down menus.");

			AddLabel("Order");
			AddCheckBox("Shortcuts First", "Complete shortcuts before building Pam's house",
				() => Globals.Config.Order.shortcutsFirst,
				(bool var) => Globals.Config.Order.shortcutsFirst = var
			);

			AddLabel("Costs");
			AddDropdown("Money Needed For Pam's House", "The amount of money Robin requires to build Pam's new house",
				() => Globals.Config.Costs.pamCostGold.ToString(),
				(string var) => Globals.Config.Costs.pamCostGold = int.Parse(var.Replace(",", "").Replace(" (Vanilla)", "")),
				pamCostGoldOptions
			);

			AddDropdown("Wood Needed For Pam's House", "The amount of wood Robin requires to build Pam's new house",
				() => Globals.Config.Costs.pamCostWood.ToString(),
				(string var) => Globals.Config.Costs.pamCostWood = int.Parse(var.Replace(",", "").Replace(" (Vanilla)", "")),
				pamCostWoodOptions
			);

			AddDropdown("Money Needed For Shortcuts", "The amount of money Robin requires to create the shortcuts",
				() => Globals.Config.Costs.shortcutCostGold.ToString(),
				(string var) => Globals.Config.Costs.shortcutCostGold = int.Parse(var.Replace(",", "").Replace(" (Vanilla)", "")),
				shortcutCostGoldOptions
			);

			AddLabel("Requirements");
			AddDropdown("House Upgrades Needed", "The number of house upgrades required before Robin will give you the option",
				() => Globals.Config.Requirements.numFarmhouseUpgrades.ToString(),
				(string var) => Globals.Config.Requirements.numFarmhouseUpgrades = int.Parse(var.Replace(" (Vanilla)", "")),
				numFarmhouseUpgradesOptions
			);

			AddDropdown("CC/Warehouse Rooms Needed", "The number of Community Center or Joja Warehouse rooms that need to be completed before Robin will give you the option",
				() => Globals.Config.Requirements.numRoomsCompleted.ToString(),
				(string var) => Globals.Config.Requirements.numRoomsCompleted = int.Parse(var.Replace(" (Vanilla)", "")),
				numRoomsCompletedOptions
			);

			AddDropdown("Friendship Hearts Needed", "The total number of hearts required with the townspeople before Robin will give you the option",
				() => Globals.Config.Requirements.numFriendshipHeartsGained.ToString(),
				(string var) => Globals.Config.Requirements.numFriendshipHeartsGained = int.Parse(var.Replace(" (Vanilla)", "")),
				numFriendshipHeartsGainedOptions
			);

			AddLabel("Time");
			AddIntSlider("Number of Days Required", "The numbers of days to pass before the community upgrade is completed.",
				() => Globals.Config.Time.daysUntilCommunityUpgrade,
				(int var) => Globals.Config.Time.daysUntilCommunityUpgrade = var,
				1,
				28
			);

			AddLabel("Instant Unlock");
			AddParagraph("This will override the costs/requirements options above. The upgrade will be unlocked from the very beginning of the game. You will not receive the friendship points from Pam if you choose to unlock her house from the start.");

			AddCheckBox("Pam's House", "Unlocks Pam's house from the start.",
				() => Globals.Config.InstantUnlocks.pamsHouse,
				(bool var) => Globals.Config.InstantUnlocks.pamsHouse = var
			);

			AddCheckBox("Shortcuts", "This will unlock the shortcuts around the valley from the start.",
				() => Globals.Config.InstantUnlocks.shortcuts,
				(bool var) => Globals.Config.InstantUnlocks.shortcuts = var
			);
		}

		private static void AddLabel(string name, string desc = "")
		{
			api.RegisterLabel(Globals.Manifest, name, desc);
		}

		private static void AddParagraph(string text)
		{
			api.RegisterParagraph(Globals.Manifest, text);
		}

		private static void AddDropdown(string name, string desc, Func<string> get, Action<string> set, string[] choices)
		{
			api.RegisterChoiceOption(Globals.Manifest, name, desc, get, set, choices);
		}

		private static void AddCheckBox(string name, string desc, Func<bool> get, Action<bool> set)
		{
			api.RegisterSimpleOption(Globals.Manifest, name, desc, get, set);
		}

		private static void AddIntSlider(string name, string desc, Func<int> get, Action<int> set, int min, int max)
		{
			api.RegisterClampedOption(Globals.Manifest, name, desc, get, set, min, max);
		}
	}
}
