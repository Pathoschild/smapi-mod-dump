/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Netcode;
using StardewValley;
using System.Runtime.CompilerServices;

namespace Circuit.VirtualProperties
{
    public static class FarmerTeamCurrentEvent
    {
        internal class Holder { public readonly NetRef<EventBase> Value = new(); }

        internal static ConditionalWeakTable<FarmerTeam, Holder> Values = new();

        public static NetRef<EventBase> get_FarmerTeamCurrentEvent(this FarmerTeam team)
        {
            Holder holder = Values.GetOrCreateValue(team);
            return holder.Value;
        }
    }
}
