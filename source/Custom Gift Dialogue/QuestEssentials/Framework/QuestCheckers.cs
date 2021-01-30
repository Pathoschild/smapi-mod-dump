/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using QuestEssentials.Quests;
using QuestFramework.Extensions;
using QuestFramework.Quests;
using StardewValley;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestEssentials.Framework
{
    internal static class QuestCheckers
    {
        private static IEnumerable<Quest> GetQuestsForCheck<TQuest>() where TQuest : CustomQuest
        {
            return Game1.player
                .questLog
                .Where(q => !q.completed.Value && q.AsManagedQuest() is TQuest);
        }

        public static void CheckEarnQuests(int earnedMoney)
        {
            foreach (var quest in GetQuestsForCheck<EarnMoneyQuest>())
            {
                quest.checkIfComplete(null, earnedMoney, -2, null, "money");
            }
        }

        public static void CheckSellQuests(ISalable itemToSell)
        {
            foreach (var quest in GetQuestsForCheck<SellItemQuest>())
            {
                quest.checkIfComplete(null, itemToSell.Stack, -2, itemToSell as Item, "sell");
            }
        }

        public static void CheckTalkQuests(NPC currentSpeaker)
        {
            var talkQuests = GetQuestsForCheck<TalkQuest>()
                .Where(q => string.IsNullOrEmpty(q.AsManagedQuest().ReactionText));

            foreach (var quest in talkQuests)
            {
                quest.checkIfComplete(currentSpeaker, -1, -2, null, "talk");
            }
        }
    }
}
