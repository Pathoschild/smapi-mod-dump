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
    internal interface IManagedNetEvent<TEventArgs> : IReceivableNetEvent, IManagedEvent<TEventArgs>
        where TEventArgs : EventArgs
    {
        /// <summary>Broadcast this event to all peers via multiplayer message.</summary>
        /// <param name="args">Event arguments.</param>
        /// <param name="sendToSelf">Raise event locally, since multiplayer messages are not sent back to the sender.</param>
        /// <exception cref="InvalidOperationException">Thrown when broadcasting before mod initialization.</exception>
        void Broadcast(TEventArgs args, bool sendToSelf = true);
    }
}
