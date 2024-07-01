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
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;

namespace NermNermNerm.Stardew.QuestableTractor
{
    /// <summary>
    ///   This class represents aspects of the class that need monitoring whether or not the player is
    ///   actually on the quest.  For example, if there are triggers that start the quest, the controller
    ///   will detect them.  The controller is also the place that knows whether the quest has been
    ///   completed or not.
    /// </summary>
    /// <remarks>
    ///   Quest Controllers should be constructed when the mod is initialized (in <code>Mod.Entry</code>)
    ///   and they are never destroyed.
    /// </remarks>
    public abstract class BaseQuestController : ISimpleLog
    {
        private readonly Dictionary<string, Action<Item>> itemsToWatch = new();
        private bool isWatchingInventory;

        protected BaseQuestController(ModEntry mod)
        {
            this.Mod = mod;
        }

        public readonly static string QuestCompleteStateMagicWord = I("Complete");

        public ModEntry Mod { get; }

        protected abstract string ModDataKey { get; }

        public bool IsStarted => this.OverallQuestState != OverallQuestState.NotStarted;
        public bool IsComplete => this.OverallQuestState == OverallQuestState.Completed;

        public bool IsCompletedByMasterPlayer => this.GetOverallQuestState(Game1.MasterPlayer) == OverallQuestState.Completed;
        public bool IsStartedByMasterPlayer => this.GetOverallQuestState(Game1.MasterPlayer) != OverallQuestState.NotStarted;

        public static void Spout(string message)
        {
            Game1.DrawDialogue(new Dialogue(null, null, message));
        }

        /// <summary>
        ///   Attempt to fix a player in a broken state.
        /// </summary>
        public abstract void Fix();

        protected void EnsureInventory(string itemId, bool shouldContain)
        {
            if (shouldContain)
            {
                this.EnsureItemIsInInventory(itemId);
            }
            else
            {
                this.EnsureItemIsNotInInventory(itemId);
            }
        }

        protected void EnsureItemIsInInventory(string itemId)
        {
            string qiid = ItemRegistry.QualifyItemId(itemId)!;
            if (!Game1.player.Items.Any(i => i?.QualifiedItemId == qiid))
            {
                var restoredItem = ItemRegistry.Create(qiid);
                if (restoredItem is StardewValley.Object o)
                {
                    o.questItem.Value = true;
                }
                Game1.player.addItemToInventory(restoredItem);
            }
        }

        protected void EnsureItemIsNotInInventory(string itemId)
        {
            string qiid = ItemRegistry.QualifyItemId(itemId)!;
            while (Game1.player.Items.Any(i => i?.QualifiedItemId == qiid))
            {
                Game1.player.removeFirstOfThisItemFromInventory(qiid);
            }
        }

        /// <summary>
        ///   Conversation keys are managed mod-wide, as there's a complex interplay, where
        ///   the main-quest conversation key is thrown up until that one gets started,
        ///   and then the part quest hints get dribbled out later.
        /// </summary>
        public virtual string? HintTopicConversationKey { get; } = null;

        /// <summary>
        ///   Registers for watching for an item appearing in the player's inventory.
        ///   The assumption is that <paramref name="itemId"/> is a quest item, and
        ///   <paramref name="onItemAddedToHostPlayer"/> will only be called if the host player
        ///   gets the item.  For non-host players, there's a dialog telling them to give
        ///   the item to the host.
        /// </summary>
        /// <param name="itemId">The item to watch for</param>
        /// <param name="onItemAddedToHostPlayer">A callback for when the host player gets the item.</param>
        protected void MonitorInventoryForItem(string itemId, Action<Item> onItemAddedToHostPlayer)
        {
            if (!this.isWatchingInventory)
            {
                this.LogTrace($"{this.GetType().Name} Started monitoring inventory changes");
                this.Mod.Helper.Events.Player.InventoryChanged += this.Player_InventoryChanged;
                this.isWatchingInventory = true;
            }

            if (!this.itemsToWatch.ContainsKey(itemId))
            {
                this.LogTrace($"{this.GetType().Name} Started monitoring inventory for {itemId}");
            }

            this.itemsToWatch[itemId] = onItemAddedToHostPlayer;
        }

        protected void StopMonitoringInventoryFor(string itemId)
        {
            if (this.itemsToWatch.Remove(itemId))
            {
                this.LogTrace($"{this.GetType().Name} Stopped monitoring inventory for {itemId}");
            }

            if (!this.itemsToWatch.Any() && this.isWatchingInventory)
            {
                this.LogTrace($"{this.GetType().Name} Stopped monitoring inventory changes");
                this.Mod.Helper.Events.Player.InventoryChanged -= this.Player_InventoryChanged;
                this.isWatchingInventory = false;
            }
        }

        private void Player_InventoryChanged(object? sender, StardewModdingAPI.Events.InventoryChangedEventArgs e)
        {
            foreach (var item in e.Added)
            {
                if (this.itemsToWatch.TryGetValue(item.ItemId, out var handler))
                {
                    if (!e.Player.IsMainPlayer)
                    {
                        e.Player.holdUpItemThenMessage(item, true);
                        Spout(L("This item is for unlocking the tractor - only the host can advance this quest.  Give this item to the host.  (You have to put in a chest for them.)"));
                    }
                    else
                    {
                        handler(item);
                    }
                }
            }
        }

        public string? RawQuestState
        {
            get => this.GetRawQuestState(Game1.player);
            set
            {
                this.SetRawQuestState(Game1.player, value);
            }
        }


        public string? GetRawQuestState(Farmer player)
        {
            player.modData.TryGetValue(this.ModDataKey, out string storedValue);
            return storedValue;
        }

