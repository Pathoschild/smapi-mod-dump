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
// StardewValley.Network.LidgrenClient

#pragma warning disable CS8600, CS8602, CS8618

extern alias slnet;
using slnet::Lidgren.Network;
using StardewValley;
using StardewValley.Network;
using System.Net;
using System.Text.RegularExpressions;

namespace IPv6.Patch.Classes;

public class LidgrenClient : HookableClient
{
    public string address;

    public NetClient client;

    private bool serverDiscovered;

    private int maxRetryAttempts = 0;

    private int retryMs = 10000;

    private double lastAttemptMs;

    private int retryAttempts;

    private float lastLatencyMs;

    public LidgrenClient(string address)
    {
        this.address = address;
#if DEBUG
        MyPatch.LogInfo($"class {GetType().FullName} patched");
#endif
    }

    public override string getUserID()
    {
        return "";
    }

    public override float GetPingToHost()
    {
        return lastLatencyMs / 2f;
    }

    protected override string getHostUserName()
    {
        return client.ServerConnection.RemoteEndPoint.Address.ToString();
    }

    protected override void connectImpl()
    {
        NetPeerConfiguration config = new NetPeerConfiguration("StardewValley");
        config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
        config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
        config.ConnectionTimeout = 30f;
        config.PingInterval = 5f;
        config.MaximumTransmissionUnit = 1200;

        if (!IPEndPoint.TryParse(address, out var addr))
        {
            var res = Regex.Match(address, @"^(.+?)(?:\:([0-9]+))?$");
            if (res.Success)
            {
                try
                {
                    addr = NetUtility.Resolve(res.Groups[1].Value,
                    res.Groups[2].Value != "" ? int.Parse(res.Groups[2].Value) : 0);
                }
                catch { }
                if (addr != null)
                {
                    MyPatch.GameLog($"{address} => {addr}");
                    address = addr.ToString();
                }
            }
        }
        if (addr != null && addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            config.LocalAddress = IPAddress.IPv6Any;
            MyPatch.GameLog($"client enable IPv6");
        }
        else
        {
            MyPatch.GameLog($"client enable IPv4");
        }

        client = new NetClient(config);
        client.Start();
        attemptConnection();
    }

    private void attemptConnection()
    {
        int port = 24642;

        if (IPEndPoint.TryParse(address, out var addr))
        {
            if (addr.Port > 0)
                port = addr.Port;
            address = addr.Address.ToString();
        }
        MyPatch.GameLog($"client target address {address} port {port}");

        client.DiscoverKnownPeer(address, port);
        lastAttemptMs = DateTime.UtcNow.TimeOfDay.TotalMilliseconds;
    }

    public override void disconnect(bool neatly = true)
    {
        if (client == null)
        {
            return;
        }
        if (client.ConnectionStatus != NetConnectionStatus.Disconnected && client.ConnectionStatus != NetConnectionStatus.Disconnecting)
        {
            if (neatly)
            {
                sendMessage(new OutgoingMessage(19, Game1.player));
            }
            client.FlushSendQueue();
            client.Disconnect("");
            client.FlushSendQueue();
        }
        connectionMessage = null;
    }

    protected virtual bool validateProtocol(string version)
    {
        return version == Multiplayer.protocolVersion;
    }

    protected override void receiveMessagesImpl()
    {
        if (client != null && !serverDiscovered && DateTime.UtcNow.TimeOfDay.TotalMilliseconds >= lastAttemptMs + (double)retryMs && retryAttempts < maxRetryAttempts)
        {
            attemptConnection();
            retryAttempts++;
        }
        NetIncomingMessage inc;
        while ((inc = client.ReadMessage()) != null)
        {
            switch (inc.MessageType)
            {
                case NetIncomingMessageType.ConnectionLatencyUpdated:
                    readLatency(inc);
                    break;
                case NetIncomingMessageType.DiscoveryResponse:
                    if (!serverDiscovered)
                    {
                        MyPatch.log.Verbose("Found server at " + inc.SenderEndPoint);
                        string protocolVersion = inc.ReadString();
                        if (validateProtocol(protocolVersion))
                        {
                            serverName = inc.ReadString();
                            receiveHandshake(inc);
                            serverDiscovered = true;
                            break;
                        }
                        MyPatch.log.Warn($"Failed to connect. The server's protocol ({protocolVersion}) does not match our own ({Multiplayer.protocolVersion}).");
                        connectionMessage = Game1.content.LoadString("Strings\\UI:CoopMenu_FailedProtocolVersion");
                        client.Disconnect("");
                    }
                    break;
                case NetIncomingMessageType.Data:
                    parseDataMessageFromServer(inc);
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
            }
        }
    }

    private void readLatency(NetIncomingMessage msg)
    {
        lastLatencyMs = msg.ReadFloat() * 1000f;
    }

    private void receiveHandshake(NetIncomingMessage msg)
    {
        client.Connect(msg.SenderEndPoint.Address.ToString(), msg.SenderEndPoint.Port);
    }

    private void statusChanged(NetIncomingMessage message)
    {
        NetConnectionStatus status = (NetConnectionStatus)message.ReadByte();
        if (status == NetConnectionStatus.Disconnected || status == NetConnectionStatus.Disconnecting)
        {
            string byeMessage = message.ReadString();
            clientRemotelyDisconnected(status, byeMessage);
        }
    }

    private void clientRemotelyDisconnected(NetConnectionStatus status, string message)
    {
        timedOut = true;
        if (status == NetConnectionStatus.Disconnected)
        {
            if (message == Multiplayer.kicked)
            {
                pendingDisconnect = Multiplayer.DisconnectType.Kicked;
            }
            else
            {
                pendingDisconnect = Multiplayer.DisconnectType.LidgrenTimeout;
            }
        }
        else
        {
            pendingDisconnect = Multiplayer.DisconnectType.LidgrenDisconnect_Unknown;
        }
    }

    protected virtual void sendMessageImpl(OutgoingMessage message)
    {
        NetOutgoingMessage sendMsg = client.CreateMessage();
        LidgrenMessageUtils.WriteMessage(message, sendMsg);
        client.SendMessage(sendMsg, NetDeliveryMethod.ReliableOrdered);
        bandwidthLogger?.RecordBytesUp(sendMsg.LengthBytes);
    }

    public override void sendMessage(OutgoingMessage message)
    {
        base.OnSendingMessage(message, sendMessageImpl, delegate
        {
            sendMessageImpl(message);
        });
    }

    private void parseDataMessageFromServer(NetIncomingMessage dataMsg)
    {
        bandwidthLogger?.RecordBytesDown(dataMsg.LengthBytes);
        IncomingMessage message = new IncomingMessage();
        try
        {
            using NetBufferReadStream stream = new NetBufferReadStream(dataMsg);
            while (dataMsg.LengthBits - dataMsg.Position >= 8)
            {
                LidgrenMessageUtils.ReadStreamToMessage(stream, message);
                base.OnProcessingMessage(message, sendMessageImpl, delegate
                {
                    processIncomingMessage(message);
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
}
