/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

namespace SAML.Utilities
{
    /// <summary>
    /// A helper class to handle event listeners
    /// </summary>
    public class EventManager
    {
        private readonly Dictionary<EventIds, Dictionary<IElement, Delegate>> events = [];

        /// <summary>
        /// Add a handler to the element for a specified event
        /// </summary>
        /// <param name="owner">The target element for the event</param>
        /// <param name="eventId">The id of the event to attach a handler to</param>
        /// <param name="handler">The handler to attach to the element</param>
        /// <exception cref="ArgumentNullException">Throws an exception if the target is null</exception>
        public void AddListener(IElement owner, EventIds eventId, Delegate? handler)
        {
            if (owner is null)
                throw new ArgumentNullException(nameof(owner));
            if (!events.ContainsKey(eventId))
                events.Add(eventId, []);
            if (!events[eventId].ContainsKey(owner))
                events[eventId].Add(owner, handler);
            else
                Delegate.Combine(events[eventId][owner], handler);
        }

        /// <summary>
        /// Remove a handler to the element for a specified event
        /// </summary>
        /// <param name="owner">The target element for the event</param>
        /// <param name="eventId">The id of the event to remove the handler from</param>
        /// <param name="handler">The handler to remove from the element</param>
        /// <exception cref="ArgumentNullException">Throws an exception if the target is null</exception>
        public void RemoveListener(IElement owner, EventIds eventId, Delegate? handler)
        {
            if (owner is null)
                throw new ArgumentNullException(nameof(owner));
            if (!events.ContainsKey(eventId))
                return;
            Delegate.Remove(events[eventId][owner], handler);
        }

        /// <summary>
        /// Fire an event with given arguments on all specified targets (or all available if null)
        /// </summary>
        /// <param name="eventId">The id of the event to fire</param>
        /// <param name="args">The arguments to pass along with the event</param>
        /// <param name="sender">(optional) The object which invoked the event, default is the target</param>
        /// <param name="targets">(optional) The elements on which to invoke the event, default is all available</param>
        public void Fire<T>(EventIds eventId, T args, object? sender = null, IEnumerable<IElement?>? targets = null) where T : EventArgs
        {
            if (!events.ContainsKey(eventId))
                return;
            foreach (var item in filterByTarget(events[eventId], targets))
            {
                var handlers = item.Value.GetInvocationList();
                for (int i = 0; i < handlers.Length; i++)
                    handlers[i]?.Method.Invoke(handlers[i].Target, new object[] { sender ?? item.Key, args });
            }
        }

        /// <summary>
        /// Clear all events for a specified element
        /// </summary>
        /// <param name="owner">The element from which to remove the events</param>
        public void Clear(IElement owner)
        {
            foreach (var item in events.Keys)
            {
                if (events[item].ContainsKey(owner))
                    events[item].Remove(owner);
            }
        }

        /// <summary>
        /// Clear all events for a collection of elements
        /// </summary>
        /// <param name="owners">The elements from which to remove the events</param>
        public void Clear(IEnumerable<IElement> owners)
        {
            foreach (var item in owners)
                Clear(item);
        }

        /// <summary>
        /// [Internal] Clears all events for menu close
        /// </summary>
        internal void Clear() => events.Clear();

        private Dictionary<IElement, Delegate> filterByTarget(Dictionary<IElement, Delegate> items, IEnumerable<IElement?>? targets)
        {
            if (targets is null)
                return items;
            Dictionary<IElement, Delegate> newItems = [];
            foreach (var item in targets)
            {
                if (item is null)
                    continue;
                if (items.ContainsKey(item))
                    newItems.Add(item, items[item]);
            }
            return newItems;
        }
    }
}
