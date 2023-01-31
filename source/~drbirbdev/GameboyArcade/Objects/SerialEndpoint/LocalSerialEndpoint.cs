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
using System.Collections.Generic;
using System.Threading;
using BirbShared;
using CoreBoy.serial;
using StardewModdingAPI.Utilities;

namespace GameboyArcade
{
    class LocalSerialEndpoint : SerialEndpoint, IDisposable
    {
        private static readonly PerScreen<LocalSerialEndpoint> Online = new PerScreen<LocalSerialEndpoint>();

        public int ReceivedByte;

        public LocalSerialEndpoint()
        {
            Online.Value = this;
        }

        public int transfer(int outgoing)
        {
            this.Send(outgoing);

            int incoming = this.ReceivedByte;
            this.ReceivedByte = 0;
            Log.Debug($"Outgoing {outgoing.ToString("X2")} / Incoming {incoming.ToString("X2")}");
            return incoming;
        }

        private void Send(int outgoing)
        {
            foreach (KeyValuePair<int, LocalSerialEndpoint> player in Online.GetActiveValues())
            {
                if (player.Value == this)
                {
                    continue;
                }
                player.Value.ReceivedByte = outgoing;
            }
        }

        public void Dispose()
        {
            Online.Value = null;
        }
    }
}
