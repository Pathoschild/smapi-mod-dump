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
		private bool hasRewardBeenAdjusted = false;
		private bool isItemDeliveryQuest = false;

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

			helper.Events.GameLoop.ReturnedToTitle += (object sender, ReturnedToTitleEventArgs e) => this.ResetState();
			helper.Events.GameLoop.DayEnding += (object sender, DayEndingEventArgs e) => this.ResetState();
			helper.Events.GameLoop.DayStarted += this.OnDayStarted;
			helper.Events.Display.MenuChanged += this.OnMenuChanged;
		}

		// update only the quest description initially
		// once the quest is completed, it needs to be updated again along with the reward
		private void UpdateItemDeliveryQuest(ItemDeliveryQuest quest)
		{
			this.isItemDeliveryQuest = true;

			// item delivery quests don't have a reward property
			// instead, the reward is calculated from the item being requested
			// this assumes that the reward is always three times the item value
			int originalReward = quest.deliveryItem.Value.Price * 3;
			int adjustedReward = QuestHelper.GetAdjustedReward(originalReward, this.config);

			// replace values in the quest text
			QuestHelper.UpdateDescription(quest, originalReward, adjustedReward);

			// once the quest is completed, the reward is set
			if (QuestHelper.GetReward(quest) == 0) return;

			QuestHelper.SetReward(quest, adjustedReward);

			this.hasRewardBeenAdjusted = true;
		}

		private void ResetState()
		{
			this.hasRewardBeenAdjusted = false;
			this.isItemDeliveryQuest = false;
		}

		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			Quest dailyQuest = Game1.questOfTheDay;
			if (dailyQuest == null) return;

			if (dailyQuest is ItemDeliveryQuest itemDeliveryQuest)
			{
					itemDeliveryQuest.loadQuestInfo();
					this.UpdateItemDeliveryQuest(itemDeliveryQuest);
					return;
			}

			QuestHelper.AdjustRewardImmediately(dailyQuest, this.config);
			this.hasRewardBeenAdjusted = true;
		}

		private void OnMenuChanged(object sender, MenuChangedEventArgs e)
		{
			if (this.hasRewardBeenAdjusted) return;

			if (this.isItemDeliveryQuest && e.NewMenu is QuestLog)
			{
				this.UpdateItemDeliveryQuest((ItemDeliveryQuest)Game1.questOfTheDay);
			}
		}
	}
}