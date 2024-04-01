/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Quests;

namespace NermNermNerm.Stardew.QuestableTractor
{
    public abstract class BaseQuest : Quest, ISimpleLog
    {
        // Putting this implementation here denies a few other usages, and it also means that our suppressions are
        //  tied to the quest, and thus get tossed out every day.  I can't say if that's a bug or a feature right now.
        private HashSet<string> oldNews = new HashSet<string>();

        /// <summary>
        ///   Hacky way to know if the call to <see cref="CheckIfComplete(NPC, Item?)"/> resulted in a call to <see cref="Spout"/>.
        /// </summary>
        private bool didNpcTalk;

        protected BaseQuest(BaseQuestController controller)
        {
            this.Controller = controller;
            this.SetObjective();
        }

        public BaseQuestController Controller { get; }

        /// <summary>
        ///  Called on either actual or possible interaction with an NPC that could have bearing on a quest.
        /// </summary>
        /// <param name="n">The NPC the player is interacting with - can be null.</param>
        /// <param name="number1"></param>
        /// <param name="number2"></param>
        /// <param name="item"></param>
        /// <param name="str">Seems the same as <paramref name="item"/>.Name in all cases.  Perhaps a holdover from an earlier iteration.</param>
        /// <param name="probe">
        ///   True if the player is hovering over the NPC and (Utility.checkForCharacterInteractionAtTile) and the
        ///   object the player is holding can be given as a gift to that NPC.
        /// </param>
        /// <returns>
        ///  <para>
        ///   Documentation exists for <paramref name="probe"/> in NPC.tryToReceiveActiveObject, which says, regardin the return:
        ///   Whether to return what the method would return if called normally, but without actually accepting the item or making
        ///   any changes to the NPC. This is used to accurately predict whether the NPC would accept or react to the offer.
        ///  </para>
        ///  <para>
        ///   In the case where <paramref name="probe"/> is false and there is a held <paramref name="item"/> and the item's
        ///   isQuest property is true, a false return value will trigger a "Wrong person" error to show up in the HUD.
        ///  </para>
        /// </returns>
        public override sealed bool checkIfComplete(NPC n, int number1, int number2, Item item, string str, bool probe)
        {
            // TODO: Right now, this makes it look like you could talk to any NPC about any of our quest items, when really
            //   there's only a few people who are going to take an interest.  Perhaps it would be better if we made it so
            //   that CheckIfComplete returns an Action?, which we run if probe is false.  Then we could unhack didNpcTalk too.
            if (probe && n is not null && item is not null && this.IsItemForThisQuest(item))
            {
                return true;
            }

            if (probe || n is null)
            {
                return false;
            }

            if (item is not null && !this.IsItemForThisQuest(item))
            {
                return false;
            }

            this.didNpcTalk = false;
            this.CheckIfComplete(n, item);
            return this.didNpcTalk;
        }

        public abstract bool IsItemForThisQuest(Item item);


        public abstract void CheckIfComplete(NPC n, Item? item);

        public void SetDisplayAsNew()
        {
            this.showNew.Value = true;
        }


        public void IndicateQuestHasMadeProgress()
        {
            Game1.playSound("questcomplete"); // Note documentation suggests its for quest complete and "journal update".  That's what we are using it for.
        }

        protected abstract void SetObjective();

        public void Spout(NPC n, string message)
        {
            // This only impacts quest-based messages, and the 'oldNews' thing gets reset once per day.  Not sure if
            // the once-per-day thing is a bug or a feature.
            if (!this.oldNews.Add(n.Name + message))
            {
                return;
            }

            this.didNpcTalk = true;

            // Conversation keys and location specific dialogs take priority.  We can't fix the location-specific
            // stuff, but we can nix conversation topics.

            // Forces it to see if there are Conversation Topics that can be pulled down.
            // Pulling them down toggles their "only show this once" behavior.
            n.checkForNewCurrentDialogue(Game1.player.getFriendshipHeartLevelForNPC(n.Name));

            // Perhaps we should only nix topics that are for this mod?
            // Can (maybe) be culled off of the tail end of 'n.CurrentDialogue.First().TranslationKey'
            n.CurrentDialogue.Clear();

            n.CurrentDialogue.Push(new Dialogue(n, null, message));
            Game1.drawDialogue(n); // <- Push'ing or perhaps the clicking on the NPC causes this to happen anyway. so not sure if it actually helps.
        }

        public void Spout(string message) => BaseQuestController.Spout(message);

        protected void AddQuestItemToInventory(string itemId)
        {
            var item = ItemRegistry.Create<StardewValley.Object>(itemId);
            item.questItem.Value = true;
            Game1.player.addItemByMenuIfNecessary(item);
        }

        protected bool TryTakeItemsFromPlayer(string itemId, int count = 1)
        {
            var stack = Game1.player.Items.FirstOrDefault(i => i?.ItemId == itemId && i.stack.Value >= count);
            if (stack == null)
            {
                return false;
            }
            else if (stack.Stack == count)
            {
                Game1.player.removeItemFromInventory(stack);
                return true;
            }
            else
            {
                stack.Stack -= 3;
                return true;
            }
        }

        protected bool TryTakeItemsFromPlayer(string item1Id, int count1, string item2Id, int count2)
        {
            var stack1 = Game1.player.Items.FirstOrDefault(i => i?.ItemId == item1Id && i.stack.Value >= count1);
            var stack2 = Game1.player.Items.FirstOrDefault(i => i?.ItemId == item2Id && i.stack.Value >= count2);
            if (stack1 is null || stack2 is null)
            {
                return false;
            }

            if (stack1.Stack == count1)
            {
                Game1.player.removeItemFromInventory(stack1);
            }
            else
            {
                stack1.Stack -= count1;
            }

            if (stack2.Stack == count2)
            {
                Game1.player.removeItemFromInventory(stack2);
            }
            else
            {
                stack2.Stack -= count2;
            }

            return true;
        }

        public virtual void WriteToLog(string message, StardewModdingAPI.LogLevel level, bool isOnceOnly)
            => this.Controller.WriteToLog(message, level, isOnceOnly);

        public override void questComplete()
        {
            this.Controller.RawQuestState = BaseQuestController.QuestCompleteStateMagicWord;
            base.questComplete();
        }
    }

    public abstract class BaseQuest<TState> : BaseQuest
        where TState : struct
    {
        public BaseQuest(BaseQuestController<TState> controller) : base(controller) { }

        public new BaseQuestController<TState> Controller => (BaseQuestController<TState>)base.Controller;

        public TState State
        {
            get => this.Controller.State;
            set
            {
                if (!this.State.Equals(value))
                {
                    this.IndicateQuestHasMadeProgress();
                    this.Controller.State = value;
                    this.SetObjective();
                }
            }
        }
    }
}
