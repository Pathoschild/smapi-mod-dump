/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

namespace Pong.Framework.Messages
{
    internal class MessageEnvelope
    {
        public string MessageType { get; }
        public long RecipientId { get; }
        public object Data { get; }

        public MessageEnvelope(object data, long recipientId)
        {
            this.Data = data;
            this.MessageType = data.GetType().Name;
            this.RecipientId = recipientId;
        }
    }
}
