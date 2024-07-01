/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/1Avalon/Avas-Stardew-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendshipStreaks
{
    public class NetworkDataManager
    {
        public Dictionary<long, Dictionary<string, FriendshipStreak>> streakData;

        private IMonitor monitor = ModEntry.instance.Monitor;
        public NetworkDataManager()
        {
            streakData = new Dictionary<long, Dictionary<string, FriendshipStreak>>();
        }

        private void AddToData<T>(ref Dictionary<long, T> data, string type, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == ModEntry.instance.ModManifest.UniqueID && e.Type == type)
            {
                T obj = e.ReadAs<T>();

                if (!data.ContainsKey(e.FromPlayerID))
                {
                    data.Add(e.FromPlayerID, obj);
                    ModEntry.instance.Monitor.Log($"Added new profile for player {e.FromPlayerID} for object type {obj}");
                    return;
                }
                data[e.FromPlayerID] = obj;
            }
        }

        public void SendDataToFarmhand(long playerId)
        {
            if (streakData.TryGetValue(playerId, out Dictionary<string, FriendshipStreak> data)) 
            {
                ModEntry.instance.Helper.Multiplayer.SendMessage(data, "FriendshipStreaks", new string[] { ModEntry.instance.ModManifest.UniqueID}, new long[] { playerId });
                return;
            }
            ModEntry.instance.Monitor.Log("The data of the requesting farmhand was empty", LogLevel.Warn);
        }

        public void ReceiveMessage(ModMessageReceivedEventArgs e)
        {
            AddToData<Dictionary<string, FriendshipStreak>>(ref streakData, "FriendshipStreaks", e);

            ModEntry.instance.Monitor.Log($"Received message from {e.FromPlayerID}. Type was {e.Type}");
        }

        public void SendDataToHost()
        {
            long hostId = 0;
            foreach (IMultiplayerPeer peer in ModEntry.instance.Helper.Multiplayer.GetConnectedPlayers())
                hostId = peer.IsHost ? peer.PlayerID : 0;

            ModEntry.instance.Helper.Multiplayer.SendMessage<Dictionary<string, FriendshipStreak>>(ModEntry.streaks, "FriendshipStreaks", new string[] { ModEntry.instance.ModManifest.UniqueID }, new long[] { hostId });
        }

        public void OnMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != ModEntry.instance.ModManifest.UniqueID)
                return;

            if (e.Type == "DataRequest")
            {
                SendDataToFarmhand(e.FromPlayerID);
                return;
            }

            if (Context.IsMainPlayer)
            {
                ReceiveMessage(e);
                monitor.Log("Received message. Processing...");
                ModEntry.instance.Helper.Data.WriteSaveData<NetworkDataManager>("NetworkDataManager", ModEntry.networkDataManager);
            }
            else
            {
                monitor.Log("Receives message from host");
                var data = e.ReadAs<Dictionary<string, FriendshipStreak>>();
                ModEntry.streaks = data;
            }
        }

        public void RequestData()
        {
            long hostId = 0;
            foreach (IMultiplayerPeer peer in ModEntry.instance.Helper.Multiplayer.GetConnectedPlayers())
                hostId = peer.IsHost ? peer.PlayerID : 0;

            ModEntry.instance.Helper.Multiplayer.SendMessage<string>("", "DataRequest", new string[] { ModEntry.instance.ModManifest.UniqueID }, new long[] { hostId });
        }
        public Dictionary<string, FriendshipStreak> GetDataForPlayer(long playerId)
        {
            if (streakData.TryGetValue(playerId, out var data))
            {
                return data;
            }
            monitor.Log($"Player with id {playerId}");
            return null;
        }
    }
}
