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
using StardewValley.Locations;
using System.Runtime.CompilerServices;

namespace StardewRoguelike.VirtualProperties
{
    public static class DwarfGateDisabled
    {
        internal class Holder { public readonly NetBool Value = new(); }

        internal static ConditionalWeakTable<DwarfGate, Holder> values = new();

        public static NetBool get_DwarfGateDisabled(this DwarfGate gate)
        {
            var holder = values.GetOrCreateValue(gate);
            return holder.Value;
        }
    }
}
