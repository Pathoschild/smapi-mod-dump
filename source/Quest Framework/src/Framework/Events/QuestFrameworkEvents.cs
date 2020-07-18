using QuestFramework.Events;
using System;

namespace QuestFramework.Framework.Events
{
    class QuestFrameworkEvents : IQuestFrameworkEvents
    {
        private EventManager eventManager;

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
