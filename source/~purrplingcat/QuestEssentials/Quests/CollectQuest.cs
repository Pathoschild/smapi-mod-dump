/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using QuestFramework.Extensions;
using QuestFramework.Quests;
using QuestFramework.Quests.State;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestEssentials.Quests
{
    /// <summary>
    /// Good and working replacement for vanilla harvest quest type
    /// </summary>
    public class CollectQuest : CustomQuest<ActiveState>, ITriggerLoader, IQuestInfoUpdater
    {
        [ActiveState]
        public readonly ActiveStateField<int> collectedCount = new ActiveStateField<int>();

        public int ItemIndex { get; set; }
        public int RequiredCount { get; set; } = 1;

        public void LoadTrigger(string triggerData)
        {
            if (string.IsNullOrEmpty(triggerData))
                return;

            string[] split = triggerData.Split(' ');

            if (split.Length < 1)
                return;

            this.ItemIndex = Convert.ToInt32(split[0]);
            this.RequiredCount = split.Length >= 2 ? Convert.ToInt32(split[1]) : 1;
        }

        public override bool OnCompletionCheck(ICompletionMessage completionMessage)
        {
            bool worked = false;

            if (completionMessage is ICompletionArgs args && args.CompletionType == Quest.type_resource)
            {
                if (args.Item != null && args.Item.ParentSheetIndex == this.ItemIndex)
                {
                    this.collectedCount.Value += args.Item.Stack;
                    worked = true;
                }
            }

            if (this.collectedCount.Value >= this.RequiredCount)
            {
                this.Complete();
                worked = true;
            }

            return worked;
        }

        public void UpdateDescription(IQuestInfo questData, ref string description)
        {
        }

        public void UpdateObjective(IQuestInfo questData, ref string objective)
        {
            if (this.RequiredCount > 1)
            {
                objective = $"{this.Objective} ({this.collectedCount.Value}/{this.RequiredCount})";
            }
        }

        public void UpdateTitle(IQuestInfo questData, ref string title)
        {
        }
    }
}
