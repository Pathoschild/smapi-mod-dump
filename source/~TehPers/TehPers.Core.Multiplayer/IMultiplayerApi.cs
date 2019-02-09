using System.IO;
using StardewValley;
using TehPers.Core.Multiplayer.Synchronized;

namespace TehPers.Core.Multiplayer {
    public interface IMultiplayerApi {
        /// <summary>Tries to register a message handler for a channel. Each channel can only have a single message handler.</summary>
        /// <param name="channel">The channel to register to.</param>
        /// <param name="handler">The message handler.</param>
        /// <returns>If the channel was registered to, returns a <see cref="MessageHandler"/> that can be used to unregister the handler. Otherwise, returns null.</returns>
        MessageHandler RegisterMessageHandler(string channel, MessageReader handler);

        /// <summary>Sends a message through a channel.</summary>
        /// <param name="channel">The channel to send the message through.</param>
        /// <param name="messageWriter">A function which writes the message.</param>
        void SendMessage(string channel, MessageWriter messageWriter);

        /// <summary>Synchronizes an object for all players. Whenever the object's value is changed, it will be updated for every player.</summary>
        /// <param name="id">The object's unique identifer.</param>
        /// <param name="target">The object to keep synchronized.</param>
        /// <returns>True if the object will be synchronized, false if there was an ID collision.</returns>
        bool Synchronize(string id, ISynchronized target);

        /// <summary>Stops synchronizing an object.</summary>
        /// <param name="id">The ID of the object being synchronized.</param>
        /// <returns>True if it won't be synchronized anymore, false if the ID was not found.</returns>
        bool Desynchronize(string id);
    }

    public delegate void MessageReader(Farmer sender, BinaryReader reader);

    public delegate void MessageWriter(BinaryWriter writer);
}