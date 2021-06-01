/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using QuestEssentials.Messages;
using QuestFramework.Extensions;
using QuestFramework.Quests;
using QuestFramework.Quests.State;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestEssentials.Quests
{
    class EarnMoneyQuest : CustomQuest<ActiveState>, IQuestInfoUpdater, ITriggerLoader
    {
        public int Goal { get; set; }

        [ActiveState]
        public ActiveStateField<int> Earned { get; } = new ActiveStateField<int>(0);

        public override bool OnCompletionCheck(ICompletionMessage completionMessage)
        {
            if (completionMessage is EarnMoneyMessage earnMessage)
            {
                this.Earned.Value += earnMessage.Amount;

                if (this.Earned.Value >= this.Goal)
                {
                    this.Complete();

                    return true;
                }
            }

            return false;
        }

        public void LoadTrigger(string triggerData)
        {
            this.Goal = Convert.ToInt32(triggerData);
        }

        public void UpdateDescription(IQuestInfo questData, ref string description)
        {
        }

        public void UpdateObjective(IQuestInfo questData, ref string objective)
        {
            objective = $"{this.Goal - this.Earned.Value}g remains to earn.";
        }

        public void UpdateTitle(IQuestInfo questData, ref string title)
        {
        }
    }
}
