/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dem1se/SDVMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Dem1se.CustomReminders.Multiplayer
{

    static class Multiplayer
    {
        /* The below two methods are bound in the entry method to their respective events.*/
        
        
        /// <summary>
        /// Used by the peers to recieve and set the value for Utilities.Utilities.SaveFolderName field
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Contains all the data/args of the event</param>
        public static void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == Utilities.Globals.Helper.ModRegistry.ModID && e.Type == "SaveFolderName")
            {
                SaveFolderNameModel message = e.ReadAs<SaveFolderNameModel>();
                Utilities.Globals.SaveFolderName = message.SaveFolderName;
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
                Utilities.Globals.Helper.Multiplayer.SendMessage<SaveFolderNameModel>(new SaveFolderNameModel() { SaveFolderName = Utilities.Globals.SaveFolderName }, "SaveFolderName", modIDs: new[] { Utilities.Globals.Helper.ModRegistry.ModID }, playerIDs: new[] { e.Peer.PlayerID });
            }
        }
    }

    /// <summary>Data model for sending and receiving data in Multiplayer messages.</summary>
    class SaveFolderNameModel
    {
        public string SaveFolderName { get; set; }
    }
}
