using EqualMoneySplit.Networking.Models;
using StardewValley;

namespace EqualMoneySplit.Networking.Communicators
{
    /// <summary>
    /// Handles outgoing messages to Farmers and the corresponding responses 
    /// </summary>
    public abstract class Messenger
    {
        /// <summary>
        /// Sends a message to a given Farmer, defaulting to all farmers
        /// </summary>
        /// <param name="address">Destination address to check for message</param>
        /// <param name="payload">Payload data to be delivered to the Farmer</param>
        /// <param name="recipient">The farmer who will receive the message</param>
        public virtual void SendMessageToFarmer(string address, object payload, long recipient)
        {
            Message message = new Message(address, payload, recipient);

            SendCoreMessageToFarmer(message);
        }

        /// <summary>
        /// Sends a message to all Farmers
        /// </summary>
        /// <param name="address">Destination address to send the payload</param>
        /// <param name="payload">Payload data to be delivered to the Farmer</param>
        public virtual void SendMessageToAllFarmers(string address, object payload)
        {
            Message message = new Message(address, payload, -1);

            SendCoreMessageToFarmer(message);
        }

        /// <summary>
        /// Sends a Message with an object payload to a given farmer
        /// </summary>
        /// <param name="message">The message to be delivered to the farmer</param>
        private void SendCoreMessageToFarmer(Message message)
        {
            EqualMoneyMod.Logger.Log($"Local farmer {Game1.player.Name} is sending a message to {message.Address} for {Game1.getFarmer(message.Recipient)}");
            EqualMoneyMod.SMAPI.Multiplayer.SendMessage(message, message.Address, new[] { EqualMoneyMod.SMAPI.Multiplayer.ModID }, message.Recipient != -1 ? new[] { message.Recipient } : null);
        }
    }
}
