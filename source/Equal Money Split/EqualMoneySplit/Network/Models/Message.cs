using StardewValley;

namespace EqualMoneySplit.Networking.Models
{
    /// <summary>
    /// Context of Payload that is sent over Network
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Unique identifier for a given message
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Destination address the message will be sent to
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Payload to be delivered
        /// </summary>
        public object Payload { get; set; }

        /// <summary>
        /// Farmer sending the message
        /// </summary>
        public long Sender { get; set; }

        /// <summary>
        /// Destination listener message will be sent to
        /// </summary>
        public long Recipient { get; set; }

        /// <summary>
        /// Message that will be sent over Network
        /// </summary>
        public Message() { }

        /// <summary>
        /// Message that will be sent over Network
        /// </summary>
        /// <param name="address">Destination address the message will be sent to</param>
        /// <param name="payload">Payload to be delivered</param>
        /// <param name="recipient">ID of farmer to send message to</param>
        public Message(string address, object payload, long recipient = -1)
        {
            this.Address = address;
            this.Payload = payload;
            this.Sender = Game1.player.UniqueMultiplayerID;
            this.Recipient = recipient;
        }
    }
}
