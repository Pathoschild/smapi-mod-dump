/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace DeluxeJournal.Framework.Events
{
    /// <summary>ManagedNetEvent concrete class for a simple JSON serializable <see cref="TEventArgs"/>.</summary>
    internal class ManagedNetEvent<TEventArgs> : ManagedNetEvent<TEventArgs, TEventArgs> where TEventArgs : EventArgs
    {
        public ManagedNetEvent(string name, IMultiplayerHelper multiplayer)
            : base(name, multiplayer)
        {
        }

        protected override TEventArgs EventArgsToMessage(TEventArgs args)
        {
            return args;
        }

        protected override TEventArgs MessageToEventArgs(TEventArgs message)
        {
            return message;
        }
    }

    internal abstract class ManagedNetEvent<TEventArgs, TMessage> : ManagedEvent<TEventArgs>, IManagedNetEvent<TEventArgs>
        where TEventArgs : EventArgs
        where TMessage : notnull
    {
        private IMultiplayerHelper Multiplayer { get; }

        public ManagedNetEvent(string name, IMultiplayerHelper multiplayer) : base(name)
        {
            Multiplayer = multiplayer;
        }

        /// <summary>Construct a <see cref="TMessage"/> from args.</summary>
        protected abstract TMessage EventArgsToMessage(TEventArgs args);

        /// <summary>Construct <see cref="TEventArgs"/> from a broadcasted message.</summary>
        protected abstract TEventArgs MessageToEventArgs(TMessage message);

        public void RaiseFromMessage(object? invoker, ModMessageReceivedEventArgs args)
        {
            if (args.Type == EventName && args.ReadAs<TMessage>() is TMessage message)
            {
                Raise(invoker, MessageToEventArgs(message));
            }
        }

        public void Broadcast(TEventArgs args, bool sendToSelf = true)
        {
            if (DeluxeJournalMod.Instance is not DeluxeJournalMod mod)
            {
                throw new InvalidOperationException("Attempted to broadcast event before mod entry.");
            }

            Multiplayer.SendMessage(
                EventArgsToMessage(args),
                EventName,
                modIDs: new[] { mod.ModManifest.UniqueID }
            );

            // Messages are not sent back to the mod that sent them, so raise this event locally
            if (sendToSelf)
            {
                Raise(null, args);
            }
        }
    }
}
