using QuestFramework.Framework.Controllers;
using QuestFramework.Framework.Networking;
using QuestFramework.Framework.Store;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework.Networing
{
    class NetworkOperator
    {
        private readonly IMultiplayerHelper helper;
        private readonly QuestStateStore questStateStore;
        private readonly QuestController questController;
        private readonly IManifest modManifest;
        private readonly IMonitor monitor;

        private bool hasInitReceived = false;

        public event EventHandler InitReceived;

        public NetworkOperator(IMultiplayerHelper helper, IMultiplayerEvents events, QuestStateStore questStateStore, QuestController questController, IManifest modManifest, IMonitor monitor)
        {
            this.helper = helper;
            this.questStateStore = questStateStore;
            this.questController = questController;
            this.modManifest = modManifest;
            this.monitor = monitor;

            events.ModMessageReceived += this.OnMessageReceived;
            events.PeerConnected += this.OnClientConnected;
        }

        [EventPriority(EventPriority.High + 100)]
        private void OnClientConnected(object sender, PeerConnectedEventArgs e)
        {
            if (Context.IsMainPlayer && e.Peer.Mods.Any(m => m.ID == this.modManifest.UniqueID))
            {
                var store = new QuestStateStoreData
                {
                    { e.Peer.PlayerID, this.questStateStore.GetPayloadList(e.Peer.PlayerID) }
                };
                var message = new InitalMessage(this.questController.GetQuestIds(), store);

                this.helper.SendMessage(
                    message, "Init", new[] { this.modManifest.UniqueID }, new[] { e.Peer.PlayerID });
                this.monitor.Log($"Sent quests initial state to {e.Peer.PlayerID}");
            }
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

                this.questStateStore.Commit(payload);
            }

            if (e.Type == "Init" && !Context.IsMainPlayer && !this.hasInitReceived)
            {
                var inital = e.ReadAs<InitalMessage>();

                if (inital == null)
                    return;

                this.hasInitReceived = true;
                this.questController.SetQuestIdCache(inital.QuestIdList);
                this.questStateStore.RestoreData(inital.InitalStore);
                this.monitor.Log($"Received inital data from host. World ready: {Context.IsWorldReady}");
                this.InitReceived?.Invoke(this, new EventArgs());
            }
        }
    }
}
