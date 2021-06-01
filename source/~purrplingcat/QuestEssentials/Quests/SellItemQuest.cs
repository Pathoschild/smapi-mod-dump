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

namespace QuestEssentials.Quests
{
    public class SellItemQuest : CustomQuest<ActiveState>, ITriggerLoader, IQuestInfoUpdater
    {
        public int ItemToSellIndex { get; set; }
        public string ItemName { get; set; }
        public int Count { get; set; } = 1;
        public bool OnlyShip { get; set; } = false;


        [ActiveState]
        public ActiveStateField<int> ItemsSold { get; } = new ActiveStateField<int>(0);

        public override bool OnCompletionCheck(ICompletionMessage completionMessage)
        {
            if (completionMessage is SellItemMessage sellMessage)
            {
                if (sellMessage.Item.Stack <= 0)
                    return false;

                if (this.OnlyShip && !sellMessage.Ship)
                    return false;

                if ((this.ItemName != null && this.ItemName == sellMessage.Item.Name) || sellMessage.Item.ParentSheetIndex == this.ItemToSellIndex)
                {
                    this.ItemsSold.Value += sellMessage.Item.Stack;
                }

                if (this.ItemsSold.Value >= this.Count)
                {
                    this.Complete();

                    return true;
                }
            }

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
