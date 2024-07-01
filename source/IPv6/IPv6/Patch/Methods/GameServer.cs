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
// StardewValley.Network.GameServer

extern alias slnet;
using slnet::Lidgren.Network;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;

namespace IPv6.Patch.Methods;

internal static class GameServer
{
    public static void UpdateLocalOnlyFlag(object __instance)
    {
#if DEBUG
        MyPatch.LogInfo($"method {typeof(GameServer).FullName}.UpdateLocalOnlyFlag() patched");
#endif
        if (!Game1.game1.IsMainInstance)
        {
            return;
        }
        bool local_only = true;
        HashSet<long> local_clients = new HashSet<long>();
        GameRunner.instance.ExecuteForInstances(delegate
        {
            Client client = Game1.client;
            if (client == null && Game1.activeClickableMenu is FarmhandMenu farmhandMenu)
            {
                client = farmhandMenu.client;
            }
            if (client is Classes.LidgrenClient lidgrenClient)
            {
                local_clients.Add(lidgrenClient.client.UniqueIdentifier);
            }
        });

        foreach (Server server in MyPatch.GetServers(__instance))
        {
            if (server is Classes.LidgrenServer lidgren_server)
            {
                foreach (NetConnection connection in ((NetPeer)lidgren_server.server).Connections)
                {
                    if (!local_clients.Contains(connection.RemoteUniqueIdentifier))
                    {
                        local_only = false;
                        break;
                    }
                }
            }
            else if (server.connectionsCount > 0)
            {
                local_only = false;
                break;
            }
            if (!local_only)
            {
                break;
            }
        }
        if (Game1.hasLocalClientsOnly != local_only)
        {
            Game1.hasLocalClientsOnly = local_only;
            if (Game1.hasLocalClientsOnly)
            {
                MyPatch.log.Verbose("Game has only local clients.");
            }
            else
            {
                MyPatch.log.Verbose("Game has remote clients.");
            }
        }
    }

}
