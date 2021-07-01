/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using SpaceCore.Events;
using StardewValley.Network;

namespace SpaceCore.Overrides
{
    public class ServerGotClickHook
    {
        public static void Postfix(GameServer __instance, long peer)
        {
            SpaceEvents.InvokeServerGotClient(__instance, peer);
        }
    }
}
