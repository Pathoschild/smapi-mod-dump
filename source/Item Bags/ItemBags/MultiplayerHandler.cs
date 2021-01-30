/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using ItemBags.Bags;
using ItemBags.Persistence;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemBags
{
    public static class MultiplayerHandler
    {
        //  Note: To test multiplayer locally:
        //  1. Launch SMAPI through the visual studio debugger
        //  2. Launch a 2nd instance of SMAPI normally
        //  3. One one instance, host a multiplayer lobby
        //  4. On the second instance, connect to 127.0.0.1

        private static IModHelper Helper { get; set; }
        private static IMonitor Monitor { get { return ItemBagsMod.ModInstance.Monitor; } }

        internal static void OnModEntry(IModHelper Helper)
        {
            MultiplayerHandler.Helper = Helper;

            Helper.Events.Multiplayer.PeerContextReceived += Multiplayer_PeerContextReceived;
            Helper.Events.Multiplayer.ModMessageReceived += Multiplayer_ModMessageReceived;
        }

        private static void Multiplayer_PeerContextReceived(object sender, PeerContextReceivedEventArgs e)
        {
            //  Whenever a new client connects to the host, force the NetStrings that store bag data to get re-loaded on the other clients
            if (Context.IsMainPlayer)
            {
                ItemBag.GetAllBags(false).ForEach(x => x.Resync());

                //  I have no idea why, but it seems like the items in the connected farmhand's inventory are getting replaced with new instances after I've synchronized the current instances.
                //  So to maintain bags in their inventory, send the data to the peer using IMultiplayerEvents.SendMessage/IMultiplayerEvents.ModMessageReceived.
                //  This will force the connected client to overwrite their inventory with the correct data.
                Farmer ConnectedFarmer = Game1.getAllFarmers().First(x => x.UniqueMultiplayerID == e.Peer.PlayerID);
                Dictionary<int, string> PendingSync = new Dictionary<int, string>();
                for (int i = 0; i < ConnectedFarmer.Items.Count; i++)
                {
                    Item Item = ConnectedFarmer.Items[i];
                    if (Item != null && Item is ItemBag Bag && Bag.TrySerializeToString(out string DataString, out Exception Error))
                    {
                        PendingSync.Add(i, DataString);
                    }
                }
                if (PendingSync.Any())
                {
                    Helper.Multiplayer.SendMessage(PendingSync, ForceResyncCommandType, new string[] { ItemBagsMod.ModUniqueId }, new long[] { ConnectedFarmer.UniqueMultiplayerID });
                }

                Helper.Multiplayer.SendMessage("", OnConnectedCommandType, new string[] { ItemBagsMod.ModUniqueId }, new long[] { ConnectedFarmer.UniqueMultiplayerID });
            }
        }

        private const string ForceResyncCommandType = "ForceFarmhandResync";
        private const string OnConnectedCommandType = "OnConnectedToHost";

        private static void Multiplayer_ModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == ItemBagsMod.ModUniqueId)
            {
                if (e.Type == ForceResyncCommandType)
                {
                    Dictionary<int, string> LatestItemData = e.ReadAs<Dictionary<int, string>>();
                    foreach (var KVP in LatestItemData)
                    {
                        int InventoryIndex = KVP.Key;
                        string Data = KVP.Value;
                        if (Game1.player.Items[InventoryIndex] is ItemBag IB)
                        {
                            IB.TryDeserializeFromString(Data, out Exception Error);
                        }
                    }
                }
                else if (e.Type == OnConnectedCommandType)
                {
                    ModdedBag.OnConnectedToHost();
                }
            }
        }
    }
}
