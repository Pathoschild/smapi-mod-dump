using Galaxy.Api;
using StardewValley.Network;
using StardewValley.SDKs;

namespace MTN {
    public class MTNGalaxyNetClient : GalaxyNetClient {

        protected ulong uploadByteCount = 0;

        public MTNGalaxyNetClient(GalaxyID lobbyId) : base(lobbyId) { }

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
