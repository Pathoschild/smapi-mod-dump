using System;
using Bookcase.Lib;

namespace Bookcase.Events {

    /// <summary>
    /// An enum containing the various event priorities. 
    /// 
    /// Higher priority events happen first. Lower priority happen last.
    /// </summary>
    public enum Priority { Highest, Hight, Normal, Low, Lowest };

    /// <summary>
    /// The event bus used by Bookcase. Allows for fairly efficient events with priority and event cancelling. 
    /// </summary>
    /// <typeparam name="T">The event type</typeparam>
    public class EventBus<T> where T : Event {

        /// <summary>
        /// A delegate for the type of method that is a valid listener.
        /// </summary>
        /// <param name="args">Arg type must match the event type.</param>
        public delegate void EventListener(T args);

        /// <summary>
        /// Internal collection of all subscribed listeners.
        /// </summary>
        private MultiDict<Priority, EventListener> listeners = new MultiDict<Priority, EventListener>();

        /// <summary>
        /// Adds an event listener to this event bus.
        /// </summary>
        /// <param name="listener">The listener to add.</param>
        /// <param name="priority">Optional parameter to set the priority of the listener.</param>
        public void Add(EventListener listener, Priority priority = Priority.Normal) {

            this.listeners.Add(priority, listener);
        }

        /// <summary>
        /// Removes a listener from the bus.
        /// </summary>
        /// <param name="listener">The listener to remove.</param>
        public void Remove(EventListener listener, Priority priority = Priority.Normal) {

            this.listeners.Remove(listener, priority);
        }

        /// <summary>
        /// Sends an event to all of the listeners.
        /// </summary>
        /// <param name="args">The event object to post.</param>
        /// <returns>Whether or not the event was canceled.</returns>
        public bool Post(T args) {

            foreach (Priority priority in Enum.GetValues(typeof(Priority))) {

                foreach (var listener in this.listeners.Get(priority)) {

                    listener(args);

                    if (args.IsCanceled()) {

                        break;
                    }
                }
            }
            return args.IsCanceled();
        }
    }
}
