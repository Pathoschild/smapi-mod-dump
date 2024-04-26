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
	using System;
	using Common;
	using StardewValley;
	using StardewValley.Quests;

	/// <summary>Utility functions to work with the various quest types.</summary>
	internal static class QuestHelper
	{
		/// <summary>Get the monetary reward of a quest.</summary>
		/// <param name="genericQuest">Quest to get the reward from.</param>
		/// <returns>Monetary reward for the quest.</returns>
		public static int GetReward(Quest genericQuest)
		{
			switch (genericQuest)
			{
				case ItemDeliveryQuest quest:
					return quest.moneyReward.Value;
				case FishingQuest quest:
					return quest.reward.Value;
				case ResourceCollectionQuest quest:
					return quest.reward.Value;
				case SlayMonsterQuest quest:
					return quest.reward.Value;
				default:
					throw new Exception("Can not get reward of unsupported quest type: " + genericQuest.ToString());
			}
		}

		/// <summary>Set the monetary reward of a quest.</summary>
		/// <param name="genericQuest">Quest to set the reward of.</param>
		/// <param name="reward">Amount of gold.</param>
		public static void SetReward(Quest genericQuest, int reward)
		{
			switch (genericQuest)
			{
				case SlayMonsterQuest quest:
					quest.reward.Value = reward;
					break;
				case FishingQuest quest:
					quest.reward.Value = reward;
					break;
				case ResourceCollectionQuest quest:
					quest.reward.Value = reward;
					break;
				case ItemDeliveryQuest quest:
					quest.moneyReward.Value = reward;
					break;
				default:
					throw new Exception("Can not set reward for unsupported quest type: " + genericQuest.ToString());
			}
		}

		/// <summary>Update mentions of the reward in the quest description.</summary>
		/// <param name="quest">Quest to update the description of.</param>
		/// <param name="originalReward">Amount of gold this quest originally promised.</param>
		/// <param name="adjustedReward">Amount of gold this quest will now grant.</param>
		public static void UpdateDescription(Quest quest, int originalReward, int adjustedReward)
		{
			string originalRewardString = originalReward + "g";
			string adjustedRewardString = adjustedReward + "g";
			quest.questDescription = quest.questDescription.Replace(originalRewardString, adjustedRewardString);
		}

		/// <summary>Get the adjusted reward of a quest.</summary>
		/// <param name="originalReward">Amount of gold this quest originally promised.</param>
		/// <param name="config">Config object.</param>
		/// <returns>Adjusted reward value.</returns>
		public static int GetAdjustedReward(int originalReward, ModConfig config)
		{
			float rewardMultiplier = config.UseProfitMargin
				? Game1.player.difficultyModifier
				: config.CustomProfitMargin;

			return (int)Math.Ceiling(originalReward * rewardMultiplier);
		}

		/// <summary>Adjust the reward for quest types that support it.</summary>
		/// <param name="genericQuest">Quest to adjust.</param>
		/// <param name="config">Config object.</param>
		public static void AdjustRewardImmediately(Quest genericQuest, ModConfig config)
		{
			if (genericQuest is SocializeQuest)
			{
				Logger.Trace("Ignoring quest type SocializeQuest. It has no monetary reward.");
				return;
			}

			int originalReward = GetReward(genericQuest);
			int adjustedReward = GetAdjustedReward(originalReward, config);
			SetReward(genericQuest, adjustedReward);
			UpdateDescription(genericQuest, originalReward, adjustedReward);

			if (GetReward(genericQuest) == adjustedReward)
			{
				Logger.Trace($"Set reward for quest \"{genericQuest.GetName()}\" from {originalReward} to {adjustedReward}.");
			}
			else
			{
				Logger.Error($"Failed to set reward for quest \"{genericQuest.GetName()}\" from {originalReward} to {adjustedReward}.");
			}
		}

		/// <summary>Load quest information if the quest type allows it.</summary>
		/// <param name="genericQuest">Quest to adjust.</param>
		public static void LoadQuestInfo(Quest genericQuest)
		{
			switch (genericQuest)
			{
				case FishingQuest quest:
					quest.loadQuestInfo();
					break;
				case ItemDeliveryQuest quest:
					quest.loadQuestInfo();
					break;
				case ResourceCollectionQuest quest:
					quest.loadQuestInfo();
					break;
				case SlayMonsterQuest quest:
					quest.loadQuestInfo();
					break;
				case SocializeQuest quest:
					quest.loadQuestInfo();
					break;
				default:
					Logger.Warn("Cannot load quest info for unknown quest type: " + genericQuest.ToString()); break;
			}
		}
	}
}
