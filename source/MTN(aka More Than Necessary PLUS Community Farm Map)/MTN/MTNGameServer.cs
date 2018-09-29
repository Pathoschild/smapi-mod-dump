using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Minigames;
using StardewValley.Network;

namespace MTN {

    /// <summary>
    /// Reimplementation of GameServer.cs from Vanilla SDV. Done to emit Game1.whichFarm early and to enable
    /// more maps to be sent on server introduction. Also enables cabins to exist elsewhere and be registered
    /// in Multiplayer.
    /// </summary>
    public class MTNGameServer : IGameServer {
        protected List<Server> servers = new List<Server>();
        private List<Action> pendingGameAvailableActions = new List<Action>();

        public MTNGameServer() {
            servers.Add(Memory.multiplayer.InitServer(new LidgrenServer(this)));
            if (Program.sdk.Networking != null) {
                servers.Add(Program.sdk.Networking.CreateServer(this));
            }
        }

        public int connectionsCount {
            get {
                return servers.Sum((Server s) => s.connectionsCount);
            }
        }

        public string getInviteCode() {
            foreach (Server server in servers) {
                string code = server.getInviteCode();
                if (code != null) {
                    return code;
                }
            }
            return null;
        }

        public string getUserName(long farmerId) {
            foreach (Server server in servers) {
                if (server.getUserName(farmerId) != null) {
                    return server.getUserName(farmerId);
                }
            }
            return null;
        }

        protected void initialize() {
            foreach (Server server in servers) {
                server.initialize();
            }
            updateLobbyData();
        }

        public void setPrivacy(ServerPrivacy privacy) {
            foreach (Server server in servers) {
                server.setPrivacy(privacy);
            }
        }

        public void stopServer() {
            foreach (Server server in servers) {
                server.stopServer();
            }
        }

        public void receiveMessages() {
            foreach (Server server in servers) {
                server.receiveMessages();
            }
            if (isGameAvailable()) {
                foreach (Action action in pendingGameAvailableActions) {
                    action();
                }
                pendingGameAvailableActions.Clear();
            }
        }

        public void sendMessage(long peerId, OutgoingMessage message) {
            foreach (Server server in servers) {
                server.sendMessage(peerId, message);
            }
        }

        public bool canAcceptIPConnections() {
            return (from s in servers
                    select s.canAcceptIPConnections()).Aggregate(false, (bool a, bool b) => a || b);
        }

        public bool canOfferInvite() {
            return (from s in servers
                    select s.canOfferInvite()).Aggregate(false, (bool a, bool b) => a || b);
        }

        public void offerInvite() {
            foreach (Server s in servers) {
                if (s.canOfferInvite()) {
                    s.offerInvite();
                }
            }
        }

        public bool connected() {
            using (List<Server>.Enumerator enumerator = servers.GetEnumerator()) {
                while (enumerator.MoveNext()) {
                    if (!enumerator.Current.connected()) {
                        return false;
                    }
                }
            }
            return true;
        }

        public void sendMessage(long peerId, byte messageType, Farmer sourceFarmer, params object[] data) {
            sendMessage(peerId, new OutgoingMessage(messageType, sourceFarmer, data));
        }

        public void sendMessages() {
            foreach (Farmer farmer in Game1.otherFarmers.Values) {
                foreach (OutgoingMessage message in farmer.messageQueue) {
                    sendMessage(farmer.UniqueMultiplayerID, message);
                }
                farmer.messageQueue.Clear();
            }
        }

        public void startServer() {
            Memory.instance.Monitor.Log("Starting server. Protocol version: " + Memory.multiplayer.protocolVersion);
            initialize();
            Game1.serverHost = new NetFarmerRoot();
            Game1.serverHost.Value = Game1.player;
            Game1.serverHost.MarkClean();
            Game1.serverHost.Clock.InterpolationTicks = Memory.multiplayer.defaultInterpolationTicks;
            if (Game1.netWorldState.Value == null) {
                Game1.netWorldState = new NetRoot<IWorldState>(new NetWorldState());
            }
            Game1.netWorldState.Clock.InterpolationTicks = 0;
            Game1.netWorldState.Value.UpdateFromGame1();
        }

