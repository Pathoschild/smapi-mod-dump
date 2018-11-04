using StardewValley;

namespace FunnySnek.AntiCheat.Server.Framework.Patches
{
    internal class Multiplayer_ParseServerToClientsMessage_Patcher : Patch  //this totally works!
    {
        /*********
        ** Properties
        *********/
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Multiplayer), "parseServerToClientsMessage");


        /*********
        ** Public methods
        *********/
        public static void Prefix(string message)
        {
            // track new messages
            ModEntry.MessagesReceived.Add(message);
        }
    }
}
