using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using static DeepWoodsMod.DeepWoodsSettings;
using static DeepWoodsMod.DeepWoodsGlobals;
using Newtonsoft.Json;

namespace DeepWoodsMod
{
    class Game1MultiplayerAccessProvider : Game1
    {
        private Game1MultiplayerAccessProvider() { }

        private class InterceptingMultiplayer : Multiplayer
        {
            private Multiplayer intercepted;

            public InterceptingMultiplayer(Multiplayer intercepted)
            {
                this.intercepted = intercepted;
            }

            public override int MaxPlayers
            {
                get
                {
                    return intercepted.MaxPlayers;
                }
            }

            public override IEnumerable<GameLocation> activeLocations() { return intercepted.activeLocations(); }
            public override void addPlayer(NetFarmerRoot f) { intercepted.addPlayer(f); }
            public override bool allowSyncDelay() { return intercepted.allowSyncDelay(); }
            public override void broadcastEvent(Event evt, GameLocation location, Vector2 positionBeforeEvent) { intercepted.broadcastEvent(evt, location, positionBeforeEvent); }
            public override void broadcastFarmerDeltas() { intercepted.broadcastFarmerDeltas(); }
            public override void broadcastLocationDelta(GameLocation loc) { intercepted.broadcastLocationDelta(loc); }
            public override void broadcastLocationDeltas() { intercepted.broadcastLocationDeltas(); }
            public override void broadcastPlayerIntroduction(NetFarmerRoot farmerRoot) { intercepted.broadcastPlayerIntroduction(farmerRoot); }
            public override void broadcastSprites(GameLocation location, List<TemporaryAnimatedSprite> sprites) { intercepted.broadcastSprites(location, sprites); }
            public override void broadcastSprites(GameLocation location, params TemporaryAnimatedSprite[] sprites) { intercepted.broadcastSprites(location, sprites); }
            public override void broadcastUserName(long farmerId, string userName) { intercepted.broadcastUserName(farmerId, userName); }
            public override void broadcastWorldStateDeltas() { intercepted.broadcastWorldStateDeltas(); }
            public override void clientRemotelyDisconnected() { intercepted.clientRemotelyDisconnected(); }
            public override void Disconnect() { intercepted.Disconnect(); }
            public override NetFarmerRoot farmerRoot(long id) { return intercepted.farmerRoot(id); }
            public override IEnumerable<NetFarmerRoot> farmerRoots() { return intercepted.farmerRoots(); }
            public override long getNewID() { return intercepted.getNewID(); }
            public override string getUserName(long id) { return intercepted.getUserName(id); }
            public override void globalChatInfoMessage(string messageKey, params string[] args) { intercepted.globalChatInfoMessage(messageKey, args); }
            public override Client InitClient(Client client) { return intercepted.InitClient(client); }
            public override Server InitServer(Server server) { return intercepted.InitServer(server); }
            public override int interpolationTicks() { return intercepted.interpolationTicks(); }
            public override void inviteAccepted() { intercepted.inviteAccepted(); }
            public override bool isActiveLocation(GameLocation location) { return intercepted.isActiveLocation(location); }
            public override bool isAlwaysActiveLocation(GameLocation location) { return intercepted.isAlwaysActiveLocation(location); }
            public override bool isClientBroadcastType(byte messageType) { return intercepted.isClientBroadcastType(messageType); }
            public override bool isDisconnecting(Farmer farmer) { return intercepted.isDisconnecting(farmer); }
            public override bool isDisconnecting(long uid) { return intercepted.isDisconnecting(uid); }
            public override NetRoot<GameLocation> locationRoot(GameLocation location) { return intercepted.locationRoot(location); }
            public override void parseServerToClientsMessage(string message) { intercepted.parseServerToClientsMessage(message); }
            public override void playerDisconnected(long id) { intercepted.playerDisconnected(id); }
            public override void processIncomingMessage(IncomingMessage msg) { InterceptProcessIncomingMessage(msg); }
            public override NetFarmerRoot readFarmer(BinaryReader reader) { return intercepted.readFarmer(reader); }
            public override void readObjectDelta<T>(BinaryReader reader, NetRoot<T> root) { intercepted.readObjectDelta(reader, root); }
            public override NetRoot<T> readObjectFull<T>(BinaryReader reader) { return intercepted.readObjectFull<T>(reader); }
            public override TemporaryAnimatedSprite[] readSprites(BinaryReader reader, GameLocation location) { return intercepted.readSprites(reader, location); }
            public override void receiveChatMessage(Farmer sourceFarmer, LocalizedContentManager.LanguageCode language, string message) { intercepted.receiveChatMessage(sourceFarmer, language, message); }
            public override void receivePlayerIntroduction(BinaryReader reader) { intercepted.receivePlayerIntroduction(reader); }
            public override void receiveWorldState(BinaryReader msg) { intercepted.receiveWorldState(msg); }
            public override void requestCharacterWarp(NPC character, GameLocation targetLocation, Vector2 position) { intercepted.requestCharacterWarp(character, targetLocation, position); }
            public override void saveFarmhands() { intercepted.saveFarmhands(); }
            public override void sendChatMessage(LocalizedContentManager.LanguageCode language, string message) { intercepted.sendChatMessage(language, message); }
            public override void sendFarmhand() { intercepted.sendFarmhand(); }
            public override void sendServerToClientsMessage(string message) { intercepted.sendServerToClientsMessage(message); }
            public override void StartServer() { intercepted.StartServer(); }
            public override void tickFarmerRoots() { intercepted.tickFarmerRoots(); }
            public override void tickLocationRoots() { intercepted.tickLocationRoots(); }
            public override void UpdateEarly() { intercepted.UpdateEarly(); }
            public override void UpdateLate(bool forceSync = false) { intercepted.UpdateLate(forceSync); }
            public override void writeObjectDelta<T>(BinaryWriter writer, NetRoot<T> root) { intercepted.writeObjectDelta<T>(writer, root); }
            public override byte[] writeObjectDeltaBytes<T>(NetRoot<T> root) { return intercepted.writeObjectDeltaBytes<T>(root); }
            public override void writeObjectFull<T>(BinaryWriter writer, NetRoot<T> root, long? peer) { intercepted.writeObjectFull<T>(writer, root, peer); }
            public override byte[] writeObjectFullBytes<T>(NetRoot<T> root, long? peer) { return intercepted.writeObjectFullBytes<T>(root, peer); }

