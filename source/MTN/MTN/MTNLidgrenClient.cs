using StardewValley.Network;

namespace MTN
{
    public class MTNLidgrenClient : LidgrenClient
    {
        public ulong uploadByteCount = 0;

        public MTNLidgrenClient(string address) : base(address) { }

        public override void sendMessage(OutgoingMessage message) {
            base.sendMessage(message);
            if (message.Data != null)
            {
                foreach (var packet in message.Data)
                    uploadByteCount += (ulong)packet.ToString().Length;
            }
        }

        public ulong readUploadAmount() {
            ulong results = uploadByteCount;
            uploadByteCount = 0;
            return results;
        }
    }
}
