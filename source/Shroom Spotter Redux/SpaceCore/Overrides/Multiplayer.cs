/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Network;

namespace SpaceCore.Overrides
{
    public class MultiplayerPackets
    {
        public static bool Prefix(Multiplayer __instance, IncomingMessage msg)
        {
            // MTN uses packets 30, 31, and 50, PyTK uses 99
            
            if ( msg.MessageType == 234 )
            {
                string msgType = msg.Reader.ReadString();
                if (Networking.messageHandlers.ContainsKey(msgType))
                    Networking.messageHandlers[msgType].Invoke(msg);

                if (Game1.IsServer)
                {
                    foreach (var key in Game1.otherFarmers.Keys)
                        if (key != msg.FarmerID)
                            Game1.server.sendMessage(key, 234, Game1.otherFarmers[msg.FarmerID], msg.Data);
                }
            }

            return true;
        }
    }
}
