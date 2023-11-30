/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Threading;
using BirbCore.Attributes;
using CoreBoy.serial;

namespace GameboyArcade
{
    class RemoteSerialEndpoint : SerialEndpoint, IDisposable
    {
        private readonly string MinigameId;
        private int ReceivedByte = 0xff;
        private bool Received = false;
        private int CurrentByte = 0;

        public RemoteSerialEndpoint(string minigameId)
        {
            this.MinigameId = minigameId;
        }

        public int transfer(int outgoing)
        {
            ModEntry.Instance.Helper.Events.Multiplayer.ModMessageReceived += this.Multiplayer_ModMessageReceived;

            this.Received = false;
            int time = 0;
            Message message = new Message()
            {
                Data = outgoing,
                Minigame = this.MinigameId,
                Id = this.CurrentByte,
            };
            while (!this.Received)
            {
                Thread.Sleep(100);
                time += 100;
                if (time > 10000)
                {
                    this.ReceivedByte = 0xff;
                    break;
                }
                ModEntry.Instance.Helper.Multiplayer.SendMessage<Message>(message, "SerialReceive", new string[] { ModEntry.Instance.ModManifest.UniqueID });
            }
            ModEntry.Instance.Helper.Multiplayer.SendMessage<Message>(message, "SerialReceive", new string[] { ModEntry.Instance.ModManifest.UniqueID });

            int incoming = this.ReceivedByte;
            this.ReceivedByte = 0xff;
            Log.Debug($"ID {this.CurrentByte} / Outgoing {outgoing:X2} / Incoming {incoming:X2}");

            return incoming;
        }

        private void Multiplayer_ModMessageReceived(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
        {
            if (e.FromModID == ModEntry.Instance.ModManifest.UniqueID && e.Type == "SerialReceive")
            {
                Message message = e.ReadAs<Message>();
                if (message.Minigame != this.MinigameId)
                {
                    return;
                }
                if (message.Id < this.CurrentByte)
                {
                    return;
                }
                this.ReceivedByte = message.Data;
                this.Received = true;
                this.CurrentByte++;
                ModEntry.Instance.Helper.Events.Multiplayer.ModMessageReceived -= this.Multiplayer_ModMessageReceived;
            }
        }

        public void Dispose()
        {
            ModEntry.Instance.Helper.Events.Multiplayer.ModMessageReceived -= this.Multiplayer_ModMessageReceived;
        }

        public class Message
        {
            public int Data;
            public string Minigame;
            public int Id;
        }
    }
}
