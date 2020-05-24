using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Events
{
    internal class ManagedEvent<TEventArgs> : IManagedEvent
    {
        /// <summary>The underlying event.</summary>
        private event EventHandler<TEventArgs> Event;

        /// <summary>A human-readable name for the event.</summary>
        public string EventName { get; }


        /// <summary>Construct an instance.</summary>
        /// <param name="eventName">A human-readable name for the event.</param>
        public ManagedEvent(string eventName)
        {
            this.EventName = eventName;
        }

        /// <summary>Get whether anything is listening to the event.</summary>
        public bool HasListeners()
        {
            return this.Event != null && this.Event.GetInvocationList().Length > 0;
        }

        /// <summary>Add an event handler.</summary>
        /// <param name="handler">The event handler.</param>
        /// <param name="mod">The mod which added the event handler.</param>
        public void Add(EventHandler<TEventArgs> handler)
        {
            this.Event += handler;
        }

        /// <summary>Remove an event handler.</summary>
        /// <param name="handler">The event handler.</param>
        public void Remove(EventHandler<TEventArgs> handler)
        {
            this.Event -= handler;
        }

        /// <summary>Raise the event and notify all handlers.</summary>
        /// <param name="args">The event arguments to pass.</param>
        public void Fire(TEventArgs args, object invoker)
        {
            this.Event?.Invoke(invoker, args);
        }
    }
}
