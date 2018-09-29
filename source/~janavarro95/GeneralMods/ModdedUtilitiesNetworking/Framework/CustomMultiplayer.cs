using ModdedUtilitiesNetworking.Framework.Clients;
using ModdedUtilitiesNetworking.Framework.Messages;
using ModdedUtilitiesNetworking.Framework.Servers;
using Netcode;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using static ModdedUtilitiesNetworking.Framework.Delegates.DelegateInfo;

namespace ModdedUtilitiesNetworking.Framework
{
    public class CustomMultiplayer : StardewValley.Multiplayer
    {
        public List<long> hasConnectedOnce = new List<long>();

        public CustomMultiplayer()
        {
            this.hasConnectedOnce = new List<long>();

            
        }

        /*
        public override void writeObjectFull<T>(BinaryWriter writer, NetRoot<T> root, long? peer)
        {
            try
            {
                root.CreateConnectionPacket(writer, peer);
            }
            catch(Exception err)
            {

            }
        }
        */
        /*
        public override void readObjectDelta<T>(BinaryReader reader, NetRoot<T> root)
        {
            try
            {
                root.Read(reader);
            }
            catch(Exception err)
            {

            }
        }
        */

        public override void receiveWorldState(BinaryReader msg)
        {
            this.readObjectDelta<IWorldState>(msg, Game1.netWorldState);
            if (Game1.IsServer)
                return;
            int num1 = Game1.timeOfDay;
            Game1.netWorldState.Value.WriteToGame1();
            int num2 = Game1.timeOfDay;
            if (num1 == num2 || Game1.currentLocation == null || Game1.newDaySync != null)
                return;
            Game1.performTenMinuteClockUpdate();
        }



        public override bool isClientBroadcastType(byte messageType)
        {
            return true;
        }

        public override void processIncomingMessage(IncomingMessage msg)
        {
            if (msg.MessageType <= 19)
            {
                switch (msg.MessageType)
                {
                    case 0:
                        NetFarmerRoot netFarmerRoot = this.farmerRoot(msg.Reader.ReadInt64());
                        this.readObjectDelta<Farmer>(msg.Reader, (NetRoot<Farmer>)netFarmerRoot);
                        break;
                    case 2:
                        this.receivePlayerIntroduction(msg.Reader);
                        break;
                    case 3:
                        this.readActiveLocation(msg, false);
                        break;
                    case 4:
                        int eventId = msg.Reader.ReadInt32();
                        int tileX = msg.Reader.ReadInt32();
                        int tileY = msg.Reader.ReadInt32();
                        if (Game1.CurrentEvent != null)
                            break;
                        this.readWarp(msg.Reader, tileX, tileY, (Action)(() =>
                        {
                            Farmer farmerActor = (msg.SourceFarmer.NetFields.Root as NetRoot<Farmer>).Clone().Value;
                            farmerActor.currentLocation = Game1.currentLocation;
                            farmerActor.completelyStopAnimatingOrDoingAction();
                            farmerActor.hidden.Value = false;
                            Event eventById = Game1.currentLocation.findEventById(eventId, farmerActor);
                            Game1.currentLocation.startEvent(eventById);
                            farmerActor.Position = Game1.player.Position;
                        }));
                        break;
                    case 6:
                        GameLocation gameLocation = this.readLocation(msg.Reader);
                        if (gameLocation == null)
                            break;
                        this.readObjectDelta<GameLocation>(msg.Reader, gameLocation.Root);
                        break;
                    case 7:
                        GameLocation location = this.readLocation(msg.Reader);
                        if (location == null)
                            break;
                        location.temporarySprites.AddRange((IEnumerable<TemporaryAnimatedSprite>)this.readSprites(msg.Reader, location));
                        break;
                    case 8:
                        NPC character = this.readNPC(msg.Reader);
                        GameLocation targetLocation = this.readLocation(msg.Reader);
                        if (character == null || targetLocation == null)
                            break;
                        Game1.warpCharacter(character, targetLocation, BinaryReaderWriterExtensions.ReadVector2(msg.Reader));
                        break;
                    case 10:
                        this.receiveChatMessage(msg.SourceFarmer, BinaryReaderWriterExtensions.ReadEnum<LocalizedContentManager.LanguageCode>(msg.Reader), msg.Reader.ReadString());
                        break;
                    case 12:
                        try
                        {
                            this.receiveWorldState(msg.Reader);
                        }
                        catch(Exception err)
                        {

                        }
                        break;
                    case 13:
                        this.receiveTeamDelta(msg.Reader);
                        break;
                    case 14:
                        this.receiveNewDaySync(msg);
                        break;
                    case 15:
                        string messageKey = msg.Reader.ReadString();
                        string[] args = new string[(int)msg.Reader.ReadByte()];
                        for (int index = 0; index < args.Length; ++index)
                            args[index] = msg.Reader.ReadString();
                        this.receiveChatInfoMessage(msg.SourceFarmer, messageKey, args);
                        break;
                    case 17:
                        this.receiveFarmerGainExperience(msg);
                        break;
                    case 18:
                        this.parseServerToClientsMessage(msg.Reader.ReadString());
                        break;
                    case 19:
                        this.playerDisconnected(msg.SourceFarmer.UniqueMultiplayerID);
                        break;
                }
            }

            if (msg.MessageType == 20)
            {
                ModCore.monitor.Log("CUSTOM FUNCTION???");
            }
        }

