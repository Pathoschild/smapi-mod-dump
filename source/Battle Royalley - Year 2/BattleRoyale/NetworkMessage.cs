/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using BattleRoyale.Utils;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BattleRoyale
{
    public enum NetworkMessageDestination
    {
        ALL,
        ALL_OTHERS,
        SELF,
        SPECIFIC_PEER,
        HOST
    }

    class MessageData
    {
        public NetworkUtils.MessageTypes MessageType { get; set; }

        public long SourceFarmerId { get; set; }

        public List<object> Data { get; set; }

        public MessageData() { }

        public MessageData(NetworkUtils.MessageTypes messageType, Farmer sourceFarmer, List<object> data)
        {
            MessageType = messageType;
            SourceFarmerId = sourceFarmer.UniqueMultiplayerID;
            Data = data;
        }
    }

    abstract class NetworkMessage
    {
        public NetworkUtils.MessageTypes MessageType;

        public static Dictionary<NetworkUtils.MessageTypes, NetworkMessage> instances = new();

        public static void SetupMessages()
        {
            foreach (Type type in (from type in Assembly.GetExecutingAssembly().GetTypes()
                                   where type.IsClass && type.BaseType == typeof(NetworkMessage)
                                   select type))
            {
                NetworkMessage instance = ((NetworkMessage)Activator.CreateInstance(type));
                instances.Add(instance.MessageType, instance);
            }
        }

        public static void OnMessageReceive(object sender, ModMessageReceivedEventArgs e)
        {
            MessageData message = e.ReadAs<MessageData>();
            Farmer sourceFarmer = Game1.getFarmer(message.SourceFarmerId);

            if (instances.ContainsKey(message.MessageType))
                instances[message.MessageType].Receive(sourceFarmer, message.Data);
        }

        public static void Send(NetworkUtils.MessageTypes messageType, NetworkMessageDestination destination, List<object> data, long? targetPeer = null)
        {
            instances[messageType].Send(destination, data, targetPeer);
        }

        public virtual void Send(NetworkMessageDestination destination, List<object> data, long? targetPeer = null)
        {
            bool sendToSelf = false;
            List<long> toSendTo = new();

            switch (destination)
            {
                case NetworkMessageDestination.ALL:
                    sendToSelf = true;
                    foreach (Farmer player in Game1.getOnlineFarmers())
                    {
                        if (player != null && player != Game1.player)
                            toSendTo.Add(player.UniqueMultiplayerID);
                    }
                    break;
                case NetworkMessageDestination.ALL_OTHERS:
                    foreach (Farmer player in Game1.getOnlineFarmers())
                    {
                        if (player != null && player != Game1.player)
                            toSendTo.Add(player.UniqueMultiplayerID);
                    }
                    break;
                case NetworkMessageDestination.SELF:
                    sendToSelf = true;
                    break;
                case NetworkMessageDestination.SPECIFIC_PEER:
                    if (targetPeer == null)
                        throw new Exception("TargetPeer cannot be null with specific peer destination.");
                    if (targetPeer == Game1.player.UniqueMultiplayerID)
                    {
                        sendToSelf = true;
                        break;
                    }
                    toSendTo.Add((long)targetPeer);
                    break;
                case NetworkMessageDestination.HOST:
                    if (Game1.IsServer)
                    {
                        sendToSelf = true;
                        break;
                    }
                    toSendTo.Add(Game1.MasterPlayer.UniqueMultiplayerID);
                    break;
                default:
                    throw new Exception("Invalid network message destination.");
            }

            if (sendToSelf)
                Receive(Game1.player, data);

            MessageData messageData = new(MessageType, Game1.player, data);
            ModEntry.Multiplayer.SendMessage(messageData, "MessageData", playerIDs: toSendTo.ToArray());
        }

        public virtual void Receive(Farmer source, List<object> data) { }
    }
}
