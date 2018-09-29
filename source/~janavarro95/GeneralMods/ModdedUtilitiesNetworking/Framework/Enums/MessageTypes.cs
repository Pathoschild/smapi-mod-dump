using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModdedUtilitiesNetworking.Framework.Enums
{
    public class MessageTypes
    {
        public enum messageTypes
        {
            SendOneWay,
            SendToAll,
            SendToSpecific
        }


        /// <summary>
        /// Sends a one way message. If sent from server it sends to all clients.
        /// If sent from a client it sends only to the server.
        /// </summary>
        public const int SendOneWay = 20;

        /// <summary>
        /// Sends the message to all clients and the server.
        /// </summary>
        public const int SendToAll = 21;

        /// <summary>
        /// Unused.
        /// </summary>
        public const int SendToSpecific = 22;

    }
}
