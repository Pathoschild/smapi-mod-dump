/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/desto-git/smapi-RegularQuality
**
*************************************************/

namespace BillboardProfitMargin
{
	using Common;
	using StardewModdingAPI;
	using StardewModdingAPI.Events;
	using StardewValley;
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
				Logger.Error("Error in config.json: \"CustomQuestMargin\" must be at least 0.");
				Logger.Error("Deactivating mod");
				return;
			}

			helper.Events.GameLoop.DayStarted += this.OnDayStarted;
			helper.Events.Display.MenuChanged += this.OnMenuChanged;
		}

		// update only the quest description initially
		// once the quest is completed, it needs to be updated again along with the reward
		private void UpdateItemDeliveryQuest(ItemDeliveryQuest quest)
		{
			if (quest.deliveryItem.Value == null)
			{
				Logger.Trace("Can not adjust reward for daily quest that is managed by Quest Framework.");
				return;
			}

			// item delivery quests don't have a reward property
			// instead, the reward is calculated from the item being requested once the quest has been completed
			// this assumes that the reward is always three times the item value
			int originalReward = quest.deliveryItem.Value.Price * 3;
			int adjustedReward = QuestHelper.GetAdjustedReward(originalReward, this.config);

			if (QuestHelper.GetReward(quest) == adjustedReward) return;

			// replace values in the quest text
			QuestHelper.UpdateDescription(quest, originalReward, adjustedReward);

			// true once the reward can be collected from the quest log
			if (!quest.hasReward()) return;

			QuestHelper.SetReward(quest, adjustedReward);
		}

		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			// wait for Quest Framework to potentially initialize a quest
			this.Helper.Events.GameLoop.UpdateTicked += this.OnDayStartedDelayed;
		}

		private void OnMenuChanged(object sender, MenuChangedEventArgs e)
		{
			if (e.NewMenu is QuestLog)
			{
				foreach (ItemDeliveryQuest quest in QuestLogHelper.GetDailyItemDeliveryQuests())
				{
					this.UpdateItemDeliveryQuest(quest);
				}
			}
		}

		private void OnDayStartedDelayed(object sender, UpdateTickedEventArgs e)
		{
			this.Helper.Events.GameLoop.UpdateTicked -= this.OnDayStartedDelayed;

			Quest dailyQuest = Game1.questOfTheDay;
			if (dailyQuest == null) return;

			if (dailyQuest is ItemDeliveryQuest itemDeliveryQuest)
			{
				itemDeliveryQuest.loadQuestInfo();
				this.UpdateItemDeliveryQuest(itemDeliveryQuest);
				return;
			}

			QuestHelper.AdjustRewardImmediately(dailyQuest, this.config);
		}
	}
}