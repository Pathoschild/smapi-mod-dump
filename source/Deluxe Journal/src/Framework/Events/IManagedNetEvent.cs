/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewModdingAPI.Events;

namespace DeluxeJournal.Framework.Events
{
    internal interface IManagedNetEvent : IManagedEvent
    {
        /// <summary>Raise this event from a broadcast message.</summary>
        /// <param name="invoker">Object that raised this event.</param>
        /// <param name="args"><see cref="IMultiplayerEvents.ModMessageReceived"/> event args.</param>
        void RaiseFromMessage(object? invoker, ModMessageReceivedEventArgs args);
    }
}
