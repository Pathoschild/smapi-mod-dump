using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Dem1se.CustomReminders.Multiplayer
{

    static class Multiplayer
    {
        /// <summary>
        /// Used by the peers to recieve and set the value for Utilities.Utilities.SaveFolderName field
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Contains all the data/args of the event</param>
        public static void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == Utilities.Data.Helper.ModRegistry.ModID && e.Type == "SaveFolderName")
            {
                SaveFolderNameModel message = e.ReadAs<SaveFolderNameModel>();
                Utilities.Data.SaveFolderName = message.SaveFolderName;
            }
        }

        /// <summary>
        /// Send message method.
        /// The method to call when a peer connects. Host will send the SaveFolderName using this.
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Contains all the arguments of PeerConnectedEvent</param>
        public static void OnPeerConnected(object sender, PeerContextReceivedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                Utilities.Data.Helper.Multiplayer.SendMessage<SaveFolderNameModel>(new SaveFolderNameModel() { SaveFolderName = Utilities.Data.SaveFolderName }, "SaveFolderName", modIDs: new[] { Utilities.Data.Helper.ModRegistry.ModID }, playerIDs: new[] { e.Peer.PlayerID });
            }
        }
    }

    /// <summary>Data model for sending and receiving data in Multiplayer messages.</summary>
    class SaveFolderNameModel
    {
        public string SaveFolderName { get; set; }
    }
}