            protected override void broadcastFarmerDelta(Farmer farmer, byte[] delta) { CallProtectedIntercepted("broadcastFarmerDelta", new object[] { farmer, delta }); }
            protected override void broadcastLocationBytes(GameLocation loc, byte messageType, byte[] bytes) { CallProtectedIntercepted("broadcastLocationBytes", new object[] { loc, messageType, bytes }); }
            protected override void broadcastLocationMessage(GameLocation loc, OutgoingMessage message) { CallProtectedIntercepted("broadcastLocationMessage", new object[] { loc, message }); }
            protected override void broadcastTeamDelta(byte[] delta) { CallProtectedIntercepted("broadcastTeamDelta", new object[] { delta }); }
            protected override BinaryWriter createWriter(Stream stream) { return (BinaryWriter)CallProtectedIntercepted("createWriter", new object[] { stream }); }
            protected override void readActiveLocation(IncomingMessage msg, bool forceCurrentLocation = false) { CallProtectedIntercepted("readActiveLocation", new object[] { msg, forceCurrentLocation }); }
            protected override GameLocation readLocation(BinaryReader reader) { return (GameLocation)CallProtectedIntercepted("readLocation", new object[] { reader }); }
            protected override LocationRequest readLocationRequest(BinaryReader reader) { return (LocationRequest)CallProtectedIntercepted("readLocationRequest", new object[] { reader }); }
            protected override NPC readNPC(BinaryReader reader) { return (NPC)CallProtectedIntercepted("readNPC", new object[] { reader }); }
            protected override void readWarp(BinaryReader reader, int tileX, int tileY, Action afterWarp) { CallProtectedIntercepted("readWarp", new object[] { reader, tileX, tileY, afterWarp }); }
            protected override void receiveChatInfoMessage(Farmer sourceFarmer, string messageKey, string[] args) { CallProtectedIntercepted("receiveChatInfoMessage", new object[] { sourceFarmer, messageKey, args }); }
            protected override void receiveFarmerGainExperience(IncomingMessage msg) { CallProtectedIntercepted("receiveFarmerGainExperience", new object[] { msg }); }
            protected override void receiveNewDaySync(IncomingMessage msg) { CallProtectedIntercepted("receiveNewDaySync", new object[] { msg }); }
            protected override void receiveTeamDelta(BinaryReader msg) { CallProtectedIntercepted("receiveTeamDelta", new object[] { msg }); }
            protected override void removeDisconnectedFarmers() { CallProtectedIntercepted("removeDisconnectedFarmers", new object[] { }); }
            protected override void saveFarmhand(NetFarmerRoot farmhand) { CallProtectedIntercepted("saveFarmhand", new object[] { farmhand }); }
            protected override void sendChatInfoMessage(string messageKey, params string[] args) { CallProtectedIntercepted("sendChatInfoMessage", new object[] { messageKey, args }); }
            protected override void updatePendingConnections() { CallProtectedIntercepted("updatePendingConnections", new object[] { }); }

