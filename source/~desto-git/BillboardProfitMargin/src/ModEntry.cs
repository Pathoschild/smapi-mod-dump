/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/desto-git/sdv-mods
**
*************************************************/

namespace BillboardProfitMargin
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using Common;
	using StardewModdingAPI;
	using StardewModdingAPI.Events;
	using StardewValley;
	using StardewValley.GameData.SpecialOrders;
	using StardewValley.Menus;
	using StardewValley.Quests;

	/// <summary>Main class.</summary>
	internal class ModEntry : Mod
	{
		private ModConfig config;

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			Logger.Init(this.Monitor);

			this.config = this.Helper.ReadConfig<ModConfig>();
			if (this.config.CustomProfitMargin < 0)
			{
				Logger.Error("Error in config.json: \"CustomProfitMargin\" must be at least 0.");
				return;
			}

			if (this.config.CustomProfitMarginForSpecialOrders < 0)
			{
				Logger.Error("Error in config.json: \"CustomProfitMarginForSpecialOrders\" must be at least 0.");
				return;
			}

			helper.Events.Content.AssetRequested += this.OnAssetRequested;
			helper.Events.GameLoop.DayStarted += this.OnDayStarted;
			helper.Events.Display.MenuChanged += this.OnMenuChanged;
		}

		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			// wait for Quest Framework to potentially initialize a quest
			this.Helper.Events.GameLoop.UpdateTicked += this.OnDayStartedDelayed;
		}

		private void OnMenuChanged(object sender, MenuChangedEventArgs e)
		{
			// The description would reset when a quest gets completed, so we set it every time it is viewed.
			if (!(e.NewMenu is QuestLog)) return;

			var enumerator = Game1.player.questLog.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Quest quest = enumerator.Current;
				if (quest.id.Value != null) continue; // Daily quests have no ID

				int currentReward = QuestHelper.GetReward(quest);
				Match match = Regex.Match(quest.questDescription, "[0-9]+g");

				if (match.Value != $"{currentReward}g")
				{
					int originalReward = int.Parse(match.Value.Substring(0, match.Length - 1));
					QuestHelper.UpdateDescription(quest, originalReward, QuestHelper.GetReward(quest));
					Logger.Trace($"Updated quest description for \"{quest.GetName()}\" to match the actual reward.");
				}
			}
		}

		private void OnDayStartedDelayed(object sender, UpdateTickedEventArgs e)
		{
			this.Helper.Events.GameLoop.UpdateTicked -= this.OnDayStartedDelayed;

			Quest dailyQuest = Game1.questOfTheDay;
			if (dailyQuest == null) return;

			QuestHelper.LoadQuestInfo(dailyQuest);
			QuestHelper.AdjustRewardImmediately(dailyQuest, this.config);
		}

		private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
		{
			if (e.NameWithoutLocale.IsEquivalentTo("Data/SpecialOrders"))
			{
				var specialOrderMultiplier = this.config.UseProfitMarginForSpecialOrders
				? Game1.player.difficultyModifier
				: this.config.CustomProfitMarginForSpecialOrders;

				e.Edit(assetData =>
				{
					// update monetary rewards for special order quests
					IDictionary<string, SpecialOrderData> quests = assetData.AsDictionary<string, SpecialOrderData>().Data;

					// https://stackoverflow.com/a/31767807
					// .ToList is part of System.Linq
					// Without it, the loop would error after an assignment to a dictionary element
					foreach (KeyValuePair<string, SpecialOrderData> questData in quests)
					{
						SpecialOrderData quest = questData.Value;
						foreach (SpecialOrderRewardData reward in quest.Rewards)
						{
							if (reward.Type != "Money") continue;

							Dictionary<string, string> data = reward.Data;

							if (!data.ContainsKey("Amount")) throw new Exception("Could not get 'Amount' for special order quest.");
							string amount = data["Amount"];

							// amount is dictated by the requested resource with a multiplier
							if (amount.StartsWith("{"))
							{
								// There is actually nothing to do here.
								// The base price is already taking the profit margin into account.
							}

							// reward is a fixed gold amount
							else
							{
								int newAmount = (int)Math.Ceiling(int.Parse(amount) * specialOrderMultiplier);
								data["Amount"] = newAmount.ToString();
							}
						}
					}
				});
			}
		}
	}
}