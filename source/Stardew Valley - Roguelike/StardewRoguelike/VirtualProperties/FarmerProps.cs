/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using Netcode;
using StardewValley;
using System.Runtime.CompilerServices;

namespace StardewRoguelike.VirtualProperties
{
    public static class FarmerIsSpectating
    {
        internal class Holder { public readonly NetBool Value = new(); }

        internal static ConditionalWeakTable<Farmer, Holder> values = new();

        public static NetBool get_FarmerIsSpectating(this Farmer farmer)
        {
            var holder = values.GetOrCreateValue(farmer);
            return holder.Value;
        }
    }

    public static class FarmerCurrentLevel
    {
        internal class Holder { public readonly NetInt Value = new(); }

        internal static ConditionalWeakTable<Farmer, Holder> values = new();

        public static NetInt get_FarmerCurrentLevel(this Farmer farmer)
        {
            var holder = values.GetOrCreateValue(farmer);
            return holder.Value;
        }
    }

    public static class FarmerCurses
    {
        internal class Holder { public readonly NetList<int, NetInt> Value = new(); }

        internal static ConditionalWeakTable<Farmer, Holder> values = new();

        public static NetList<int, NetInt> get_FarmerCurses(this Farmer farmer)
        {
            var holder = values.GetOrCreateValue(farmer);
            return holder.Value;
        }
    }
}
