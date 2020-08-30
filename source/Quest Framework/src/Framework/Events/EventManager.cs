using PurrplingCore.Events;
using QuestFramework.Events;
using StardewModdingAPI;
using System;

namespace QuestFramework.Framework.Events
{
    class EventManager
    {
        public ManagedEvent<ChangeStateEventArgs> ChangeState { get; }
        public ManagedEvent<GettingReadyEventArgs> GettingReady { get; }
        public ManagedEvent<ReadyEventArgs> Ready { get; }
        public ManagedEvent<QuestEventArgs> QuestCompleted { get; }
        public ManagedEvent<QuestEventArgs> QuestAccepted { get; }
        public ManagedEvent<QuestEventArgs> QuestRemoved { get; }
        public ManagedEvent<EventArgs> QuestLogMenuOpen { get; }
        public ManagedEvent<EventArgs> QuestLogMenuClosed { get; }
        public ManagedEvent<EventArgs> Refreshed { get; }

        public EventManager(IMonitor monitor)
        {
            ManagedEvent<TEventArgs> ManageEvent<TEventArgs>(string eventName)
            {
                var managedEvent = new ManagedEvent<TEventArgs>(eventName);

                if (monitor.IsVerbose)
                {
                    managedEvent.Fired += (_, e) => monitor.VerboseLog(
                        $"Event `{e.EventName}` fired. Listeners count: {e.ListenersCount}");
                }

                return managedEvent;
            }

            this.ChangeState = ManageEvent<ChangeStateEventArgs>(nameof(IQuestFrameworkEvents.ChangeState));
            this.GettingReady = ManageEvent<GettingReadyEventArgs>(nameof(IQuestFrameworkEvents.GettingReady));
            this.Ready = ManageEvent<ReadyEventArgs>(nameof(IQuestFrameworkEvents.Ready));
            this.QuestCompleted = ManageEvent<QuestEventArgs>(nameof(IQuestFrameworkEvents.QuestCompleted));
            this.QuestAccepted = ManageEvent<QuestEventArgs>(nameof(IQuestFrameworkEvents.QuestAccepted));
            this.QuestRemoved = ManageEvent<QuestEventArgs>(nameof(IQuestFrameworkEvents.QuestRemoved));
            this.QuestLogMenuOpen = ManageEvent<EventArgs>(nameof(IQuestFrameworkEvents.QuestLogMenuOpen));
            this.QuestLogMenuClosed = ManageEvent<EventArgs>(nameof(IQuestFrameworkEvents.QuestLogMenuClosed));
            this.Refreshed = ManageEvent<EventArgs>(nameof(IQuestFrameworkEvents.Refreshed));
        }
    }
}
