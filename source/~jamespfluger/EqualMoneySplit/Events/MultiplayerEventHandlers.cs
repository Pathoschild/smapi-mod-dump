/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-EqualMoneySplit
**
*************************************************/

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
        public async void OnPeerConnected(object sender, PeerConnectedEventArgs args)
        {
            EventSubscriber.Instance.TrySetEventSubscriptions();

            Task requestTask = Task.Run(async () =>
            {
                bool hasPlayerConnected = Game1.getOnlineFarmers().Any(f => f.UniqueMultiplayerID == args.Peer.PlayerID); ;
                int intervalsWaited = 0;
                while (!hasPlayerConnected && intervalsWaited < 300)
                {
                    hasPlayerConnected = Game1.getOnlineFarmers().Any(f => f.UniqueMultiplayerID == args.Peer.PlayerID);

                    if (hasPlayerConnected)
                        break;

                    await Task.Delay(100);
                    intervalsWaited++;
                }

                if (hasPlayerConnected)
                    CheckForValidModInstall(args.Peer, Game1.getFarmer(args.Peer.PlayerID).Name);
                else
                    CheckForValidModInstall(args.Peer, args.Peer.PlayerID.ToString());
            });

            await requestTask;

            EqualMoneyMod.FarmerData.Value ??= new();
        }
        public void OnPeerDisconnected(object sender, PeerDisconnectedEventArgs args)
        {
            EventSubscriber.Instance.TrySetEventSubscriptions();
        }

        /// <summary>
        /// Displays an error message if a user does not have EqualMoneySplit installed properly
        /// </summary>
        /// <param name="newPlayerData"></param>
        /// <param name="newPlayerName"></param>
        private void CheckForValidModInstall(IMultiplayerPeer newPlayerData, string newPlayerName)
        {
            IModInfo currentPlayerMod = EqualMoneyMod.SMAPI.ModRegistry.Get(Models.Constants.ModId);
            IMultiplayerPeerMod newPlayerModData = newPlayerData.GetMod(Models.Constants.ModId);
            string errorMessage = "";

            if (!newPlayerData.HasSmapi)
                errorMessage = $"Player {newPlayerName} does not have SMAPI installed! EqualMoneySplit will not function properly!";
            else if (newPlayerData.Mods.Count() == 0)
                errorMessage = $"Player {newPlayerName} does not have any mods installed! EqualMoneySplit will not function properly!";
            else if (newPlayerModData == null)
                errorMessage = $"Player {newPlayerName} does not have EqualMoneySplit installed! EqualMoneySplit will not function properly!";
            else if (newPlayerModData.Version.IsOlderThan(currentPlayerMod.Manifest.Version))
                errorMessage = $"Player {newPlayerName} has an older version ({newPlayerModData.Version}) EqualMoneySplit than yours ({currentPlayerMod.Manifest.Version})! EqualMoneySplit will not function properly!";
            else if (newPlayerModData.Version.IsNewerThan(currentPlayerMod.Manifest.Version))
                errorMessage = $"Player {newPlayerName} has a newer version ({newPlayerModData.Version}) of EqualMoneySplit than yours ({currentPlayerMod.Manifest.Version})! EqualMoneySplit will not function properly!";

            if (!string.IsNullOrEmpty(errorMessage))
                Game1.chatBox.addErrorMessage(errorMessage);
        }
    }
}
