using StardewValley;
using StardewValley.Network;

namespace FunnySnek.AntiCheat.Server.Framework.Patches
{
    /// <summary>Harmony patch for kick.</summary>
    internal class Server_SendMessage_Patcher : Patch
    {
        /*********
        ** Properties
        *********/
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(GameServer), "sendMessage", new System.Type[] { typeof(long), typeof(OutgoingMessage) });


        /*********
        ** Public methods
        *********/
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
