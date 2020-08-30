using QuestFramework.Events;
using QuestFramework.Extensions;
using QuestFramework.Framework.Events;
using QuestFramework.Framework.Stats;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework.Watchdogs
{
    internal class QuestLogWatchdog
    {
        private EventManager EventManager { get; }
        private StatsManager StatsManager { get; }
        private IMonitor Monitor { get; }
        private List<Quest> LastQuestLog { get; set; }

        public QuestLogWatchdog(IModEvents modEvents, EventManager eventManager, StatsManager statsManager, IMonitor monitor)
        {
            this.EventManager = eventManager;
            this.StatsManager = statsManager;
            this.Monitor = monitor;

            // QuestLog reference only for andoroid, 
            // other platform uses ElementChanged event on NetObjectList
            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                modEvents.GameLoop.UpdateTicked += this.OnUpdateTicked;
                modEvents.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
            }
        }

        public void Initialize()
        {
            if (!Context.IsWorldReady)
                return;

            if (Constants.TargetPlatform == GamePlatform.Android)
                this.LastQuestLog = new List<Quest>(Game1.player.questLog);
            else
                this.RegisterNetListElementChangedEvent();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("SMAPI.CommonErrors", "AvoidImplicitNetFieldCast", Justification = "Accessing to instance via reference for add event handler with reflection")]
        private void RegisterNetListElementChangedEvent()
        {
            // Used reflection because Android SMAPI issues with broken code, called only on desktop systems
            // This code practilaly isn't broken, because it's not called on android SMAPI (see Initialize method)
            // This code effectively do this: Game1.player.questLog.OnElementChanged += this.OnQuestLogChanged;

            var eventInfo = Game1.player.questLog.GetType().GetEvent("OnElementChanged");
            var listenerInfo = this.GetType().GetMethod(nameof(this.OnQuestLogChanged), BindingFlags.NonPublic | BindingFlags.Instance);
            var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, listenerInfo);

            eventInfo.AddEventHandler(Game1.player.questLog, handler);
        }

        private void OnQuestLogChanged(Netcode.NetList<StardewValley.Quests.Quest, Netcode.NetRef<StardewValley.Quests.Quest>> list, int index, StardewValley.Quests.Quest oldValue, StardewValley.Quests.Quest newValue)
        {
            this.HandleChange(oldValue, newValue);
        }

        private void HandleChange(Quest oldValue, Quest newValue)
        {
            if (oldValue == null && newValue != null && newValue.IsManaged())
            {
                var managedQuest = newValue.AsManagedQuest();

                this.StatsManager.AddAcceptedQuest(managedQuest.GetFullName());
                this.Monitor.Log($"Managed quest `{managedQuest.GetFullName()}` added to player's quest log");
            }

            if (newValue == null && oldValue != null && oldValue.IsManaged())
            {
                var managedQuest = oldValue.AsManagedQuest();

                this.StatsManager.AddRemovedQuest(managedQuest.GetFullName());
                this.Monitor.Log($"Managed quest `{managedQuest.GetFullName()}` removed from player's quest log");
            }

            if (oldValue == null && newValue != null)
                this.EventManager.QuestAccepted.Fire(new QuestEventArgs(newValue), this);

            if (newValue == null && oldValue != null)
            {
                this.EventManager.QuestRemoved.Fire(new QuestEventArgs(oldValue), this);

                if (oldValue.IsManaged())
                    oldValue.AsManagedQuest().Reset();
            }
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // This observe the android questlog watching for trigs add/remove quest from questlog events
            if (!Context.IsWorldReady || this.LastQuestLog == null)
                return;

            if (this.IsQuestLogChanged(Game1.player.questLog))
            {
                // detect added/removed quests
                var currentQuests = new List<Quest>(Game1.player.questLog);
                var removedQuests = this.LastQuestLog.Except(currentQuests);
                var addedQuests = currentQuests.Except(this.LastQuestLog);

                // update tracking
                this.LastQuestLog = currentQuests;

                // Fire QuestAccepted event
                foreach (var addedQuest in addedQuests)
                    this.HandleChange(null, addedQuest);

                // Fire QuestRemoved event
                foreach (var removedQuest in removedQuests)
                    this.HandleChange(removedQuest, null);
            }
        }

        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            this.LastQuestLog = null;
        }

        private bool IsQuestLogChanged(IList<Quest> questLog)
        {
            if (this.LastQuestLog.Count != questLog.Count)
                return true;

            if (this.LastQuestLog[this.LastQuestLog.Count - 1] != questLog[questLog.Count - 1])
                return true;

            return false;
        }
    }
}