        /// <summary>
        /// Sends an outgoing message to appropriate players.
        /// </summary>
        /// <param name="message"></param>
        private void sendMessage(OutgoingMessage message)
        {
                if (Game1.server != null)
                {
                    foreach (long peerId in (IEnumerable<long>)Game1.otherFarmers.Keys)
                    {
                        Game1.server.sendMessage(peerId, message);
                    }
                }
                if (Game1.client != null)
                {
                    if (Game1.client is CustomLidgrenClient)
                    {
                        (Game1.client as CustomLidgrenClient).sendMessage(message);
                        return;
                    }
                    if (Game1.client is CustomGalaxyClient)
                    {
                        (Game1.client as CustomGalaxyClient).sendMessage(message);
                        return;
                    }
                    ModCore.monitor.Log("Error sending server message!!!");

                
            }
        }


        /// <summary>
        /// Updates the server.
        /// </summary>
        public override void UpdateEarly()
        {
            if (Game1.CurrentEvent == null)
                this.removeDisconnectedFarmers();
            this.updatePendingConnections();
            if (Game1.server != null)
                (Game1.server as CustomGameServer).receiveMessages();
            else if (Game1.client != null)
                Game1.client.receiveMessages();
            this.tickFarmerRoots();
            this.tickLocationRoots();
        }

        public void baseProcessMessage(IncomingMessage message)
        {
            base.processIncomingMessage(message);
        }

        /// <summary>
        /// Creates a net outgoing message that is written specifically to call a void function when sent. USed to specifiy types and specific ways to handle them.
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="objectParametersType"></param>
        /// <param name="data"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public OutgoingMessage sendOutGoingMessageReturnVoid(string functionName, string objectParametersType, object data, Farmer source,Enums.MessageTypes.messageTypes sendingInfo)
        {
            byte bite = new byte();
            if (sendingInfo == Enums.MessageTypes.messageTypes.SendOneWay) bite = Enums.MessageTypes.SendOneWay;
            if (sendingInfo == Enums.MessageTypes.messageTypes.SendToAll) bite = Enums.MessageTypes.SendToAll;
            OutgoingMessage message = new OutgoingMessage(bite, source, makeDataArray(functionName, objectParametersType, data));
            return message;
        }

        public OutgoingMessage sendOutGoingMessageReturnVoid(string functionName, Type objectParametersType, object data, Farmer source, Enums.MessageTypes.messageTypes sendingInfo)
        {
            byte bite=new byte();
            if (sendingInfo == Enums.MessageTypes.messageTypes.SendOneWay) bite = Enums.MessageTypes.SendOneWay;
            if (sendingInfo == Enums.MessageTypes.messageTypes.SendToAll) bite = Enums.MessageTypes.SendToAll;
            OutgoingMessage message = new OutgoingMessage(bite, source, makeDataArray(functionName, objectParametersType.ToString(), data));
            return message;
        }

