using StardewValley;
using StardewValley.Network;

namespace BattleRoyale
{
    class Server_sendMessage_Patcher : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(GameServer), "sendMessage", new System.Type[] { typeof(long), typeof(OutgoingMessage)});

        public static bool Prefix(long peerId)
        {
            if (Game1.IsServer && (!Game1.otherFarmers.ContainsKey(peerId)))
            {
                //They have been kicked off the server
                return false;
            }

            return true;
        }
    }
}