        /// <summary>
        /// Modified version from the original. Emits Game1.whichFarm early, and sends all maps
        /// that are of BuildableGameLocation, rather than just the base farm.
        /// </summary>
        /// <param name="peer"></param>
        public void sendServerIntroduction(long peer) {
            //Sends the base farm and farmhouse.
            sendLocation(peer, Game1.getFarm());
            sendLocation(peer, Game1.getLocationFromName("FarmHouse"));

            //Send Game1.whichFarm early. To load additional maps you fool.
            sendMessage(peer, 30, Game1.serverHost.Value, new object[] { Game1.whichFarm });

            //Custom maps
            //Send the data of the custom maps, the ones that are always active (Farms)
            for (int i = 56; i < Game1.locations.Count; i++) {
                if (Game1.locations[i] is BuildableGameLocation) {
                    sendLocation(peer, Game1.locations[i]);
                }
            }

            //Send the state of the world, the host player, and the other farmhands.
            sendMessage(peer, new OutgoingMessage(1, Game1.serverHost.Value, new object[]
            {
                Memory.multiplayer.writeObjectFullBytes<Farmer>(Game1.serverHost, new long?(peer)),
                Memory.multiplayer.writeObjectFullBytes<FarmerTeam>(Game1.player.teamRoot, new long?(peer)),
                Memory.multiplayer.writeObjectFullBytes<IWorldState>(Game1.netWorldState, new long?(peer))
            }));

            foreach (KeyValuePair<long, NetRoot<Farmer>> r in Game1.otherFarmers.Roots) {
                if (r.Key != Game1.player.UniqueMultiplayerID && r.Key != peer) {
                    sendMessage(peer, new OutgoingMessage(2, r.Value.Value, new object[]
                    {
                        getUserName(r.Value.Value.UniqueMultiplayerID),
                        Memory.multiplayer.writeObjectFullBytes<Farmer>(r.Value, new long?(peer))
                    }));
                }
            }
        }

        public void playerDisconnected(long disconnectee) {
            Farmer disconnectedFarmer = null;
            Game1.otherFarmers.TryGetValue(disconnectee, out disconnectedFarmer);
            Memory.multiplayer.playerDisconnected(disconnectee);
            if (disconnectedFarmer != null) {
                OutgoingMessage message = new OutgoingMessage(19, disconnectedFarmer, new object[0]);
                foreach (long peer in Game1.otherFarmers.Keys) {
                    if (peer != disconnectee) {
                        sendMessage(peer, message);
                    }
                }
            }
        }

        public bool isGameAvailable() {
            bool inIntro = Game1.currentMinigame is Intro || Game1.Date.DayOfMonth == 0;
            bool isWedding = Game1.CurrentEvent != null && Game1.CurrentEvent.isWedding;
            bool isSleeping = Game1.newDaySync != null && !Game1.newDaySync.hasFinished();
            bool isDemolishing = Game1.player.team.buildingLock.IsLocked();
            return !Game1.isFestival() && !isWedding && !inIntro && !isSleeping && !isDemolishing;
        }

        public bool whenGameAvailable(Action action) {
            if (isGameAvailable()) {
                action();
                return true;
            }
            pendingGameAvailableActions.Add(action);
            return false;
        }

        private void rejectFarmhandRequest(string userID, NetFarmerRoot farmer, Action<OutgoingMessage> sendMessage) {
            sendAvailableFarmhands(userID, sendMessage);
            Memory.instance.Monitor.Log("Rejected request for farmhand " + ((farmer.Value != null) ? farmer.Value.UniqueMultiplayerID.ToString() : "???"));
        }

