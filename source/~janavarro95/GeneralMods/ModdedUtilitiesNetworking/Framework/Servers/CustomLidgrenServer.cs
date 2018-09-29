using Lidgren.Network;
using ModdedUtilitiesNetworking.Framework.Messages;
using ModdedUtilitiesNetworking.Framework.Network;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModdedUtilitiesNetworking.Framework.Servers
{
    class CustomLidgrenServer : Server
    {
        ///Save this for later

        protected override void playerDisconnected(long disconnectee)
        {
            this.gameServer.playerDisconnected(disconnectee);
            
              this.introductionsSent.Remove(this.peers[disconnectee]);

            this.peers.RemoveLeft(disconnectee);
        }

        protected void sendMessage(NetConnection connection, OutgoingMessage message)
        {
            NetOutgoingMessage message1 = this.server.CreateMessage();
            using (NetBufferWriteStream bufferWriteStream = new NetBufferWriteStream((NetBuffer)message1))
            {
                using (BinaryWriter writer = new BinaryWriter((Stream)bufferWriteStream)) {
                    if (message.MessageType < 20)
                    {
                        message.Write(writer);
                    }
                    else
                    {
                        OutgoingMessageBase.WriteFromMessage(message, writer);
                    }
                }
            }
            int num = (int)this.server.SendMessage(message1, connection, NetDeliveryMethod.ReliableOrdered);
        }
        


        private HashSet<NetConnection> introductionsSent = new HashSet<NetConnection>();
        private Bimap<long, NetConnection> peers = new Bimap<long, NetConnection>();
        public const int defaultPort = 24642;
        private NetServer server;

        public override int connectionsCount
        {
            get
            {
                if (this.server == null)
                    return 0;
                return this.server.ConnectionsCount;
            }
        }

        public CustomLidgrenServer(IGameServer gameServer)
          : base(gameServer)
        {
        }

        public override string getUserName(long farmerId)
        {
            if (!this.peers.ContainsLeft(farmerId))
                return (string)null;
            return this.peers[farmerId].RemoteEndPoint.Address.ToString();
        }

        public override void setPrivacy(ServerPrivacy privacy)
        {
        }

        public override bool canAcceptIPConnections()
        {
            return true;
        }

        public override bool connected()
        {
            return this.server != null;
        }

        public override void initialize()
        {
            Console.WriteLine("Starting LAN server");
            NetPeerConfiguration config = new NetPeerConfiguration("StardewValley");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.Port = 24642;
            config.ConnectionTimeout = 30f;
            config.PingInterval = 5f;
            config.MaximumConnections = ModCore.multiplayer.playerLimit * 2;
            config.MaximumTransmissionUnit = 1200;
            this.server = new NetServer(config);
            this.server.Start();
        }

        public override void stopServer()
        {
            Console.WriteLine("Stopping LAN server");
            this.server.Shutdown("Server shutting down...");
            this.server.FlushSendQueue();
            this.introductionsSent.Clear();
            this.peers.Clear();
        }

        public override void receiveMessages()
        {
            NetIncomingMessage netIncomingMessage;
            while ((netIncomingMessage = this.server.ReadMessage()) != null)
            {
                switch (netIncomingMessage.MessageType)
                {
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.DebugMessage:
                        string str = netIncomingMessage.ReadString();
                        Console.WriteLine("{0}: {1}", (object)netIncomingMessage.MessageType, (object)str);
                        Game1.debugOutput = str;
                        break;
                    case NetIncomingMessageType.DiscoveryRequest:
                        if (Game1.options.ipConnectionsEnabled)
                        {
                            this.sendVersionInfo(netIncomingMessage);
                            break;
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        this.statusChanged(netIncomingMessage);
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                        if (Game1.options.ipConnectionsEnabled)
                        {
                            netIncomingMessage.SenderConnection.Approve();
                            break;
                        }
                        netIncomingMessage.SenderConnection.Deny();
                        break;
                    case NetIncomingMessageType.Data:
                        this.parseDataMessageFromClient(netIncomingMessage);
                        break;
                    default:
                        Game1.debugOutput = netIncomingMessage.ToString();
                        break;
                }
                this.server.Recycle(netIncomingMessage);
            }
            foreach (NetConnection netConnection in this.server.Connections)
            {
                NetConnection conn = netConnection;
                if (conn.Status == NetConnectionStatus.Connected && !this.introductionsSent.Contains(conn))
                {
                    this.gameServer.sendAvailableFarmhands("", (Action<OutgoingMessage>)(msg => this.sendMessage(conn, msg)));
                    this.introductionsSent.Add(conn);
                }
            }
        }

        private void sendVersionInfo(NetIncomingMessage message)
        {
            NetOutgoingMessage message1 = this.server.CreateMessage();
            message1.Write(ModCore.multiplayer.protocolVersion);
            message1.Write("StardewValley");
            this.server.SendDiscoveryResponse(message1, message.SenderEndPoint);
        }

        private void statusChanged(NetIncomingMessage message)
        {
            switch ((NetConnectionStatus)message.ReadByte())
            {
                case NetConnectionStatus.Disconnected:
                case NetConnectionStatus.Disconnecting:
                    if (!this.peers.ContainsRight(message.SenderConnection))
                        break;
                    this.playerDisconnected(this.peers[message.SenderConnection]);
                    break;
            }
        }



        private void parseDataMessageFromClient(NetIncomingMessage dataMsg)
        {
            NetConnection peer = dataMsg.SenderConnection;
            using (IncomingMessage message = new IncomingMessage())
            {
                using (NetBufferReadStream bufferReadStream = new NetBufferReadStream((NetBuffer)dataMsg))
                {
                    using (BinaryReader reader = new BinaryReader((Stream)bufferReadStream))
                    {
                        while ((long)dataMsg.LengthBits - dataMsg.Position >= 8L)
                        {
                            message.Read(reader);
                            if (this.peers.ContainsLeft(message.FarmerID) && this.peers[message.FarmerID] == peer)
                                this.gameServer.processIncomingMessage(message);
                            else if ((int)message.MessageType == 2)
                            {
                                NetFarmerRoot farmer = ModCore.multiplayer.readFarmer(message.Reader);
                                this.gameServer.checkFarmhandRequest("", farmer, (msg => this.sendMessage(peer, msg)), (Action)(() => this.peers[farmer.Value.UniqueMultiplayerID] = peer));
                            }
                        }
                    }
                }
            }
        }

        public override void sendMessage(long peerId, OutgoingMessage message)
        {
            if (!this.peers.ContainsLeft(peerId))
                return;
            this.sendMessage(this.peers[peerId], message);
        }



        public override void setLobbyData(string key, string value)
        {
        }
    }
}
