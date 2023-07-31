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
        private StreamReader streamReader;
        private StreamWriter streamWriter;
        private TaskCompletionSource<int> connected = new TaskCompletionSource<int>();

        IMonitor monitor;

        public event TwitchChatEventHandler OnMessage = delegate { };
        public delegate void TwitchChatEventHandler(object sender, TwitchChatMessage e);

        public class TwitchChatMessage : EventArgs
        {
            public string Sender { get; set; }
            public string Message { get; set; }
            public string Channel { get; set; }
        }

        public TwitchBot(string username, string password, IMonitor monitor)
        {
            this.username = username;
            this.password = password;
            this.monitor = monitor;
        }

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

            await streamWriter.WriteLineAsync($"PASS {password}");
            await streamWriter.WriteLineAsync($"NICK {username}");
            connected.SetResult(0);

            try
            {
                // Permanent loop waiting for new Twitch messages
                while (true)
                {
                    string line = await streamReader.ReadLineAsync();
                    string[] split = line.Split(' ');

                    // PING :tmi.twitch.tv
                    // Respond with PONG :tmi.twitch.tv
                    if (line.StartsWith("PING"))
                    {
                        await streamWriter.WriteLineAsync($"PONG {split[1]}");
                    }

                    // Normal message
                    if (split.Length > 2 && split[1] == "PRIVMSG")
                    {
                        // Grab name
                        int exclamationPointPosition = split[0].IndexOf("!");
                        string username = split[0].Substring(1, exclamationPointPosition - 1);
                        // Skip the first character, the first colon, then find the next colon
                        int secondColonPosition = line.IndexOf(':', 1);
                        string message = line.Substring(secondColonPosition + 1);
                        string channel = split[2].TrimStart('#');

                        this.OnMessage(this, new TwitchChatMessage
                        {
                            Message = message,
                            Sender = username,
                            Channel = channel
                        });
                    }
                }
            }
            catch (NullReferenceException)
            {
                this.monitor.Log($"Encountered an error, most likely caused by invalid Twitch login credentials.", LogLevel.Debug);
            }
        }

        public async Task JoinChannel(string channel)
        {
            await connected.Task;
            await streamWriter.WriteLineAsync($"JOIN #{channel}");
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return sslPolicyErrors == SslPolicyErrors.None;
        }
    }
}
