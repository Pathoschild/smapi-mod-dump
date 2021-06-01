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
using QuestEssentials.Framework.Factories;
using QuestEssentials.Messages;
using QuestFramework.Messages;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestEssentials.Tasks
{
    class TalkTask : QuestTask<TalkTask.TalkData>
    {
        public struct TalkData
        {
            public string NpcName { get; set; }
            public string DialogueText { get; set; }
            public string StartEvent { get; set; }
            public string ReceiveItems { get; set; }
        }

        /// <summary>
        /// Internal message internally sent by <see cref="DoAdjust(object)"/>
        /// </summary>
        private class TalkRequest : StoryMessage
        {
            public Farmer who;
            public NPC speaker;

            public TalkRequest(Farmer who, NPC speaker) : base("TalkRequest")
            {
                this.who = who;
                this.speaker = speaker;
            }
        }

        public override bool OnCheckProgress(IStoryMessage message)
        {
            if (this.IsCompleted())
                return false;

            if (message is TalkRequest talkRequest && this.IsWhenMatched())
            {
                if (talkRequest.speaker.Name == this.Data.NpcName)
                {
                    var dialogue = new Dialogue(this.Data.DialogueText, talkRequest.speaker)
                    {
                        onFinish = this.OnDialogueSpoken
                    };

                    talkRequest.speaker.CurrentDialogue.Push(dialogue);
                    Game1.drawDialogue(talkRequest.speaker);
                    this.IncrementCount(this.Goal);
                }
            }

            return false;
        }

        private void OnDialogueSpoken()
        {
            Game1.exitActiveMenu();
            Game1.dialogueUp = false;
            Game1.currentSpeaker = null;

            if (this.Data.ReceiveItems != null)
            {
                // Give items from NPC to player
                DelayedAction.functionAfterDelay(delegate
                {
                    string[] itemDescriptions = this.Data.ReceiveItems.Split(',');
                    List<Item> items = itemDescriptions.Select(d => ItemFactory.Create(d))
                        .Where(i => i != null)
                        .ToList();

                    Game1.player.addItemsByMenuIfNecessary(items);
                }, 200);
                Game1.player.freezePause = 250;
            }

            if (this.Data.StartEvent != null)
            {
                // Play event in current location by talk with NPC 
                var eventStartAction = new DelayedAction(220, () => Game1.player.currentLocation.StartEventFrom(this.Data.StartEvent))
                {
                    waitUntilMenusGone = true
                };

                Game1.delayedActions.Add(eventStartAction);
            }
        }

        public override void DoAdjust(object toAdjust)
        {
            if (toAdjust is ITalkMessage talkAdjust && !Game1.dialogueUp && this.IsActive())
            {
                // Quest Framework sends an object to adjust when Farmer requests talk with NPC via NPC.checkAction
                // We catch it and transfer as TalkRequest and call OnCheckProgress with the request as argument
                if (this.IsRegistered() && !this.IsCompleted())
                {
                    this.OnCheckProgress(new TalkRequest(talkAdjust.Farmer, talkAdjust.Npc));
                }
            }
        }

        public override bool ShouldShowProgress()
        {
            return false;
        }
    }
}
