/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace EndgameBundles
{
	public class ModEntry : Mod
	{
		//public static readonly Dictionary<int, bool> bundles;
		//public static Dictionary<string, string>? bundleData = null!;

		List<EndgameBundleSet> bundleSets = new List<EndgameBundleSet>();

		//Dictionary<string, EndgameBundleSet_Old>? bundlesets = null!;

		public override void Entry(IModHelper helper)
		{
			helper.Events.Input.ButtonPressed += OnButtonPressed;
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
		}

		private void OnGameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
		{
			Dictionary<string, EndgameBundleSet>? bundleData = Helper.Data.ReadJsonFile<Dictionary<string, EndgameBundleSet>>("assets/BundleDataModel.json");

			if (bundleData is null)
			{
				Monitor.Log("Failed to load the bundle data", LogLevel.Error);
			}
			else
			{
				ParseBundleData(bundleData);
			}
		}

		private void OnButtonPressed(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
		{
			if (e.Button is SButton.J)
			{
				Monitor.Log("Player pressed J, opening the Bundle Menu", LogLevel.Info);

				Game1.activeClickableMenu = new EndgameBundleMenu(Helper, Monitor, bundleSets);
			}
		}


		private void ParseBundleData(Dictionary<string, EndgameBundleSet> bundleData)
		{
			foreach (KeyValuePair<string, EndgameBundleSet> kvp in bundleData)
			{
				bundleSets.Add(kvp.Value);
			}
		}
	}

	public class EndgameBundleSet
	{
		public string ID { get; set; } = string.Empty;
		public List<EndgameBundle> Bundles { get; set; } = new List<EndgameBundle>();

		// Not auto filled
		public bool Completed { get; set; }
		public readonly List<int> UnclaimedRewards = new List<int>();
	}


	public class EndgameBundle
	{
		public string Name { get; set; } = string.Empty;
		public int MinRequiredAmount { get; set; }
		public EndgameBundleReward Reward { get; set; } = new EndgameBundleReward();
		public List<EndgameBundleIngredient> Ingredients { get; set; } = new List<EndgameBundleIngredient>();

		// Not auto filled
		public bool Completed { get; set; }
	}


	public class EndgameBundleReward
	{
		public string ItemType { get; set; } = string.Empty;
		public int ItemID { get; set; }
		public int Amount { get; set; }

		/// <summary>
		/// Converts the object to a string that can be used to generate the Stardew object
		/// </summary>
		/// <returns>Returns the object in "ItemType ItemID Amount"</returns>
		public override string ToString()
		{
			return $"{ItemType} {ItemID} {Amount}";
		}
	}


	public class EndgameBundleIngredient
	{
		public int ItemID { get; set; }
		public int Amount { get; set; }
		public int Quality { get; set; }

		// Not auto filled
		public bool Completed { get; set; }
	}
}
