/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

namespace DeluxeJournal.Framework.Events
{
    internal interface IManagedEvent<TEventArgs> where TEventArgs : EventArgs
    {
        /// <summary>Unique human-readable name for the event.</summary>
        string EventName { get; }

        /// <summary>Whether this event has any subscribed event handlers.</summary>
        bool HasEventListeners { get; }

        /// <summary>Add an event handler.</summary>
        void Add(EventHandler<TEventArgs> handler);

        /// <summary>Remove an event handler.</summary>
        void Remove(EventHandler<TEventArgs> handler);

        /// <summary>Raise this event.</summary>
        /// <param name="invoker">Object that raised this event.</param>
        /// <param name="args">Event args.</param>
        void Raise(object? invoker, TEventArgs args);
    }
}
