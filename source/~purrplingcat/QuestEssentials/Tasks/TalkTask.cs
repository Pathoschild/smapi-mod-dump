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
using QuestEssentials.Quests;
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
        private List<Item> _itemsToRemove;

        public struct TalkData
        {
            public string NpcName { get; set; }
            public string DialogueText { get; set; }
            public string StartEvent { get; set; }
            public string ReceiveItems { get; set; }
            public string RequiredItems { get; set; }
            public bool KeepRequiredItems { get; set; }
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

            if (message is DeliverMessage deliverMessage && this.IsWhenMatched() && this._itemsToRemove.Any(i => i.ParentSheetIndex == deliverMessage.Item.ParentSheetIndex))
            {
                return this.DoTalk(deliverMessage.Npc, deliverMessage.Who);
            }

            if (message is TalkRequest talkRequest && this.IsWhenMatched())
            {
                return this.DoTalk(talkRequest.speaker, talkRequest.who);
            }

            return false;
        }

        private bool DoTalk(NPC speaker, Farmer who)
        {
            if (speaker.Name == this.Data.NpcName && this.CheckItemsInInventory(who))
            {
                var dialogue = new Dialogue(this.Data.DialogueText, speaker)
                {
                    onFinish = this.OnDialogueSpoken
                };

                if (!this.Data.KeepRequiredItems)
                {
                    foreach (Item toRemove in this._itemsToRemove)
                    {
                        Game1.player.removeFirstOfThisItemFromInventory(toRemove.ParentSheetIndex);
                    }
                }

                if (who.ActiveObject == null)
                {
                    who.showNotCarrying();
                }

                speaker.CurrentDialogue.Push(dialogue);
                Game1.drawDialogue(speaker);
                this.IncrementCount(this.Goal);

                return true;
            }

            return false;
        }

        private bool CheckItemsInInventory(Farmer who)
        {
            foreach (Item item in this._itemsToRemove)
            {
                if (!who.hasItemInInventory(item.ParentSheetIndex, 1))
                {
                    return false;
                }
            }

            return true;
        }

        private void OnDialogueSpoken()
        {
            Game1.exitActiveMenu();
            Game1.dialogueUp = false;
            Game1.currentSpeaker = null;
            Game1.player.freezePause = 50;

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

        public override void Register(SpecialQuest quest)
        {
            base.Register(quest);

            this._itemsToRemove = new List<Item>();

            if (this.Data.RequiredItems != null)
            {
                string[] itemsToRemove = this.Data.RequiredItems.Split(',');
                foreach (string toRemove in itemsToRemove)
                {
                    var item = ItemFactory.Create(toRemove);

                    if (item != null)
                    {
                        this._itemsToRemove.Add(item);
                    }
                }
            }
        }

        public override bool ShouldShowProgress()
        {
            return false;
        }
    }
}