        public OutgoingMessage sendOutGoingMessageReturnVoid(string functionName, string objectParametersType, object data, Farmer source, Enums.MessageTypes.messageTypes sendingInfo, Farmer recipient)
        {
            byte bite = new byte();
            if (sendingInfo == Enums.MessageTypes.messageTypes.SendOneWay) bite = Enums.MessageTypes.SendOneWay;
            if (sendingInfo == Enums.MessageTypes.messageTypes.SendToAll) bite = Enums.MessageTypes.SendToAll;
            if (sendingInfo == Enums.MessageTypes.messageTypes.SendToSpecific) bite = Enums.MessageTypes.SendToSpecific;
            OutgoingMessage message = new OutgoingMessage(bite, source, makeDataArray(functionName, objectParametersType, data,recipient));
            return message;
        }

        public OutgoingMessage sendOutGoingMessageReturnVoid(string functionName, Type objectParametersType, object data, Farmer source, Enums.MessageTypes.messageTypes sendingInfo, Farmer recipient)
        {
            byte bite = new byte();
            if (sendingInfo == Enums.MessageTypes.messageTypes.SendOneWay) bite = Enums.MessageTypes.SendOneWay;
            if (sendingInfo == Enums.MessageTypes.messageTypes.SendToAll) bite = Enums.MessageTypes.SendToAll;
            if (sendingInfo == Enums.MessageTypes.messageTypes.SendToSpecific) bite = Enums.MessageTypes.SendToSpecific;
            OutgoingMessage message = new OutgoingMessage(bite, source, makeDataArray(functionName, objectParametersType.ToString(), data, recipient));
            return message;
        }



        public object[] makeDataArray(string functionName, string objectParametersType, object data)
        {
            DataInfo datainfo = new DataInfo(objectParametersType, data);
            object[] obj = new object[3]
            {
                functionName,
                typeof(DataInfo).ToString(),
                datainfo,
            };
            return obj;
        }

        public object[] makeDataArray(string functionName, string objectParametersType, object data, Farmer recipient)
        {
            DataInfo datainfo = new DataInfo(objectParametersType, data,recipient);
            object[] obj = new object[3]
            {
                functionName,
                typeof(DataInfo).ToString(),
                datainfo,
            };
            return obj;
        }

        public object[] makeDataArray(string functionName, string objectParametersType, object data, long recipient)
        {
            DataInfo datainfo = new DataInfo(objectParametersType, data, recipient.ToString());
            object[] obj = new object[3]
            {
                functionName,
                typeof(DataInfo).ToString(),
                datainfo,
            };
            return obj;
        }

        /// <summary>
        /// Creates all of the necessary parameters for the outgoing message to be sent to the server/client on what to do and how to handle the data sent.
        /// This message written will attempt to access a function that doesn't return anything. Essentially null.
        /// </summary>
        /// <param name="uniqueID"></param>
        /// <param name="classType"></param>
        /// <param name="data"></param>
        public void sendMessage(string uniqueID, Type classType, object data, Enums.MessageTypes.messageTypes sendingInfo, Farmer recipient = null)
        {
            Farmer f = Game1.player;

            if ((sendingInfo == Enums.MessageTypes.messageTypes.SendOneWay || sendingInfo == Enums.MessageTypes.messageTypes.SendToAll))
            {
                OutgoingMessage message = ModCore.multiplayer.sendOutGoingMessageReturnVoid(uniqueID, classType, data, f, sendingInfo);
                ModCore.multiplayer.sendMessage(message);
                return;
            }

            if (sendingInfo == Enums.MessageTypes.messageTypes.SendToSpecific && recipient != null)
            {
                OutgoingMessage message = ModCore.multiplayer.sendOutGoingMessageReturnVoid(uniqueID, classType, data, f, sendingInfo, recipient);
                ModCore.multiplayer.sendMessage(message);
                return;
            }

            if (sendingInfo == Enums.MessageTypes.messageTypes.SendToSpecific && recipient == null)
            {
                ModCore.monitor.Log("ERROR: Attempted to send a target specific message to a NULL recipient");
                return;
            }
        }


