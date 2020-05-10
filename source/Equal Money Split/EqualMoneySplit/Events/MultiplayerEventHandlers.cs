using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EqualMoneySplit.Events
{
    public class MultiplayerEventHandlers
    {
        public void OnPeerContextReceived(object sender, PeerContextReceivedEventArgs args)
        {
            Task requestTask = Task.Run(() =>
            {
                bool hasPlayerConnected = Game1.getOnlineFarmers().Any(f => f.UniqueMultiplayerID == args.Peer.PlayerID); ;
                int intervalsWaited = 0;
                while (!hasPlayerConnected && intervalsWaited < 300)
                {
                    hasPlayerConnected = Game1.getOnlineFarmers().Any(f => f.UniqueMultiplayerID == args.Peer.PlayerID);

                    if (hasPlayerConnected)
                        break;

                    Thread.Sleep(100);
                    intervalsWaited++;
                }

                if (hasPlayerConnected)
                    CheckForValidModInstall(args.Peer, Game1.getFarmer(args.Peer.PlayerID).Name);
                else
                    CheckForValidModInstall(args.Peer, args.Peer.PlayerID.ToString());
            });
        }

        /// <summary>
        /// Displays an error message if a user does not have EqualMoneySplit installed properly
        /// </summary>
        /// <param name="newPlayerData"></param>
        /// <param name="newPlayerName"></param>
        private void CheckForValidModInstall(IMultiplayerPeer newPlayerData, string newPlayerName)
        {
            var currentPlayerMod = EqualMoneyMod.SMAPI.ModRegistry.Get(Models.Constants.ModId);
            var newPlayerModData = newPlayerData.GetMod(Models.Constants.ModId);
            string errorMessage = "";

            if (!newPlayerData.HasSmapi)
                errorMessage = $"Player {newPlayerName} does not have SMAPI installed! EqualMoneySplit will not function properly!";
            else if (newPlayerData.Mods.Count() == 0)
                errorMessage = $"Player {newPlayerName} does not have any mods installed! EqualMoneySplit will not function properly!";
            else if (newPlayerModData == null)
                errorMessage = $"Player {newPlayerName} does not have EqualMoneySplit installed! EqualMoneySplit will not function properly!";
            else if (newPlayerModData.Version.IsOlderThan(currentPlayerMod.Manifest.Version))
                errorMessage = $"Player {newPlayerName} has an older version ({newPlayerModData.Version.ToString()}) EqualMoneySplit than yours ({currentPlayerMod.Manifest.Version.ToString()})! EqualMoneySplit will not function properly!";
            else if (newPlayerModData.Version.IsNewerThan(currentPlayerMod.Manifest.Version))
                errorMessage = $"Player {newPlayerName} has a newer version ({newPlayerModData.Version.ToString()}) of EqualMoneySplit than yours ({currentPlayerMod.Manifest.Version.ToString()})! EqualMoneySplit will not function properly!";

            if (!string.IsNullOrEmpty(errorMessage))
                Game1.chatBox.addErrorMessage(errorMessage);
        }
    }
}
