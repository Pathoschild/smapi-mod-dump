/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/desto-git/smapi-RegularQuality
**
*************************************************/

namespace Common
{
	using System.Collections.Generic;
	using StardewValley;
	using StardewValley.Quests;

	/// <summary>Utility functions to work with the various quest types.</summary>
	internal static class QuestLogHelper
	{
		/// <summary>Get a daily quest the player has accepted.</summary>
		/// <returns>Array of daily quests.</returns>
		public static ItemDeliveryQuest[] GetDailyItemDeliveryQuests()
		{
			List<ItemDeliveryQuest> quests = new List<ItemDeliveryQuest>();

			var enumerator = Game1.player.questLog.GetEnumerator();
			while (enumerator.MoveNext())
			{
				// Mailbox quests have an ID greater than 0
				if (enumerator.Current.id.Value == 0 && enumerator.Current is ItemDeliveryQuest itemDeliveryQuest)
				{
					quests.Add(itemDeliveryQuest);
				}
			}

			return quests.ToArray();
		}
	}
}
