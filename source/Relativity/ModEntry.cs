/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/adroslice/sdv-relativity-mirror
**
*************************************************/

using System.Linq;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Relativity
{
    public class ModEntry : Mod
    {
        // GTI = GameTickInterval, STP = ShouldTimePass
        private static int LocalGTI => 7000 + (Game1.currentLocation?.getExtraMillisecondsPerInGameMinuteForThisLocation() ?? 0);
        private static bool LocalSTP => Game1.shouldTimePass(true);
        private readonly Dictionary<long, (bool STP, int GTI)> playerTimeData = new();

        public override void Entry(IModHelper helper)
        {
            Helper.Events.Multiplayer.ModMessageReceived += Multiplayer_ModMessageReceived;
            Helper.Events.Multiplayer.PeerDisconnected += Multiplayer_PeerDisconnected;
            Helper.Events.Multiplayer.PeerConnected += Multiplayer_PeerConnected;
            Helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
            Helper.Events.Player.Warped += Player_Warped;
        }

        private void Player_Warped(object sender, WarpedEventArgs e) =>
            UpdatePeerData(e.Player.UniqueMultiplayerID, (LocalSTP, LocalGTI));
        private void Multiplayer_PeerConnected(object sender, PeerConnectedEventArgs e) =>
            UpdatePeerData(e.Peer.PlayerID, (LocalSTP, LocalGTI));
        private void Multiplayer_PeerDisconnected(object sender, PeerDisconnectedEventArgs e) =>
            UpdatePeerData(e.Peer.PlayerID, null);

        private void Multiplayer_ModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (!Context.IsMainPlayer || e.FromModID != ModManifest.UniqueID) return;

            if (e.Type == "TimeData") UpdatePeerData(e.FromPlayerID, e.ReadAs<(bool, int)>());
        }

        private void UpdatePeerData(long playerID, (bool, int)? data)
        {
            if (!Context.IsMainPlayer)
            {
                Helper.Multiplayer.SendMessage(data, "TimeData", new[] { ModManifest.UniqueID }); ;
                return;
            }

            if (data == null) playerTimeData.Remove(playerID);
            else playerTimeData[playerID] = ((bool, int))data;
        }

        private bool prevSTP = false;
        private float totalElapsedGTI = 0;
        private void GameLoop_UpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsMultiplayer) return;

            if (LocalSTP != prevSTP)
                UpdatePeerData(Game1.player.UniqueMultiplayerID, (prevSTP = LocalSTP, LocalGTI));

            if (!Context.IsMainPlayer) return;

            if (float.IsNaN(totalElapsedGTI)) totalElapsedGTI = 0;

            float players = playerTimeData.Count;
            float active = playerTimeData.Count(x => x.Value.STP);

            totalElapsedGTI += Game1.gameTimeInterval * (active / players);
            Game1.gameTimeInterval = 0;

            float averageGTI = playerTimeData.Sum(x => x.Value.GTI) / players;
            if (totalElapsedGTI >= averageGTI)
                totalElapsedGTI -= Game1.gameTimeInterval = (int) averageGTI;
        }
    }
}