        public void SetRawQuestState(Farmer player, string? value)
        {
            if (player != Game1.MasterPlayer)
            {
                throw new NotImplementedException(I("QuestableTractorMod quests should only be playable by the main player"));
            }

            bool wasChanged;
            if (value is null)
            {
                wasChanged = player.modData.Remove(this.ModDataKey);
            }
            else
            {
                player.modData.TryGetValue(this.ModDataKey, out string? oldValue);
                player.modData[this.ModDataKey] = value;
                wasChanged = (value != oldValue);
            }

            if (wasChanged)
            {
                if (value is null)
                {
                    this.LogTrace($"Cleared {player.Name}'s ModData[{this.ModDataKey}]");
                }
                else
                {
                    this.LogTrace($"Set {player.Name}'s ModData[{this.ModDataKey}] to '{value}'");
                }
                this.OnStateChanged();
            }
        }

        /// <summary>
        ///  This method is called when the state changes and also when the game is loaded.
        /// </summary>
        protected virtual void OnStateChanged() { }

        public OverallQuestState OverallQuestState => this.GetOverallQuestState(Game1.player);

        public OverallQuestState GetOverallQuestState(Farmer player)
        {
            string? state = this.GetRawQuestState(player);
            if (state is null)
                return OverallQuestState.NotStarted;
            else if (state == QuestCompleteStateMagicWord)
                return OverallQuestState.Completed;
            else
                return OverallQuestState.InProgress;
        }

        /// <summary>
        ///   This is a hacky way to deal with quest completion until something more clever can be thought up.
        ///   Right now this gets called in the 1-second-tick callback.  It returns true if the item resulted
        ///   in quest completion and the tractor config should be rebuilt.
        /// </summary>
        public virtual void PlayerIsInGarage(Item itemInHand) {}

        public virtual void WriteToLog(string message, LogLevel level, bool isOnceOnly)
            => ((ISimpleLog)this.Mod).WriteToLog(message, level, isOnceOnly);

        /// <summary>
        ///   Creates a new instance of the Quest object, assuming the State is correct.
        /// </summary>
        /// <remarks>
        ///   Perhaps it should also take the role of ensuring that the state is actually valid
        ///   and correcting it if not.
        /// </remarks>
        protected abstract BaseQuest CreateQuest();

        protected abstract string InitialQuestState { get; }

        /// <summary>
        ///   Creates a new instance of the Quest object, assuming the State empty.
        /// </summary>
        public void CreateQuestNew(Farmer player)
        {
            this.RawQuestState = this.InitialQuestState;
            var quest = this.CreateQuest();
            quest.SetDisplayAsNew();
            FakeQuest.AddToQuestLog(player, quest);
        }

        public BaseQuest? GetQuest(Farmer player) => FakeQuest.GetFakeQuestByController(player, this);

        /// <summary>
        ///   Called once at the start of every day when the quest is not started.
        /// </summary>
        /// <remarks>
        ///   Implementations should ensure that the triggers necessary to start the quest are present.
        /// </remarks>
        protected virtual void OnDayStartedQuestNotStarted() { }

        /// <summary>
        ///   Called once at the start of every day when the quest is in progress.
        /// </summary>
        /// <remarks>
        ///   Implementations should use this to ensure that items are placed correctly and do any
        ///   day-over-day quest advancements.
        /// </remarks>
        protected virtual void OnDayStartedQuestInProgress() { }

        public void OnDayStarted()
        {
            // The promise is that we call this when the state changes and on initial load...  But right now
            // we haven't hooked into that event and, in any case, OnStateChange shouldn't suffer from being
            // called more frequently than needed, so here it is.
            this.OnStateChanged();

            switch (this.OverallQuestState)
            {
                case OverallQuestState.NotStarted:
                    this.OnDayStartedQuestNotStarted();
                    break;
                case OverallQuestState.InProgress:
                    this.OnDayStartedQuestInProgress();
                    break;
                case OverallQuestState.Completed:
                    // We don't have any quests that could use an on-complete event...
                    break;
            }

            // Re-test the state beause it might have completed overnight
            if (Game1.IsMasterGame && this.OverallQuestState == OverallQuestState.InProgress)
            {
                var newQuest = this.CreateQuest();
                newQuest.MarkAsViewed();
                FakeQuest.AddToQuestLog(Game1.player, newQuest);
            }
        }
    }

    public abstract class BaseQuestController<TQuestState>
        : BaseQuestController
        where TQuestState : struct
    {
        public BaseQuestController(ModEntry mod) : base(mod) { }

        protected override string InitialQuestState => default(TQuestState).ToString()!;

        public TQuestState State
        {
            get => this.GetState(Game1.player);
            set
            {
                this.RawQuestState = value.ToString();
            }
        }

        public TQuestState GetState(Farmer player)
        {
            string? rawState = this.GetRawQuestState(player);
            if (rawState == null)
            {
                throw new InvalidOperationException(I("State should not be queried when the quest isn't started"));
            }

            if (!this.TryParse(rawState, out TQuestState result))
            {
                // Part of the design of the state enums should include making sure that the default value of
                // the enum is the starting condition of the quest, so we can possibly recover from this error.
                this.LogError($"{this.GetType().Name} quest has invalid state: {rawState}");
            }

            return result;
        }

        protected virtual bool TryParse(string rawState, out TQuestState result) => Enum.TryParse(rawState, out result);

        protected override void OnDayStartedQuestInProgress()
        {
            this.State = this.AdvanceStateForDayPassing(this.State);
        }

        protected abstract TQuestState AdvanceStateForDayPassing(TQuestState oldState);
    }
}
