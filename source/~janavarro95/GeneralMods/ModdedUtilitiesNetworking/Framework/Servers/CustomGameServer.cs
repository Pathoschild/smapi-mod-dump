using Microsoft.Xna.Framework;
using ModdedUtilitiesNetworking.Framework.Clients;
using ModdedUtilitiesNetworking.Framework.Extentions;
using Netcode;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Minigames;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ModdedUtilitiesNetworking.Framework.Servers
{
    public class CustomGameServer : IGameServer
    {     
            public List<Server> servers = new List<Server>();
            public List<Action> pendingGameAvailableActions = new List<Action>();
        public List<long> hasPlayerDisconnectedOnce = new List<long>();

        public CustomGameServer()
        {
            this.servers = new List<Server>();
            this.servers.Add(ModCore.multiplayer.InitServer((Server)new CustomLidgrenServer((IGameServer)this)));
            ModCore.monitor.Log("Custom Lidgren Server Created");
            ModCore.monitor.Log("Custom Game Server Created");
            hasPlayerDisconnectedOnce = new List<long>();
            try {
                if (Program.sdk.Networking == null)
                    return;
                Server server = Program.sdk.Networking.CreateServer((IGameServer)this);
                if (server != null)
                {
                    this.servers.Add(server);
                    ModCore.monitor.Log("Custom Galaxy Server Created");
                }
            }
            catch(Exception err)
            {
                ModCore.monitor.Log("Issue creating custom galaxy game server. If you are not playing via GOG you may ignore this message.");
                return;
            }
        }

    public int connectionsCount
    {
      get
      {
        return Enumerable.Sum<Server>((IEnumerable<Server>) this.servers, (Func<Server, int>) (s => s.connectionsCount));
      }
    }



        public string getInviteCode()
        {
            foreach (Server server in this.servers)
            {
                string inviteCode = server.getInviteCode();
                if (inviteCode != null)
                    return inviteCode;
            }
            return (string)null;
        }

    public string getUserName(long farmerId)
    {
      foreach (Server server in this.servers)
      {
        if (server.getUserName(farmerId) != null)
          return server.getUserName(farmerId);
      }
      return (string) null;
    }

        protected void initialize()
        {
            foreach (Server server in this.servers)
            {
                if(server!=null) server.initialize();
            }
            this.updateLobbyData();
        }

        public void setPrivacy(ServerPrivacy privacy)
        {
            foreach (Server server in this.servers)
                server.setPrivacy(privacy);
        }

        public void stopServer()
        {
            foreach (Server server in this.servers)
                server.stopServer();
        }

        public void receiveMessages()
        {
           
            foreach (Server server in this.servers)
            {
                if (server == null) continue;
               server.receiveMessages();              
            }
            if (!this.isGameAvailable())
                return;
            foreach (Action action in this.pendingGameAvailableActions)
                action();
            this.pendingGameAvailableActions.Clear();
        }

        public void sendMessage(long peerId, OutgoingMessage message)
        {
            foreach (Server server in this.servers)
                if (server is CustomLidgrenServer || server is LidgrenServer)
                {
                    server.sendMessage(peerId, message);
                }
                else //If I am not a custom lidgren server ignore sending modding info to clients. The lidgren server should handle all of that.
                {
                    if (message.MessageType <= 19)
                    {
                        server.sendMessage(peerId, message);
                    }
                }
        }

        public bool canAcceptIPConnections()
        {
            return Enumerable.Aggregate<bool, bool>(Enumerable.Select<Server, bool>((IEnumerable<Server>)this.servers, (Func<Server, bool>)(s => s.canAcceptIPConnections())), false, (Func<bool, bool, bool>)((a, b) => a | b));
        }

        public bool canOfferInvite()
        {
            if (this.servers == null)
            {
                ModCore.monitor.Log("WAIT WHY IS THIS HAPPENING????");
            }
            foreach(var v in this.servers)
            {
                if (v.canOfferInvite() == true) continue;
                else return false;
            }
            return true;

            //return Enumerable.Aggregate<bool, bool>(Enumerable.Select<Server, bool>((IEnumerable<Server>)this.servers, (Func<Server, bool>)(s => s.canOfferInvite())), false, (Func<bool, bool, bool>)((a, b) => a | b));
        }

        public void offerInvite()
        {
            foreach (Server server in this.servers)
            {
                if (server.canOfferInvite())
                    server.offerInvite();
            }
        }

        public bool connected()
        {
            foreach (Server server in this.servers)
            {
                if (!server.connected())
                    return false;
            }
            return true;
        }

        public void sendMessage(long peerId, byte messageType, Farmer sourceFarmer, params object[] data)
        {
            this.sendMessage(peerId, new OutgoingMessage(messageType, sourceFarmer, data));
        }

        public void sendMessages()
        {
            foreach (Farmer farmer in (IEnumerable<Farmer>)Game1.otherFarmers.Values)
            {
                foreach (OutgoingMessage message in (IEnumerable<OutgoingMessage>)farmer.messageQueue)
                    this.sendMessage(farmer.UniqueMultiplayerID, message);
                farmer.messageQueue.Clear();
            }
        }

        public void startServer()
        {
            Console.WriteLine("Starting server. Protocol version: " + ModCore.multiplayer.protocolVersion);
            this.initialize();
            if (Game1.serverHost.Value == null)
                Game1.serverHost = new NetFarmerRoot();
            Game1.serverHost.Value = Game1.player;
            Game1.serverHost.MarkClean();
            Game1.serverHost.Clock.InterpolationTicks = ModCore.multiplayer.defaultInterpolationTicks;
            if (Game1.netWorldState.Value == (NetRef<IWorldState>)null)
                Game1.netWorldState = new NetRoot<IWorldState>((IWorldState)new NetWorldState());
            Game1.netWorldState.Clock.InterpolationTicks = 0;
            Game1.netWorldState.Value.UpdateFromGame1();
        }

        public void sendServerIntroduction(long peer)
        {
            this.sendLocation(peer, (GameLocation)Game1.getFarm());
            this.sendLocation(peer, Game1.getLocationFromName("FarmHouse"));
            this.sendMessage(peer, new OutgoingMessage((byte)1, Game1.serverHost.Value, new object[3]
            {
        (object) ModCore.multiplayer.writeObjectFullBytes<Farmer>((NetRoot<Farmer>) Game1.serverHost, new long?(peer)),
        (object) ModCore.multiplayer.writeObjectFullBytes<FarmerTeam>(Game1.player.teamRoot, new long?(peer)),
        (object) ModCore.multiplayer.writeObjectFullBytes<IWorldState>(Game1.netWorldState, new long?(peer))
            }));
            foreach (KeyValuePair<long, NetRoot<Farmer>> keyValuePair in Game1.otherFarmers.Roots)
            {
                if (keyValuePair.Key != Game1.player.UniqueMultiplayerID && keyValuePair.Key != peer)
                    this.sendMessage(peer, new OutgoingMessage((byte)2, keyValuePair.Value.Value, new object[2]
                    {
            (object) this.getUserName(keyValuePair.Value.Value.UniqueMultiplayerID),
            (object) ModCore.multiplayer.writeObjectFullBytes<Farmer>(keyValuePair.Value, new long?(peer))
                    }));
            }
        }



        public void playerDisconnected(long disconnectee)
        {
            Farmer sourceFarmer = (Farmer)null;
            Game1.otherFarmers.TryGetValue(disconnectee, out sourceFarmer);
            ModCore.multiplayer.playerDisconnected(disconnectee);
            if (sourceFarmer == null)
                return;

            
            if (this.hasPlayerDisconnectedOnce.Contains(disconnectee))
            {
                OutgoingMessage message = new OutgoingMessage((byte)19, sourceFarmer, new object[0]);
                foreach (long peerId in (IEnumerable<long>)Game1.otherFarmers.Keys)
                {
                    if (peerId != disconnectee)
                        this.sendMessage(peerId, message);
                }
            }
            else
            {
                this.hasPlayerDisconnectedOnce.Add(disconnectee);
            }
            /*
            OutgoingMessage message = new OutgoingMessage((byte)19, sourceFarmer, new object[0]);
            foreach (long peerId in (IEnumerable<long>)Game1.otherFarmers.Keys)
            {
                if (peerId != disconnectee)
                    this.sendMessage(peerId, message);
            }
            */

        }



        public bool isGameAvailable()
        {
            bool flag1 = Game1.currentMinigame is Intro || Game1.Date.DayOfMonth == 0;
            bool flag2 = Game1.CurrentEvent != null && Game1.CurrentEvent.isWedding;
            bool flag3 = Game1.newDaySync != null && !Game1.newDaySync.hasFinished();
            if (!Game1.isFestival() && !flag2 && !flag1)
                return !flag3;
            return false;
        }

        public bool whenGameAvailable(Action action)
        {
            if (this.isGameAvailable())
            {
                action();
                return true;
            }
            this.pendingGameAvailableActions.Add(action);
            return false;
        }

        private void rejectFarmhandRequest(string userID, NetFarmerRoot farmer, Action<OutgoingMessage> sendMessage)
        {
            this.sendAvailableFarmhands(userID, sendMessage);
            Console.WriteLine("Rejected request for farmhand " + (farmer.Value != null ? farmer.Value.UniqueMultiplayerID.ToString() : "???"));
        }

        private IEnumerable<Cabin> cabins()
        {
            if (Game1.getFarm() != null)
            {
                foreach (Building building in Game1.getFarm().buildings)
                {
                    if ((int)((NetFieldBase<int, NetInt>)building.daysOfConstructionLeft) <= 0 && building.indoors.Value is Cabin)
                        yield return building.indoors.Value as Cabin;
                }
            }
        }

        private bool authCheck(string userID, Farmer farmhand)
        {
            if (!Game1.options.enableFarmhandCreation && !(bool)((NetFieldBase<bool, NetBool>)farmhand.isCustomized))
                return false;
            if (!(userID == "") && !(farmhand.userID.Value == ""))
                return farmhand.userID.Value == userID;
            return true;
        }

        private Farmer findOriginalFarmhand(Farmer farmhand)
        {
            foreach (Cabin cabin in this.cabins())
            {
                if (cabin.getFarmhand().Value.UniqueMultiplayerID == farmhand.UniqueMultiplayerID)
                    return cabin.getFarmhand().Value;
            }
            return (Farmer)null;
        }

        public void checkFarmhandRequest(string userID, NetFarmerRoot farmer, Action<OutgoingMessage> sendMessage, Action approve)
        {
            if (farmer.Value == null)
            {
                this.rejectFarmhandRequest(userID, farmer, sendMessage);
            }
            else
            {
                long id = farmer.Value.UniqueMultiplayerID;
                if (this.whenGameAvailable((Action)(() =>
                {
                    Farmer originalFarmhand = this.findOriginalFarmhand(farmer.Value);
                    if (originalFarmhand == null)
                    {
                        Console.WriteLine("Rejected request for farmhand " + (object)id + ": doesn't exist");
                        this.rejectFarmhandRequest(userID, farmer, sendMessage);
                    }
                    else if (!this.authCheck(userID, originalFarmhand))
                    {
                        Console.WriteLine("Rejected request for farmhand " + (object)id + ": authorization failure");
                        this.rejectFarmhandRequest(userID, farmer, sendMessage);
                    }
                    else if (Game1.otherFarmers.ContainsKey(id) && !ModCore.multiplayer.isDisconnecting(id) || Game1.serverHost.Value.UniqueMultiplayerID == id)
                    {
                        Console.WriteLine("Rejected request for farmhand " + (object)id + ": already in use");
                        this.rejectFarmhandRequest(userID, farmer, sendMessage);
                    }
                    else
                    {
                        Console.WriteLine("Approved request for farmhand " + (object)id);
                        approve();
                        ModCore.multiplayer.addPlayer(farmer);
                        ModCore.multiplayer.broadcastPlayerIntroduction(farmer);
                        this.sendServerIntroduction(id);
                        this.updateLobbyData();
                    }
                })))
                    return;
                Console.WriteLine("Postponing request for farmhand " + (object)id);
                sendMessage(new OutgoingMessage((byte)11, Game1.player, new object[1]
                {
          (object) "Strings\\UI:Client_WaitForHostAvailability"
                }));
            }
        }

        public void sendAvailableFarmhands(string userID, Action<OutgoingMessage> sendMessage)
        {
            List<NetRef<Farmer>> list = new List<NetRef<Farmer>>();
            Game1.getFarm();
            foreach (Cabin cabin in this.cabins())
            {
                NetRef<Farmer> farmhand = cabin.getFarmhand();
                if ((!farmhand.Value.isActive() || ModCore.multiplayer.isDisconnecting(farmhand.Value.UniqueMultiplayerID)) && this.authCheck(userID, farmhand.Value))
                    list.Add(farmhand);
            }
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter((Stream)memoryStream))
                {
                    writer.Write(Game1.year);
                    writer.Write(Utility.getSeasonNumber(Game1.currentSeason));
                    writer.Write(Game1.dayOfMonth);
                    writer.Write((byte)list.Count);
                    foreach (NetRef<Farmer> netRef in list)
                    {
                        try
                        {
                            netRef.Serializer = SaveGame.farmerSerializer;
                            netRef.WriteFull(writer);
                        }
                        finally
                        {
                            netRef.Serializer = (XmlSerializer)null;
                        }
                    }
                    memoryStream.Seek(0L, SeekOrigin.Begin);
                    sendMessage(new OutgoingMessage((byte)9, Game1.player, new object[1]
                    {
            (object) memoryStream.ToArray()
                    }));
                }
            }
        }

        private void sendLocation(long peer, GameLocation location)
        {
            this.sendMessage(peer, (byte)3, Game1.serverHost.Value, (object)ModCore.multiplayer.writeObjectFullBytes<GameLocation>(ModCore.multiplayer.locationRoot(location), new long?(peer)));
        }

        private void warpFarmer(Farmer farmer, short x, short y, string name, bool isStructure)
        {
            GameLocation locationFromName = Game1.getLocationFromName(name, isStructure);
            farmer.currentLocation = locationFromName;
            farmer.Position = new Vector2((float)((int)x * 64), (float)((int)y * 64 - (farmer.Sprite.getHeight() - 32) + 16));
            this.sendLocation(farmer.UniqueMultiplayerID, locationFromName);
        }

        public void processIncomingMessage(IncomingMessage message)
        {
            //ModCore.monitor.Log("MESSAGES????");
            switch (message.MessageType)
            {
                case 2:
                    message.Reader.ReadString();
                    ModCore.multiplayer.processIncomingMessage(message);
                    break;
                case 5:
                    this.warpFarmer(message.SourceFarmer, message.Reader.ReadInt16(), message.Reader.ReadInt16(), message.Reader.ReadString(), (int)message.Reader.ReadByte() == 1);
                    break;
                case Enums.MessageTypes.SendOneWay:
                    object[] obj = message.Reader.ReadModdedInfoPacket();
                    string functionName = (string)obj[0];
                    string classType = (string)obj[1];
                    object actualObject = ModCore.processTypesToRead(message.Reader, classType);
                    ModCore.processVoidFunction(functionName, actualObject);
                    break;
                case Enums.MessageTypes.SendToAll:
                    object[] obj2 = message.Reader.ReadModdedInfoPacket();
                    string functionName2 = (string)obj2[0];
                    string classType2 = (string)obj2[1];
                    object actualObject2 = ModCore.processTypesToRead(message.Reader, classType2);
                    ModCore.processVoidFunction(functionName2, actualObject2);
                    this.rebroadcastClientMessageToAllClients(message);
                    break;
                case Enums.MessageTypes.SendToSpecific:
                    //Read in specific info.
                    object[] obj3 = message.Reader.ReadModdedInfoPacket();
                    string functionName3 = (string)obj3[0];
                    string classType3 = (string)obj3[1];
                    //Parse 
                    object actualObject3 = ModCore.processTypesToRead(message.Reader, classType3);
                    DataInfo info = (DataInfo)actualObject3;

                    if (info.recipientID == Game1.player.UniqueMultiplayerID.ToString())
                    {
                        ModCore.processVoidFunction(functionName3, actualObject3);
                    }
                    else
                    {
                        this.sendMessageToSpecificClient(message, info);
                    }

                    break;
                default:
                    ModCore.multiplayer.processIncomingMessage(message);
                    break;
            }
            //this.rebroadcastClientMessage(message);
        }

        /// <summary>
        /// Takes an incoming client message and sends it back to all clients to process.
        /// </summary>
        /// <param name="message"></param>
        private void rebroadcastClientMessageToAllClients(IncomingMessage message)
        {
            byte messageType = Enums.MessageTypes.SendOneWay;
            Farmer f = Game1.player;         
            object data = message.Data.Clone();
            OutgoingMessage message1 = new OutgoingMessage(messageType, f, data);
            foreach (long peerId in (IEnumerable<long>)Game1.otherFarmers.Keys)
            {
                    this.sendMessage(peerId, message1);
            }
        }

        private void sendMessageToSpecificClient(IncomingMessage message, long farmerID)
        {
            byte messageType = Enums.MessageTypes.SendOneWay;
            Farmer f = Game1.player;
            object data = message.Data.Clone();
            OutgoingMessage message1 = new OutgoingMessage(messageType, f, data);
            this.sendMessage(farmerID, message1);
        }

        private void sendMessageToSpecificClient(IncomingMessage message, DataInfo info)
        {
            byte messageType = Enums.MessageTypes.SendOneWay;
            Farmer f = Game1.player;
            object data = message.Data.Clone();
            OutgoingMessage message1 = new OutgoingMessage(messageType, f, data);
            this.sendMessage(long.Parse(info.recipientID), message1);
        }

        private void rebroadcastClientMessage(IncomingMessage message)
        {
            OutgoingMessage message1 = new OutgoingMessage(message);
            foreach (long peerId in (IEnumerable<long>)Game1.otherFarmers.Keys)
            {
                if (peerId != message.FarmerID)
                    this.sendMessage(peerId, message1);
            }
        }

        private void setLobbyData(string key, string value)
        {
            foreach (Server server in this.servers)
                try
                {
                    server.setLobbyData(key, value);
                }
                catch(Exception err)
                {

                }
        }

        private bool unclaimedFarmhandsExist()
        {
            foreach (Cabin cabin in this.cabins())
            {
                if (cabin.farmhand.Value == null || cabin.farmhand.Value.userID.Value == "")
                    return true;
            }
            return false;
        }

        public void updateLobbyData()
        {
            this.setLobbyData("farmName", Game1.player.farmName.Value);
            this.setLobbyData("farmType", Convert.ToString(Game1.whichFarm));
            this.setLobbyData("date", Convert.ToString(new WorldDate(Game1.year, Game1.currentSeason, Game1.dayOfMonth).TotalDays));
            this.setLobbyData("farmhands", string.Join(",", Enumerable.Where<string>(Enumerable.Select<Farmer, string>(Game1.getAllFarmhands(), (Func<Farmer, string>)(farmhand => farmhand.userID.Value)), (Func<string, bool>)(user => user != ""))));
            this.setLobbyData("newFarmhands", Convert.ToString(Game1.options.enableFarmhandCreation && this.unclaimedFarmhandsExist()));
        }
    }
}