        /// <summary>
        /// A way to send mod info across the net.
        /// </summary>
        /// <param name="uniqueID"></param>
        /// <param name="classType"></param>
        /// <param name="data"></param>
        /// <param name="sendingInfo"></param>
        /// <param name="recipient"></param>
        public void sendMessage(string uniqueID, string classType, object data, Enums.MessageTypes.messageTypes sendingInfo, Farmer recipient=null)
        {
            Farmer f = Game1.player;

            if ((sendingInfo == Enums.MessageTypes.messageTypes.SendOneWay || sendingInfo == Enums.MessageTypes.messageTypes.SendToAll))
            {
                OutgoingMessage message = ModCore.multiplayer.sendOutGoingMessageReturnVoid(uniqueID, classType, data, f, sendingInfo);
                ModCore.multiplayer.sendMessage(message);
                return;
            }

            if (sendingInfo == Enums.MessageTypes.messageTypes.SendToSpecific && recipient!=null)
            {
                OutgoingMessage message = ModCore.multiplayer.sendOutGoingMessageReturnVoid(uniqueID, classType, data, f, sendingInfo,recipient);
                ModCore.multiplayer.sendMessage(message);
                return;
            }

            if (sendingInfo == Enums.MessageTypes.messageTypes.SendToSpecific && recipient == null)
            {
                ModCore.monitor.Log("ERROR: Attempted to send a target specific message to a NULL recipient");
                return;
            }
        }


        /// <summary>
        /// Get's the server host farmer.
        /// </summary>
        /// <returns></returns>
        public static Farmer getServerHost()
        {
            return Game1.serverHost.Value;
            
        }

        /// <summary>
        /// Get's the farmer in the player one slot also known as player 1.
        /// </summary>
        /// <returns></returns>
        public static Farmer getPlayerOne()
        {
            try
            {
                return Game1.getAllFarmers().ElementAt(0);
            }
            catch(Exception err)
            {
                return null;
            }
        }

        public override void addPlayer(NetFarmerRoot f)
        {
            long uniqueMultiplayerId = f.Value.UniqueMultiplayerID;
            //Surpresses the first connected chat info message on non modded clients.
            if (this.hasConnectedOnce.Contains(uniqueMultiplayerId))
            {
                base.addPlayer(f);
            }
            else
            {
                this.hasConnectedOnce.Add(uniqueMultiplayerId);
            }
        }

        /// <summary>
        /// Get's the farmer in the player two slot for the server.
        /// </summary>
        /// <returns></returns>
        public static Farmer getPlayerTwo()
        {
            try
            {
                
                return Game1.getAllFarmers().ElementAt(1);
            }
            catch(Exception err)
            {
                return null;
            }
           
        }

        /// <summary>
        /// Get's the farmer in the player three slot for the server.
        /// </summary>
        /// <returns></returns>
        public static Farmer getPlayerThree()
        {
            try
            {
                return Game1.getAllFarmers().ElementAt(2);
            }
            catch (Exception err)
            {
                return null;
            }
            
        }

        /// <summary>
        /// Get's the farmer in the player four slot for the server.
        /// </summary>
        /// <returns></returns>
        public static Farmer getPlayerFour()
        {
            try
            {
                return Game1.getAllFarmers().ElementAt(3);
            }
            catch (Exception err)
            {
                return null;
            }
            
        }


        /// <summary>
        /// Gets all farmers that are not the current player.
        /// </summary>
        /// <returns></returns>
        public static List<Farmer> getAllFarmersExceptThisOne()
        {
            Farmer player = Game1.player;

            Farmer player1 = getPlayerOne();
            Farmer player2 = getPlayerTwo();
            Farmer player3 = getPlayerThree();
            Farmer player4 = getPlayerFour();

            List<Farmer> otherFarmers=new List<Farmer>();

            if (player1 != null)
            {
                if (player != player1) otherFarmers.Add(player1);
            }
            if (player2 != null)
            {
                if (player != player2) otherFarmers.Add(player2);
            }
            if (player3 != null)
            {
                if (player != player3) otherFarmers.Add(player3);
            }
            if (player4 != null)
            {
                if (player != player4) otherFarmers.Add(player4);
            }
                return otherFarmers;
        }

        /// <summary>
        /// Gets a farmer from a player index number. Player 1 is 0, player two is one, etc. 
        /// </summary>
        /// <param name="number"></param>
        public Farmer getFarmerFromIndex(int number)
        {
            if (number == 0) return getPlayerOne();
            if (number == 1) return getPlayerTwo();
            if (number == 2) return getPlayerThree();
            if (number == 3) return getPlayerFour();

            try
            {
                Game1.getAllFarmers().ElementAt(number);
            }
            catch(Exception err)
            {
                return null;
            }
            return null;
        }
    }
}
