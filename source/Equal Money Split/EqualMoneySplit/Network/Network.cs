using EqualMoneySplit.Networking.Models;
using StardewModdingAPI.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace EqualMoneySplit.Networking
{
    /// <summary>
    /// Utility that sends or receives messages over the Network
    /// </summary>
    public class Network
    {
        /// <summary>
        /// Messages that have been received
        /// </summary>
        public ConcurrentDictionary<string, List<Message>> ReceivedMessages { get; set; }

        /// <summary>
        /// Global instance of Network
        /// </summary>
        public static Network Instance { get { return network.Value; } }

        /// <summary>
        /// Local instance of the Network class loaded lazily
        /// </summary>
        private static readonly Lazy<Network> network = new Lazy<Network>(() => new Network());

        /// <summary>
        /// Utility for handling Network messages
        /// </summary>
        public Network()
        {
            ReceivedMessages = new ConcurrentDictionary<string, List<Message>>();
        }

        /// <summary>
        /// Handles the event raised after a mod message is received
        /// </summary>
        /// <param name="sender">The sender of the ModMessageReceived event</param>
        /// <param name="args">Event arguments for the ModMessageReceived event</param>
        public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs args)
        {
            Message message = args.ReadAs<Message>();

            ReceivedMessages.TryAdd(args.Type, new List<Message>());
            ReceivedMessages[args.Type].Add(message);
        }

        /// <summary>
        /// Checks for new messages from a particular farmer that haven't been handled yet
        /// </summary>
        /// <param name="address">Destination address to check for message</param>
        /// <param name="sender">ID of Farmer that sent a message</param>
        /// <returns></returns>
        public IEnumerable<Message> RetrieveMessages(string address, long sender = -1)
        {
            ReceivedMessages.TryAdd(address, new List<Message>());

            List<Message> messages = new List<Message>(ReceivedMessages[address]);

            foreach (Message message in messages)
            {
                if (sender == -1 || sender == message.Sender)
                {
                    Network.Instance.ReceivedMessages[address].Remove(message);
                    yield return message;
                }
            }
        }
    }
}
