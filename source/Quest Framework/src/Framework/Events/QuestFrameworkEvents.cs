using QuestFramework.Events;
using System;

namespace QuestFramework.Framework.Events
{
    class QuestFrameworkEvents : IQuestFrameworkEvents
    {
        private readonly EventManager eventManager;

        public event EventHandler<EventArgs> Refreshed
        {
            add => this.eventManager.Refreshed.Add(value);
            remove => this.eventManager.Refreshed.Remove(value);
        }

        public event EventHandler<QuestEventArgs> QuestCompleted
        {
            add => this.eventManager.QuestCompleted.Add(value);
            remove => this.eventManager.QuestCompleted.Remove(value);
        }
        public event EventHandler<QuestEventArgs> QuestAccepted
        {
            add => this.eventManager.QuestAccepted.Add(value);
            remove => this.eventManager.QuestAccepted.Remove(value);
        }
        public event EventHandler<QuestEventArgs> QuestRemoved
        {
            add => this.eventManager.QuestRemoved.Add(value);
            remove => this.eventManager.QuestRemoved.Remove(value);
        }
        public event EventHandler<EventArgs> QuestLogMenuOpen
        {
            add => this.eventManager.QuestLogMenuOpen.Add(value);
            remove => this.eventManager.QuestLogMenuOpen.Remove(value);
        }
        public event EventHandler<EventArgs> QuestLogMenuClosed
        {
            add => this.eventManager.QuestLogMenuClosed.Add(value);
            remove => this.eventManager.QuestLogMenuClosed.Remove(value);
        }

        public event EventHandler<ChangeStateEventArgs> ChangeState
        {
            add => this.eventManager.ChangeState.Add(value);
            remove => this.eventManager.ChangeState.Remove(value);
        }

        public event EventHandler<GettingReadyEventArgs> GettingReady
        {
            add => this.eventManager.GettingReady.Add(value);
            remove => this.eventManager.GettingReady.Remove(value);
        }

        public event EventHandler<ReadyEventArgs> Ready
        {
            add => this.eventManager.Ready.Add(value);
            remove => this.eventManager.Ready.Remove(value);
        }

        public QuestFrameworkEvents(EventManager eventManager)
        {
            this.eventManager = eventManager;
        }
    }
}
