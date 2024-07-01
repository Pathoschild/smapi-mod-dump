/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enteligenz/StardewMods
**
*************************************************/

using System;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using StardewModdingAPI;

namespace TwitchChatIntegration
{
    public class TwitchBot
    {
        const string ip = "irc.chat.twitch.tv";
        const int port = 6697;

        private string username;
        private string password;
        private bool hasAlertedConnected = false;
        private bool shouldRun = true;
        private StreamReader streamReader;
        private StreamWriter streamWriter;
        private TaskCompletionSource<int> connected = new TaskCompletionSource<int>();

        IMonitor monitor;

        public event TwitchChatEventHandler OnMessage = delegate { };
        public delegate void TwitchChatEventHandler(object sender, TwitchChatMessage e);

        public event TwitchChatStatusHandler OnStatus = delegate { };
        public delegate void TwitchChatStatusHandler(bool isError, string locKey, string rawMessage = "");

        public class TwitchChatMessage : EventArgs
        {
            public string Sender { get; set; }
            public string Message { get; set; }
            public string Channel { get; set; }
            public string Color { get; set; } = string.Empty;
            public bool IsSystem { get; set; } = false;
        }

        public TwitchBot(IMonitor monitor)
        {
            this.monitor = monitor;
        }

        public TwitchBot(string username, string password, IMonitor monitor)
        {
            this.username = username;
            this.password = password;
            this.monitor = monitor;
        }

        public void SetUserPass(string username, string password)
        {
            this.username = username;
            this.password = password;
            this.hasAlertedConnected = false;
        }

        public bool IsConnected() => this.hasAlertedConnected;

        public async Task Start()
        {
            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(ip, port);
            SslStream sslStream = new SslStream(
                tcpClient.GetStream(),
                false,
                ValidateServerCertificate,
                null
            );
            await sslStream.AuthenticateAsClientAsync(ip);
            streamReader = new StreamReader(sslStream);
            streamWriter = new StreamWriter(sslStream) { NewLine = "\r\n", AutoFlush = true };

            // Request tag support
            await streamWriter.WriteLineAsync("CAP REQ :twitch.tv/commands twitch.tv/tags");

            // Login
            await streamWriter.WriteLineAsync($"PASS {password}");
            await streamWriter.WriteLineAsync($"NICK {username}");
            connected.SetResult(0);

            try
            {
                // Permanent loop waiting for new Twitch messages
                while (this.shouldRun)
                {
                    string line = await streamReader.ReadLineAsync();

                    // If we disconnect between the last read and now, go ahead and disconnect.
                    if (!this.shouldRun)
                        break;

                    string[] split = line.Split(' ');

                    // PING :tmi.twitch.tv
                    // Respond with PONG :tmi.twitch.tv
                    if (line.StartsWith("PING"))
                    {
                        await streamWriter.WriteLineAsync($"PONG {split[1]}");
                    }

                    // Twitch IRC Message Handling
                    if (split.Length > 2)
                    {
                        string IRCMessage = split[2];
                        string channel = (split.Length > 3) ? split[3].TrimStart('#') : string.Empty;

                        string GetMessage(string MessageType)
                        {
                            string msgFindStart = $"{MessageType} #{channel} :";
                            int messageStartLocation = line.IndexOf(msgFindStart);
                            if (messageStartLocation == -1)
                                return string.Empty;

                            return line.Substring(messageStartLocation + msgFindStart.Length);
                        };

                        string GetTagString(string FieldToLookFor)
                        {
                            FieldToLookFor += '=';
                            int fieldLoc = split[0].IndexOf(FieldToLookFor);

                            if (fieldLoc == -1)
                                return string.Empty;

                            int fieldMessageEnd = split[0].IndexOf(';', fieldLoc);
                            if (fieldMessageEnd == -1)
                                return string.Empty;

                            fieldLoc += FieldToLookFor.Length;

                            return split[0].Substring(fieldLoc, fieldMessageEnd - fieldLoc);
                        };

                        // Normal message
                        if (IRCMessage == "PRIVMSG")
                        {
                            // Grab name
                            int exclamationPointPosition = split[1].IndexOf("!");
                            string username = split[1].Substring(1, exclamationPointPosition - 1);

                            // Find the message location
                            string message = GetMessage("PRIVMSG");
                            string colorHex = GetTagString("color");

                            this.OnMessage(this, new TwitchChatMessage
                            {
                                Message = message,
                                Sender = username,
                                Channel = channel,
                                Color = colorHex
                            });
                        }
                        else if (IRCMessage == "JOIN" || IRCMessage == "ROOMSTATE") // Channel connection established
                        {
                            if (this.hasAlertedConnected)
                                continue;

                            this.OnStatus.Invoke(false, "twitch.status.connected");
                            this.hasAlertedConnected = true;
                        }
                        else if (IRCMessage == "USERNOTICE")
                        {
                            // Code for raids and subscriptions

                            // Get the system-msg from twitch
                            string systemMessage = GetTagString("system-msg").Replace("\\s", " ");
                            // Message Type
                            string messageType = GetTagString("msg-id");

                            // Resubscriptions can have an multiple message assigned to them.
                            // Watch streaks also show messages too
                            if (messageType == "resub" || messageType == "viewermilestone")
                            {
                                string username = GetTagString("login");
                                string message = GetMessage("USERNOTICE");
                                string colorHex = GetTagString("color");
                                this.OnMessage(this, new TwitchChatMessage
                                {
                                    Message = message,
                                    Sender = username,
                                    Channel = channel,
                                    Color = colorHex
                                });
                            }

                            this.OnMessage(this, new TwitchChatMessage
                            {
                                Message = systemMessage,
                                Sender = "twitch",
                                Channel = channel,
                                IsSystem = true
                            });
                        }
                        else if (IRCMessage == "RECONNECT")
                        {
                            this.OnStatus.Invoke(true, "twitch.status.error.reconnect");
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
                this.monitor.Log($"Encountered an error, most likely caused by invalid Twitch login credentials.", LogLevel.Debug);
                this.OnStatus.Invoke(true, "twitch.status.error.exception");
            }

            // Cleanup our connections...
            tcpClient.Close();
            streamReader.Close();
            streamWriter.Close();
        }

        public async Task JoinChannel(string channel)
        {
            await connected.Task;
            this.OnStatus.Invoke(false, "twitch.status.connecting");
            await streamWriter.WriteLineAsync($"JOIN #{channel}");
        }

        public void Disconnect()
        {
            // This will kill the main processing loop, rendering the tcpClient to close.
            this.shouldRun = false;
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return sslPolicyErrors == SslPolicyErrors.None;
        }
    }
}
