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

namespace StardewRoguelike.VirtualProperties
{
    public static class FarmerTeamHardMode
    {
        internal class Holder { public readonly NetBool Value = new(); }

        internal static ConditionalWeakTable<FarmerTeam, Holder> values = new();

        public static NetBool get_FarmerTeamHardMode(this FarmerTeam team)
        {
            var holder = values.GetOrCreateValue(team);
            return holder.Value;
        }
    }
}