            private object CallProtectedIntercepted(string methodName, object[] parameters)
            {
                return typeof(Multiplayer).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Invoke(intercepted, parameters);
            }

            private struct DeepWoodsWarpMessageData
            {
                public int Level { get; set; }
                public string Name { get; set; }
                public Vector2 EnterLocation { get; set; }
            }

            private DeepWoodsWarpMessageData ReadDeepWoodsWarpMessage(BinaryReader reader)
            {
                return new DeepWoodsWarpMessageData()
                {
                    Level = reader.ReadInt32(),
                    Name = reader.ReadString(),
                    EnterLocation = reader.ReadVector2()
                };
            }

            private void InterceptProcessIncomingMessage(IncomingMessage msg)
            {
                if (msg.MessageType == NETWORK_MESSAGE_DEEPWOODS)
                {
                    int deepwoodsMessageType = msg.Reader.ReadInt32();
                    int randId = Game1.random.Next();

                    ModEntry.Log("InterceptProcessIncomingMessage["+randId+ "], master id: " + Game1.MasterPlayer.UniqueMultiplayerID + ", local id: " + Game1.player.UniqueMultiplayerID + ", msg.FarmerID: " + msg.FarmerID + ", deepwoodsMessageType: " + deepwoodsMessageType, StardewModdingAPI.LogLevel.Debug);

                    Farmer who = Game1.getFarmer(msg.FarmerID);
                    if (who == null || who == Game1.player)
                    {
                        ModEntry.Log(" who is null or local!", StardewModdingAPI.LogLevel.Warn);
                        return;
                    }

                    if (deepwoodsMessageType == NETWORK_MESSAGE_DEEPWOODS_INIT)
                    {
                        ModEntry.Log(" [" + randId + "] deepwoodsMessageType == NETWORK_MESSAGE_DEEPWOODS_INIT", StardewModdingAPI.LogLevel.Debug);
                        if (Game1.IsMasterGame)
                        {
                            // Client requests settings and state, send it:
                            List<string> deepWoodsLevelNames = new List<string>();
                            foreach (var location in Game1.locations)
                                if (location is DeepWoods deepWoods)
                                    deepWoodsLevelNames.Add(deepWoods.Name);

                            object[] data = new object[deepWoodsLevelNames.Count + 4];
                            data[0] = NETWORK_MESSAGE_DEEPWOODS_INIT;
                            data[1] = JsonConvert.SerializeObject(Settings);
                            data[2] = JsonConvert.SerializeObject(DeepWoodsState);
                            data[3] = (int)deepWoodsLevelNames.Count;
                            for (int i = 0; i < deepWoodsLevelNames.Count; i++)
                            {
                                data[i + 4] = deepWoodsLevelNames[i];
                            }

                            ModEntry.Log(" [" + randId + "] Client requests settings and state, deepWoodsLevelNames.Count: " + deepWoodsLevelNames.Count + ", deepWoodsLevelNames: " + String.Join(", ", deepWoodsLevelNames.ToArray()), StardewModdingAPI.LogLevel.Debug);
                            who.queueMessage(NETWORK_MESSAGE_DEEPWOODS, Game1.MasterPlayer, data);
                        }
                        else
                        {
                            // Server sent us settings and state!
                            Settings = JsonConvert.DeserializeObject<DeepWoodsSettings>(msg.Reader.ReadString());
                            DeepWoodsState = JsonConvert.DeserializeObject<DeepWoodsStateData>(msg.Reader.ReadString());
                            int numDeepWoodsLevelNames = msg.Reader.ReadInt32();
                            List<string> deepWoodsLevelNames = new List<string>();
                            for (int i = 0; i < numDeepWoodsLevelNames; i++)
                            {
                                deepWoodsLevelNames.Add(msg.Reader.ReadString());
                            }
                            ModEntry.Log(" [" + randId + "] Server sent us settings and state, deepWoodsLevelNames.Count: " + deepWoodsLevelNames.Count + ", deepWoodsLevelNames: " + String.Join(", ", deepWoodsLevelNames.ToArray()), StardewModdingAPI.LogLevel.Debug);
                            ModEntry.DeepWoodsInitServerAnswerReceived(deepWoodsLevelNames);
                        }
                    }
                    else if (deepwoodsMessageType == NETWORK_MESSAGE_DEEPWOODS_WARP)
                    {
                        ModEntry.Log(" [" + randId + "] deepwoodsMessageType == NETWORK_MESSAGE_DEEPWOODS_WARP", StardewModdingAPI.LogLevel.Debug);
                        DeepWoodsWarpMessageData data = ReadDeepWoodsWarpMessage(msg.Reader);
                        if (Game1.IsMasterGame)
                        {
                            // Client requests that we load and activate a specific DeepWoods level they want to warp into.
                            DeepWoods deepWoods = DeepWoodsManager.AddDeepWoodsFromObelisk(data.Level);
                            // Send message to client telling them we have the level ready.
                            ModEntry.Log(" [" + randId + "] Client requests that we load and activate a specific DeepWoods level they want to warp into: data.Level:" + data.Level, StardewModdingAPI.LogLevel.Debug);
                            who.queueMessage(NETWORK_MESSAGE_DEEPWOODS, Game1.MasterPlayer, new object[] { NETWORK_MESSAGE_DEEPWOODS_WARP, deepWoods.level.Value, deepWoods.Name, new Vector2(deepWoods.enterLocation.Value.X, deepWoods.enterLocation.Value.Y) });
                        }
                        else
                        {
                            // Server informs us that we can warp now!
                            ModEntry.Log(" [" + randId + "] Server informs us that we can warp now: data.Level:" + data.Level + ", data.Name:" + data.Name, StardewModdingAPI.LogLevel.Debug);
                            DeepWoodsManager.AddBlankDeepWoodsToGameLocations(data.Name);
                            DeepWoodsManager.WarpFarmerIntoDeepWoodsFromServerObelisk(data.Name, data.EnterLocation);
                        }
                    }
                    else if (deepwoodsMessageType == NETWORK_MESSAGE_DEEPWOODS_LEVEL)
                    {
                        ModEntry.Log(" [" + randId + "] deepwoodsMessageType == NETWORK_MESSAGE_DEEPWOODS_LEVEL", StardewModdingAPI.LogLevel.Debug);
                        if (!Game1.IsMasterGame && who == Game1.MasterPlayer)
                        {
                            DeepWoodsState.LowestLevelReached = msg.Reader.ReadInt32();
                            ModEntry.Log(" [" + randId + "] DeepWoodsState.LowestLevelReached: " + DeepWoodsState.LowestLevelReached, StardewModdingAPI.LogLevel.Debug);
                        }
                    }
                    else if (deepwoodsMessageType == NETWORK_MESSAGE_RCVD_STARDROP_FROM_UNICORN)
                    {
                        ModEntry.Log(" [" + randId + "] deepwoodsMessageType == NETWORK_MESSAGE_RCVD_STARDROP_FROM_UNICORN", StardewModdingAPI.LogLevel.Debug);
                        if (Game1.IsMasterGame)
                        {
                            DeepWoodsState.PlayersWhoGotStardropFromUnicorn.Add(who.UniqueMultiplayerID);
                        }
                    }
                    else if (deepwoodsMessageType == NETWORK_MESSAGE_DEEPWOODS_ADDREMOVE)
                    {
                        ModEntry.Log(" [" + randId + "] deepwoodsMessageType == NETWORK_MESSAGE_DEEPWOODS_ADDREMOVE", StardewModdingAPI.LogLevel.Debug);
                        if (!Game1.IsMasterGame)
                        {
                            bool added = msg.Reader.ReadByte() != 0;
                            string name = msg.Reader.ReadString();
                            ModEntry.Log(" [" + randId + "] added: " + added + ", name: " + name, StardewModdingAPI.LogLevel.Debug);
                            if (added)
                            {
                                DeepWoodsManager.AddBlankDeepWoodsToGameLocations(name);
                            }
                            else
                            {
                                DeepWoodsManager.RemoveDeepWoodsFromGameLocations(name);
                            }
                        }
                    }
                    else
                    {
                        ModEntry.Log(" [" + randId + "] unknown deepwoodsMessageType: " + deepwoodsMessageType + "!", StardewModdingAPI.LogLevel.Warn);
                    }
                }
                else
                {
                    intercepted.processIncomingMessage(msg);
                }
            }
        }

        public static Multiplayer GetMultiplayer()
        {
            if (!(Game1.multiplayer is InterceptingMultiplayer))
            {
                InterceptMultiplayer();
            }
            return Game1.multiplayer;
        }

        public static void InterceptMultiplayer()
        {
            Game1.multiplayer = new InterceptingMultiplayer(Game1.multiplayer);
        }
    }
}
