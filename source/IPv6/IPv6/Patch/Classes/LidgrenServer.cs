/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sunsst/Stardew-Valley-IPv6
**
*************************************************/

// Stardew Valley, Version=1.6.8.24119, Culture=neutral, PublicKeyToken=null
// StardewValley.Network.LidgrenServer

#pragma warning disable CS8600, CS8601, CS8602, CS8603, CS8604, CS8618,

extern alias slnet;
using slnet::Lidgren.Network;

using StardewValley;
using StardewValley.Network;
using System.Net;

namespace IPv6.Patch.Classes;

public class LidgrenServer : HookableServer
{
    public const int defaultPort = 24642;

    public NetServer server;

    private HashSet<NetConnection> introductionsSent = new HashSet<NetConnection>();

    protected Bimap<long, NetConnection> peers = new Bimap<long, NetConnection>();

    public override int connectionsCount
    {
        get
        {
            if (server == null)
            {
                return 0;
            }
            return server.ConnectionsCount;
        }
    }

    private IGameServer _gameServer;

    public LidgrenServer(IGameServer gameServer)
        : base(gameServer)
    {
        this._gameServer = gameServer;
#if DEBUG
        MyPatch.LogInfo($"class {GetType().FullName} patched");
#endif
    }

    public override bool isConnectionActive(string connectionID)
    {
        foreach (NetConnection connection in ((NetPeer)server).Connections)
        {
            if (getConnectionId(connection) == connectionID && connection.Status == NetConnectionStatus.Connected)
            {
                return true;
            }
        }
        return false;
    }

    public override string getUserId(long farmerId)
    {
        if (!peers.ContainsLeft(farmerId))
        {
            return null;
        }
        return peers[farmerId].RemoteEndPoint.Address.ToString();
    }

    public override bool hasUserId(string userId)
    {
        foreach (NetConnection rightValue in peers.RightValues)
        {
            if (rightValue.RemoteEndPoint.Address.ToString().Equals(userId))
            {
                return true;
            }
        }
        return false;
    }

    public override string getUserName(long farmerId)
    {
        if (!peers.ContainsLeft(farmerId))
        {
            return null;
        }
        return peers[farmerId].RemoteEndPoint.Address.ToString();
    }

    public override float getPingToClient(long farmerId)
    {
        if (!peers.ContainsLeft(farmerId))
        {
            return -1f;
        }
        return peers[farmerId].AverageRoundtripTime / 2f * 1000f;
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
        return server != null;
    }

    public override void initialize()
    {
        MyPatch.log.Verbose("Starting LAN server");
        NetPeerConfiguration config = new NetPeerConfiguration("StardewValley");
        config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
        config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
        config.Port = 24642;
        config.ConnectionTimeout = 30f;
        config.PingInterval = 5f;
        config.MaximumConnections = MyPatch.multiplayer.playerLimit * 2;
        config.MaximumTransmissionUnit = 1200;


        switch (MyPatch.ChooseIPMode)
        {
            case MyPatch.IPMode.IPv4:
                MyPatch.GameLog("server enable IPv4");
                Game1.addHUDMessage(new("��������֧�� IPv4 �ͻ�������", HUDMessage.newQuest_type));
                break;
            case MyPatch.IPMode.IPv6:
                config.LocalAddress = IPAddress.IPv6Any;
                MyPatch.GameLog("server enable IPv6");
                Game1.addHUDMessage(new("��������֧�� IPv6 �ͻ�������", HUDMessage.newQuest_type));
                break;
            default:
                config.DualStack = true;
                config.LocalAddress = IPAddress.IPv6Any;
                MyPatch.GameLog("server enable IPv4 and IPv6"); 
                Game1.addHUDMessage(new("������֧�� IPv4 �� IPv6 �ͻ�������", HUDMessage.newQuest_type));
                break;
        }

        server = new NetServer(config);
        server.Start();
    }

    public override void stopServer()
    {
        MyPatch.log.Verbose("Stopping LAN server");
        server.Shutdown("Server shutting down...");
        server.FlushSendQueue();
        introductionsSent.Clear();
        peers.Clear();
    }

    public static bool IsLocal(string host_name_or_address)
    {
        if (string.IsNullOrEmpty(host_name_or_address))
        {
            return false;
        }
        try
        {
            IPAddress[] hostAddresses = Dns.GetHostAddresses(host_name_or_address);
            IPAddress[] local_ips = Dns.GetHostAddresses(Dns.GetHostName());
            return hostAddresses.Any((IPAddress host_ip) => IPAddress.IsLoopback(host_ip) || local_ips.Contains(host_ip));
        }
        catch
        {
            return false;
        }
    }

