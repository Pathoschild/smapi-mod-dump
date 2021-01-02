/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/PlayerIncomeStats
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace PlayerIncomeStats.Core
{
    public class NetworkManager
    {
        public List<DataBlock> entries = new List<DataBlock>();
        private readonly string entriesChanged = "EntriesChanged";
        private readonly string peerConnected = "PeerConnected";
        private readonly IModHelper helper = ModEntry.instance.Helper;
        private readonly string prefix = "PlayerIncomeStatsSaveEntry_";

        public void AddEntries(List<DataBlock> data, bool sync = true, bool _override = false)
        {
            if (_override)
            {
                data.ForEach(db => db.CalculateEntryString());
                entries = data.ToList();
            }
            else
            {
                for (int i = 0; i < data.Count; i++)
                {
                    if (entries.FirstOrDefault(d =>
                    {
                        if (d.Equals(data[i]))
                        {
                            d.MoneyMade = data[i].MoneyMade;
                            d.MoneyMadeTemp = data[i].MoneyMadeTemp;
                            d.Name = data[i].Name;
                            d.justChangedColor = data[i].justChangedColor;
                            d.justChangedTimer = data[i].justChangedTimer;
                            d.CalculateEntryString();
                            return true;
                        }
                        return false;
                    }) == null)
                    {
                        data[i].CalculateEntryString();
                        entries.Add(data[i]);
                    }
                }
            }
            ModEntry.instance.userInterfaceManager.OnEntriesChanged();
            if (sync)
                helper.Multiplayer.SendMessage(data, entriesChanged);
        }

        public List<DataBlock> GetAllData()
        {
            if (!Context.IsMainPlayer) return null;

            List<DataBlock> entries = new List<DataBlock>();
            foreach (Farmer farmer in Game1.getAllFarmers())
            {
                entries.Add(GetDataSafe(farmer.uniqueMultiplayerID));
            }
            return entries;
        }

        public DataBlock GetDataSafe(long id)
        {
            if (!Context.IsMainPlayer) return null;

            DataBlock block = helper.Data.ReadSaveData<DataBlock>(prefix + id.ToString());
            if (block == null)
            {
                block = new DataBlock(id, Game1.getFarmerMaybeOffline(id).Name);
                helper.Data.WriteSaveData(prefix + id.ToString(), block);
            }
            return block;
        }

        public DataBlock GetEntrySafe(long id) => GetEntryByUniqueID(id) ?? GetDataSafe(id);

        public DataBlock GetEntryByUniqueID(long id)
            => entries.FirstOrDefault(db => db.UniqueID == id);

        public void OnSaving(object sender, SavingEventArgs e)
        {
            ModEntry.instance.flag = false;
            if (!Context.IsMainPlayer || entries == null || entries.Count == 0) return;

            entries.ForEach(db => db.Name = Game1.getFarmerMaybeOffline(db.UniqueID)?.Name);
            foreach (DataBlock block in entries)
            {
                block.MoneyMade += block.MoneyMadeTemp;
                block.MoneyMadeTemp = 0;
                helper.Data.WriteSaveData(prefix + block.UniqueID.ToString(), block);
            }
            AddEntries(entries);
        }

        public void OnItemShipped(int price, Farmer who, bool shipped = true)
        {
            if (price == 0 || who == null)
                return;

            DataBlock block = GetEntrySafe(who.uniqueMultiplayerID);
            block.Name = who.Name;
            block.MoneyMadeTemp += shipped ? price : -price;
            block.justChangedColor = shipped ? Color.YellowGreen : Color.Red;
            block.justChangedTimer = 150;
            List<DataBlock> data = new List<DataBlock> { block };
            AddEntries(data);
        }

        public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != ModEntry.instance.ModManifest.UniqueID) return;

            if (e.Type == entriesChanged)
                AddEntries(e.ReadAs<List<DataBlock>>(), false);
            else if (e.Type == peerConnected)
                AddEntries(e.ReadAs<List<DataBlock>>(), false, true);
        }

        public void OnPeerConnected(object sender, PeerConnectedEventArgs e)
        {
            if (!Context.IsMainPlayer) return;

            long id = e.Peer.PlayerID;
            DataBlock block = GetEntrySafe(id);
            block.Name = Game1.getFarmerMaybeOffline(id).Name;
            AddEntries(new List<DataBlock> { block }, false);
            helper.Multiplayer.SendMessage(entries, peerConnected, playerIDs: new[] { id });
        }

        public void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            ModEntry.instance.flag = false;
            if (!Context.IsMainPlayer) return;
            AddEntries(GetAllData(), _override: true);
        }
    }
}