        /// <summary>
        /// Modified from the original. Now scans each map, checking to see if its a BuildableGameLocation
        /// and if any of the buildings are of type Cabin. This is the routine used when "registering"
        /// what cabins said farmhand can use.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Cabin> cabins() {
            if (Game1.getFarm() == null || Game1.locations == null) {
                yield break;
            }
            foreach (GameLocation location in Game1.locations) {
                if (location is BuildableGameLocation) {
                    foreach (Building building in (location as BuildableGameLocation).buildings) {
                        if (building.daysOfConstructionLeft.Value <= 0 && building.indoors.Value is Cabin) {
                            Cabin cabin = building.indoors.Value as Cabin;
                            yield return cabin;
                        }
                    }
                }
            }
            yield break;
        }

        private bool authCheck(string userID, Farmer farmhand) {
            return (Game1.options.enableFarmhandCreation || farmhand.isCustomized.Value) && (userID == "" || farmhand.userID.Value == "" || farmhand.userID.Value == userID);
        }

        private Cabin findCabin(Farmer farmhand) {
            foreach (Cabin cabin in cabins()) {
                if (cabin.getFarmhand().Value.UniqueMultiplayerID == farmhand.UniqueMultiplayerID) {
                    return cabin;
                }
            }
            return null;
        }

        private Farmer findOriginalFarmhand(Farmer farmhand) {
            Cabin cabin = findCabin(farmhand);
            if (cabin == null) {
                return null;
            }
            return cabin.getFarmhand().Value;
        }

        public void checkFarmhandRequest(string userID, NetFarmerRoot farmer, Action<OutgoingMessage> sendMessage, Action approve) {
            if (farmer.Value == null) {
                rejectFarmhandRequest(userID, farmer, sendMessage);
                return;
            }
            long id = farmer.Value.UniqueMultiplayerID;
            Action check = delegate () {
                Farmer originalFarmhand = this.findOriginalFarmhand(farmer.Value);
                if (originalFarmhand == null) {
                    Memory.instance.Monitor.Log("Rejected request for farmhand " + id + ": doesn't exist");
                    rejectFarmhandRequest(userID, farmer, sendMessage);
                    return;
                }
                if (!authCheck(userID, originalFarmhand)) {
                    Memory.instance.Monitor.Log("Rejected request for farmhand " + id + ": authorization failure");
                    rejectFarmhandRequest(userID, farmer, sendMessage);
                    return;
                }
                if ((Game1.otherFarmers.ContainsKey(id) && !Memory.multiplayer.isDisconnecting(id)) || Game1.serverHost.Value.UniqueMultiplayerID == id) {
                    Memory.instance.Monitor.Log("Rejected request for farmhand " + id + ": already in use");
                    rejectFarmhandRequest(userID, farmer, sendMessage);
                    return;
                }
                if (findCabin(farmer.Value).isInventoryOpen()) {
                    Memory.instance.Monitor.Log("Rejected request for farmhand " + id + ": inventory in use");
                    rejectFarmhandRequest(userID, farmer, sendMessage);
                    return;
                }
                Memory.instance.Monitor.Log("Approved request for farmhand " + id);
                approve();
                Memory.multiplayer.addPlayer(farmer);
                Memory.multiplayer.broadcastPlayerIntroduction(farmer);
                sendServerIntroduction(id);
                updateLobbyData();
            };
            if (!whenGameAvailable(check)) {
                Memory.instance.Monitor.Log("Postponing request for farmhand " + id);
                sendMessage(new OutgoingMessage(11, Game1.player, new object[]
                {
                    "Strings\\UI:Client_WaitForHostAvailability"
                }));
            }
        }

