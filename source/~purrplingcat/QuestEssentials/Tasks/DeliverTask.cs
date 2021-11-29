/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using QuestEssentials.Framework;
using QuestEssentials.Messages;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestEssentials.Tasks
{
    public class DeliverTask : QuestTask<DeliverTask.DeliverData>
    {
        public struct DeliverData
        {
            public string NpcName { get; set; }
            public string AcceptedContextTags { get; set; }
            public string NotEnoughMessage { get; set; }
            public string Message { get; set; }
        }

        public override bool OnCheckProgress(IStoryMessage message)
        {
            if (this.IsCompleted())
                return false;

            if (message is DeliverMessage deliverMessage)
            {
                if (deliverMessage.Item == null || this.Data.AcceptedContextTags == null || !this.IsWhenMatched())
                    return false;

                if (deliverMessage.Npc.Name != this.Data.NpcName)
                    return false;

                if (Helper.CheckItemContextTags(deliverMessage.Item, this.Data.AcceptedContextTags))
                {
                    int requiredAmount = this.Count - this.CurrentCount;
                    int donatedAmount = Math.Min(deliverMessage.Item.Stack, requiredAmount);
                    string reaction = this.Data.Message;
                    
                    if (donatedAmount < requiredAmount && !string.IsNullOrEmpty(this.Data.NotEnoughMessage))
                    {
                        reaction = string.Format(this.Data.NotEnoughMessage, requiredAmount - donatedAmount);
                    }

                    deliverMessage.Item.Stack -= donatedAmount;
                    this.IncrementCount(donatedAmount);

                    if (!string.IsNullOrEmpty(reaction))
                    {
                        deliverMessage.Npc.CurrentDialogue.Push(new Dialogue(reaction, deliverMessage.Npc));
                        Game1.drawDialogue(deliverMessage.Npc);
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