    public override void receiveMessages()
    {
        NetIncomingMessage inc;
        while ((inc = server.ReadMessage()) != null)
        {
            bandwidthLogger?.RecordBytesDown(inc.LengthBytes);
            switch (inc.MessageType)
            {
                case NetIncomingMessageType.DiscoveryRequest:
                    if ((Game1.options.ipConnectionsEnabled || _gameServer.IsLocalMultiplayerInitiatedServer()) && (!_gameServer.IsLocalMultiplayerInitiatedServer() || IsLocal(inc.SenderEndPoint.Address.ToString())) && !_gameServer.isUserBanned(inc.SenderEndPoint.Address.ToString()))
                    {
                        sendVersionInfo(inc);
                    }
                    break;
                case NetIncomingMessageType.ConnectionApproval:
                    if (Game1.options.ipConnectionsEnabled || _gameServer.IsLocalMultiplayerInitiatedServer())
                    {
                        inc.SenderConnection.Approve();
                    }
                    else
                    {
                        inc.SenderConnection.Deny();
                    }
                    break;
                case NetIncomingMessageType.Data:
                    parseDataMessageFromClient(inc);
                    break;
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.ErrorMessage:
                    {
                        string message = inc.ReadString();
                        MyPatch.log.Verbose(inc.MessageType.ToString() + ": " + message);
                        Game1.debugOutput = message;
                        break;
                    }
                case NetIncomingMessageType.StatusChanged:
                    statusChanged(inc);
                    break;
                default:
                    Game1.debugOutput = inc.ToString();
                    break;
            }
            server.Recycle(inc);
        }
        foreach (NetConnection conn in ((NetPeer)server).Connections)
        {
            if (conn.Status == NetConnectionStatus.Connected && !introductionsSent.Contains(conn))
            {
                if (!_gameServer.whenGameAvailable(delegate
                {
                    _gameServer.sendAvailableFarmhands("", getConnectionId(conn), delegate (OutgoingMessage msg)
                     {
                         sendMessage(conn, msg);
                     });
                }, () => Game1.gameMode != 6))
                {
                    MyPatch.log.Verbose("Postponing introduction message");
                    sendMessage(conn, new OutgoingMessage(11, Game1.player, "Strings\\UI:Client_WaitForHostLoad"));
                }
                introductionsSent.Add(conn);
            }
        }
        bandwidthLogger?.Update();
    }

    private void sendVersionInfo(NetIncomingMessage message)
    {
        NetOutgoingMessage response = server.CreateMessage();
        response.Write(Multiplayer.protocolVersion);
        response.Write("StardewValley");
        server.SendDiscoveryResponse(response, message.SenderEndPoint);
        bandwidthLogger?.RecordBytesUp(response.LengthBytes);
    }

    private void statusChanged(NetIncomingMessage message)
    {
        switch ((NetConnectionStatus)message.ReadByte())
        {
            case NetConnectionStatus.Connected:
                onConnect(getConnectionId(message.SenderConnection));
                break;
            case NetConnectionStatus.Disconnecting:
            case NetConnectionStatus.Disconnected:
                onDisconnect(getConnectionId(message.SenderConnection));
                if (peers.ContainsRight(message.SenderConnection))
                {
                    playerDisconnected(peers[message.SenderConnection]);
                }
                break;
        }
    }

    public override void kick(long disconnectee)
    {
        base.kick(disconnectee);
        if (peers.ContainsLeft(disconnectee))
        {
            peers[disconnectee].Disconnect(Multiplayer.kicked);
            server.FlushSendQueue();
            playerDisconnected(disconnectee);
        }
    }

    public override void playerDisconnected(long disconnectee)
    {
        base.playerDisconnected(disconnectee);
        introductionsSent.Remove(peers[disconnectee]);
        peers.RemoveLeft(disconnectee);
    }

    protected virtual void parseDataMessageFromClient(NetIncomingMessage dataMsg)
    {
        NetConnection peer = dataMsg.SenderConnection;
        IncomingMessage message = new IncomingMessage();
        try
        {
            using NetBufferReadStream stream = new NetBufferReadStream(dataMsg);
            while (dataMsg.LengthBits - dataMsg.Position >= 8)
            {
                LidgrenMessageUtils.ReadStreamToMessage(stream, message);
                base.OnProcessingMessage(message, delegate (OutgoingMessage outgoing)
                {
                    sendMessage(peer, outgoing);
                }, delegate
                {
                    if (peers.ContainsLeft(message.FarmerID) && peers[message.FarmerID] == peer)
                    {
                        _gameServer.processIncomingMessage(message);
                    }
                    else if (message.MessageType == 2)
                    {
                        NetFarmerRoot farmer = MyPatch.multiplayer.readFarmer(message.Reader);
                        _gameServer.checkFarmhandRequest("", getConnectionId(dataMsg.SenderConnection), farmer, delegate (OutgoingMessage msg)
                        {
                            sendMessage(peer, msg);
                        }, delegate
                        {
                            peers[farmer.Value.UniqueMultiplayerID] = peer;
                        });
                    }
                });
            }
        }
        finally
        {
            if (message != null)
            {
                ((IDisposable)message).Dispose();
            }
        }
    }

    public string getConnectionId(NetConnection connection)
    {
        return "L_" + connection.RemoteUniqueIdentifier;
    }

    public override void sendMessage(long peerId, OutgoingMessage message)
    {
        if (peers.ContainsLeft(peerId))
        {
            sendMessage(peers[peerId], message);
        }
    }

    protected virtual void sendMessage(NetConnection connection, OutgoingMessage message)
    {
        NetOutgoingMessage msg = server.CreateMessage();
        LidgrenMessageUtils.WriteMessage(message, msg);
        server.SendMessage(msg, connection, NetDeliveryMethod.ReliableOrdered);
        bandwidthLogger?.RecordBytesUp(msg.LengthBytes);
    }

    public override void setLobbyData(string key, string value)
    {
    }
}
