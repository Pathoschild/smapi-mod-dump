/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using QuestFramework.Quests;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework.Store
{
    internal class QuestStateStoreData : Dictionary<long, Dictionary<string, StatePayload>> { }
    internal class QuestStateStore
    {
        private QuestStateStoreData Store { get; set; }
        public IDataHelper Helper { get; }
        public IMonitor Monitor { get; }

        public QuestStateStore(IDataHelper helper, IMonitor monitor)
        {
            this.Store = new QuestStateStoreData();
            this.Helper = helper;
            this.Monitor = monitor;
        }

        public void Persist()
        {
            if (Context.IsMainPlayer)
            {
                this.Helper.WriteSaveData("questStateStore", this.Store);
                this.Monitor.Log("Store data was written to savefile.");
            }
        }

        public void RestoreData()
        {
            if (Context.IsMainPlayer)
            {
                var data = this.Helper.ReadSaveData<QuestStateStoreData>("questStateStore");

                if (data == null)
                {
                    this.Monitor.Log("No quests state data to restore from savefile.");
                    return;
                }

                this.Store = data;
                this.Monitor.Log("Quests store data was restored from savefile.");
            }
        }

        public void Verify(long farmerUid, IEnumerable<CustomQuest> customQuests)
        {
            var payloadList = this.GetPayloadList(farmerUid);
            foreach (var customQuest in customQuests)
            {
                StatePayload payload = payloadList[customQuest.Name];
                if (customQuest is IStateRestorable restorable && !restorable.VerifyState(payload))
                    this.Monitor.Log($"State for quest `{customQuest.Name}` for farmer UID {farmerUid} mismatch! " +
                        $"Did you call Sync() in your CustomQuest type?", LogLevel.Warn);
            }
        }

        public void RestoreData(QuestStateStoreData data)
        {
            this.Store = data;
            this.Monitor.Log("Quests store data was restored from given payload.");
        }

        public Dictionary<string, StatePayload> GetPayloadList(long farmerId)
        {
            if (this.Store.TryGetValue(farmerId, out var payloadList))
                return payloadList;

            return null;
        }

        internal void Commit(StatePayload payload)
        {
            if (!this.Store.ContainsKey(payload.FarmerId))
                this.Store.Add(payload.FarmerId, new Dictionary<string, StatePayload>());

            this.Store[payload.FarmerId][payload.QuestName] = payload;
            this.Monitor.Log($"Payload `{payload.QuestName}/{payload.FarmerId}` type `{payload.StateData.Type}` commited to store");
        }

        internal void Clean()
        {
            this.Store = new QuestStateStoreData();
        }
    }
}
