/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/SpaceCore_SDV
**
*************************************************/

using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;

namespace SpaceCore
{
    public class Networking
    {
        internal static Dictionary<string, Action<IncomingMessage>> messageHandlers = new Dictionary<string, Action<IncomingMessage>>();

        public static void RegisterMessageHandler(string id, Action<IncomingMessage> handler)
        {
            messageHandlers.Add(id, handler);
        }

        public static void BroadcastMessage(string id, byte[] data)
        {
            if (!Game1.IsMultiplayer)
                return;

            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                //writer.Write((byte)234);
                writer.Write(id);
                writer.Write(data);

                if (Game1.IsServer)
                {
                    foreach (var key in Game1.otherFarmers.Keys)
                        Game1.server.sendMessage(key, 234, Game1.player, stream.ToArray());
                }
                else
                {
                    Game1.client.sendMessage(234, stream.ToArray());
                }
            }
        }

        public static void ServerSendTo(long farmerId, string id, byte[] data)
        {
            if (!Game1.IsServer)
                return;

            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                //writer.Write((byte)234);
                writer.Write(id);
                writer.Write(data);
                Game1.server.sendMessage(farmerId, 234, Game1.player, stream.ToArray());
            }
        }
    }
}
