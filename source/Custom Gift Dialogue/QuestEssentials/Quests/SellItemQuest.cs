/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using QuestFramework.Quests;
using QuestFramework.Quests.State;
using StardewValley;

namespace QuestEssentials.Quests
{
    public class SellItemQuest : CustomQuest<ActiveState>, ITriggerLoader, IQuestObserver
    {
        public int ItemToSellIndex { get; set; }
        public int Count { get; set; } = 1;

        [ActiveState]
        public ActiveStateField<int> ItemsSold { get; } = new ActiveStateField<int>(0);

        public virtual bool CheckIfComplete(IQuestInfo questData, ICompletionArgs completion)
        {
            if (questData.VanillaQuest.completed.Value
                || completion.String != "sell"
                || completion.Item?.ParentSheetIndex != this.ItemToSellIndex
                || completion.Number1 <= 0)
            {
                return false;
            }

            this.ItemsSold.Value += completion.Number1;

            if (this.ItemsSold.Value >= this.Count)
                return true;

            return false;
        }

        public void LoadTrigger(string triggerData)
        {
            int[] fragments = Utility.parseStringToIntArray(triggerData);

            if (fragments.Length > 0)
            {
                this.ItemToSellIndex = fragments[0];
                this.Count = fragments.Length >= 2 ? fragments[1] : 1;
            }
        }

        public virtual void UpdateDescription(IQuestInfo questData, ref string description)
        {
        }

        public virtual void UpdateObjective(IQuestInfo questData, ref string objective)
        {
            if (this.ItemsSold.Value == 0)
                objective = this.Objective;
            else
            {
                string itemName = Game1.objectInformation[this.ItemToSellIndex].Split('/')[4];

                objective = $"{this.ItemsSold.Value}/{this.Count} {itemName} sold.";
            }
        }

        public virtual void UpdateTitle(IQuestInfo questData, ref string title)
        {
        }
    }
}