        public void sendAvailableFarmhands(string userID, Action<OutgoingMessage> sendMessage) {
            List<NetRef<Farmer>> availableFarmhands = new List<NetRef<Farmer>>();
            Game1.getFarm();
            foreach (Cabin cabin in cabins()) {
                NetRef<Farmer> farmhand = cabin.getFarmhand();
                if ((!farmhand.Value.isActive() || Memory.multiplayer.isDisconnecting(farmhand.Value.UniqueMultiplayerID)) && this.authCheck(userID, farmhand.Value) && !cabin.isInventoryOpen()) {
                    availableFarmhands.Add(farmhand);
                }
            }
            using (MemoryStream stream = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(stream)) {
                    writer.Write(Game1.year);
                    writer.Write(Utility.getSeasonNumber(Game1.currentSeason));
                    writer.Write(Game1.dayOfMonth);
                    writer.Write((byte)availableFarmhands.Count);
                    foreach (NetRef<Farmer> farmhand2 in availableFarmhands) {
                        try {
                            farmhand2.Serializer = SaveGame.farmerSerializer;
                            farmhand2.WriteFull(writer);
                        } finally {
                            farmhand2.Serializer = null;
                        }
                    }
                    stream.Seek(0L, SeekOrigin.Begin);
                    sendMessage(new OutgoingMessage(9, Game1.player, new object[]
                    {
                        stream.ToArray()
                    }));
                }
            }
        }

        private void sendLocation(long peer, GameLocation location) {
            sendMessage(peer, 3, Game1.serverHost.Value, new object[]
            {
                Memory.multiplayer.writeObjectFullBytes<GameLocation>(Memory.multiplayer.locationRoot(location), new long?(peer))
            });
        }

        private void warpFarmer(Farmer farmer, short x, short y, string name, bool isStructure) {
            GameLocation location = Game1.getLocationFromName(name, isStructure);
            location.hostSetup();
            farmer.currentLocation = location;
            farmer.Position = new Vector2((float)(x * 64), (float)((int)(y * 64) - (farmer.Sprite.getHeight() - 32) + 16));
            sendLocation(farmer.UniqueMultiplayerID, location);
        }

        public void processIncomingMessage(IncomingMessage message) {
            byte messageType = message.MessageType;

            if (messageType != 2) {
                if (messageType == 5) {
                    warpFarmer(message.SourceFarmer, message.Reader.ReadInt16(), message.Reader.ReadInt16(), message.Reader.ReadString(), message.Reader.ReadByte() == 1);
                } else {
                    Memory.multiplayer.processIncomingMessage(message);
                }
            } else {
                message.Reader.ReadString();
                Memory.multiplayer.processIncomingMessage(message);
            }
            if (Memory.multiplayer.isClientBroadcastType(message.MessageType)) {
                rebroadcastClientMessage(message);
            }
        }

        private void rebroadcastClientMessage(IncomingMessage message) {
            OutgoingMessage outMessage = new OutgoingMessage(message);
            foreach (long peer in Game1.otherFarmers.Keys) {
                if (peer != message.FarmerID) {
                    sendMessage(peer, outMessage);
                }
            }
        }

        private void setLobbyData(string key, string value) {
            foreach (Server server in servers) {
                server.setLobbyData(key, value);
            }
        }

        private bool unclaimedFarmhandsExist() {
            foreach (Cabin cabin in cabins()) {
                if (cabin.farmhand.Value == null) {
                    return true;
                }
                if (cabin.farmhand.Value.userID.Value == "") {
                    return true;
                }
            }
            return false;
        }

        public void updateLobbyData() {
            setLobbyData("farmName", Game1.player.farmName.Value);
            setLobbyData("farmType", Convert.ToString(Game1.whichFarm));
            WorldDate date = new WorldDate(Game1.year, Game1.currentSeason, Game1.dayOfMonth);
            setLobbyData("date", Convert.ToString(date.TotalDays));
            IEnumerable<string> farmhandUserIds = from farmhand in Game1.getAllFarmhands()
                                                  select farmhand.userID.Value;
            setLobbyData("farmhands", string.Join(",", from user in farmhandUserIds
                                                       where user != ""
                                                       select user));
            setLobbyData("newFarmhands", Convert.ToString(Game1.options.enableFarmhandCreation && this.unclaimedFarmhandsExist()));
        }
    }
}
