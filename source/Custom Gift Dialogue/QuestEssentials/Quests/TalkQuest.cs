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
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestEssentials.Quests
{
    class TalkQuest : CustomQuest, IQuestObserver, ITriggerLoader
    {
        public string TalkTo { get; set; }

        public bool CheckIfComplete(IQuestInfo questData, ICompletionArgs completion)
        {
            if (questData.VanillaQuest.completed.Value)
                return false;

            if (this.TalkTo != null && completion.Npc != null && completion.Npc.isVillager() && this.TalkTo == completion.Npc.Name)
            {
                if (!string.IsNullOrEmpty(this.ReactionText))
                {
                    completion.Npc.CurrentDialogue.Push(new Dialogue(this.ReactionText, completion.Npc));
                    Game1.drawDialogue(completion.Npc);
                }

                if (Game1.activeClickableMenu is DialogueBox && Game1.currentSpeaker == completion.Npc)
                {
                    questData.VanillaQuest.questComplete();

                    return true;
                }
            }

            return false;
        }

        public void LoadTrigger(string triggerData)
        {
            this.TalkTo = triggerData;
        }

        public void UpdateDescription(IQuestInfo questData, ref string description)
        {
        }

        public void UpdateObjective(IQuestInfo questData, ref string objective)
        {
        }

        public void UpdateTitle(IQuestInfo questData, ref string title)
        {
        }
    }
}
