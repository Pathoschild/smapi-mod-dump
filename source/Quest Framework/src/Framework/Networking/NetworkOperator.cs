/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using QuestFramework.Framework.Controllers;
using QuestFramework.Framework.Networking;
using QuestFramework.Framework.Stats;
using QuestFramework.Framework.Store;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Linq;

namespace QuestFramework.Framework.Networing
{
    class NetworkOperator
    {
        private readonly IMultiplayerHelper helper;
        private readonly QuestStateStore questStateStore;
        private readonly QuestController questController;
        private readonly StatsManager statsManager;
        private readonly IManifest modManifest;
        private readonly IMonitor monitor;

        private bool hasInitReceived = false;

        public event EventHandler InitReceived;

        public NetworkOperator(IMultiplayerHelper helper, IMultiplayerEvents events, QuestStateStore questStateStore, StatsManager statsManager, QuestController questController, IManifest modManifest, IMonitor monitor)
        {
            this.helper = helper;
            this.questStateStore = questStateStore;
            this.questController = questController;
            this.statsManager = statsManager;
            this.modManifest = modManifest;
            this.monitor = monitor;

            events.ModMessageReceived += this.OnMessageReceived;
            events.PeerConnected += this.OnClientConnected;
        }

        [EventPriority(EventPriority.High + 100)]
        private void OnClientConnected(object sender, PeerConnectedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            if (e.Peer.HasSmapi)
            {
                this.monitor.Log($"Peer connected: {e.Peer.PlayerID} on SMAPI {e.Peer.ApiVersion.ToString()} game version {e.Peer.GameVersion} platform {e.Peer.PlayerID}");
            }
            else
            {
                this.monitor.Log($"Peer connected: {e.Peer.PlayerID} on pure vanilla SDV");
            }

            var qfModPeer = e.Peer.GetMod(this.modManifest.UniqueID);

            if (qfModPeer == null)
            {
                this.monitor.Log($"Connected remote peer {e.Peer.PlayerID} doesn't run on SMAPI or they haven't installed the Quest Framework.", LogLevel.Warn);
                this.monitor.Log($"Unable to send QF shake message to: {e.Peer.PlayerID} (mismatch versions or dependencies)", LogLevel.Error);

                return;
            }

            if (qfModPeer.Version.IsOlderThan(this.modManifest.Version))
            {
                this.monitor.Log($"Remote peer {e.Peer.PlayerID} is running on outdated Quest Framework {qfModPeer.Version}", LogLevel.Warn);
            }
            else
            {
                this.monitor.Log($"Remote peer {e.Peer.PlayerID} is running on Quest Framework {qfModPeer.Version}");
            }

            var store = new QuestStateStoreData
            {
                { e.Peer.PlayerID, this.questStateStore.GetPayloadList(e.Peer.PlayerID) }
            };
            var stats = this.statsManager.GetStats();
            var message = new InitalMessage(this.questController.GetQuestIds(), store, stats);

            this.helper.SendMessage(
                message, "Init", new[] { this.modManifest.UniqueID }, new[] { e.Peer.PlayerID });
            this.monitor.Log($"Sent quests initial state to {e.Peer.PlayerID}");
        }

        public void Reset()
        {
            this.hasInitReceived = false;
        }

        private void OnMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != this.modManifest.UniqueID)
                return; // Ignore all messages which are not from quest framework

            if (e.Type == "SyncState")
            {
                var payload = e.ReadAs<StatePayload>();

                if (payload == null || payload.FarmerId != e.FromPlayerID)
                {
                    this.monitor.Log("Got payload which is null or farmer id doesn't match the 'FromPlayer' id. Discard.");
                    return;
                }

                this.monitor.Log($"Received quest state data from {e.FromPlayerID}");
                this.questStateStore.Commit(payload);
            }

            if (e.Type == "QuestStats")
            {
                var payload = e.ReadAs<Stats.Stats>();

                if (payload == null)
                {
                    this.monitor.Log("Got quest stats payload which is null. Discard.");
                    return;
                }

                this.statsManager.SetStats(e.FromPlayerID, payload);
            }

            if (e.Type == "Init" && !Context.IsMainPlayer && !this.hasInitReceived)
            {
                var inital = e.ReadAs<InitalMessage>();

                if (inital == null)
                    return;

                this.hasInitReceived = true;
                this.questController.SetQuestIdCache(inital.QuestIdList);
                this.questStateStore.RestoreData(inital.InitalStore);
                this.statsManager.SetStats(inital.QuestStats);
                this.monitor.Log($"Received inital data from host. World ready: {Context.IsWorldReady}");
                this.InitReceived?.Invoke(this, new EventArgs());
            }
        }
    }
}
