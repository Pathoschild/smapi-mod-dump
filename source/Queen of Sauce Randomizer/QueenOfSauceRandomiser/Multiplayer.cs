/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Speshkitty/QueenOfSauceRandomiser
**
*************************************************/

using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueenOfSauceRandomiser
{
    class Multiplayer
    {
        public class ShuffleDataPacket
        {
            public static string ShuffleDataPacketType = "ShuffleDataPacketType";

            public Dictionary<int, int> ShuffleData;

            public ShuffleDataPacket() { }
            public ShuffleDataPacket(Dictionary<int, int> ShuffleData)
            {
                this.ShuffleData = ShuffleData;
            }
        }

        public static void PeerConnected(object sender, PeerConnectedEventArgs e)
        {
            ModEntry.Helper.Multiplayer.SendMessage(new ShuffleDataPacket(ModEntry.ShuffleData), ShuffleDataPacket.ShuffleDataPacketType, new[] { ModEntry.Helper.ModRegistry.ModID });
        }

        public static void ModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != ModEntry.Helper.ModRegistry.ModID) //Not from us
            {
                return;
            }
            if (e.Type != ShuffleDataPacket.ShuffleDataPacketType) //Not a packet we know how to use
            {
                return;
            }

            ModEntry.ShuffleData = e.ReadAs<ShuffleDataPacket>().ShuffleData;
        }
    }
}
