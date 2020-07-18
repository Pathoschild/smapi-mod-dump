using PurrplingCore.Events;
using QuestFramework.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework.Events
{
    class EventManager
    {
        public ManagedEvent<ChangeStateEventArgs> ChangeState { get; }
        public ManagedEvent<GettingReadyEventArgs> GettingReady { get; }
        public ManagedEvent<ReadyEventArgs> Ready { get; }

        public EventManager()
        {
            ManagedEvent<TEventArgs> ManageEvent<TEventArgs>(string eventName)
            {
                return new ManagedEvent<TEventArgs>(eventName);
            }

            this.ChangeState = ManageEvent<ChangeStateEventArgs>(nameof(IQuestFrameworkEvents.ChangeState));
            this.GettingReady = ManageEvent<GettingReadyEventArgs>(nameof(IQuestFrameworkEvents.GettingReady));
            this.Ready = ManageEvent<ReadyEventArgs>(nameof(IQuestFrameworkEvents.Ready));
        }
    }
}
