/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewValley.Tools;
using System.Runtime.CompilerServices;

namespace StardewRoguelike.VirtualProperties
{
    public static class FishingRodProps
    {
        internal class Holder { public int Value { get; set; } = 0; }

        internal static ConditionalWeakTable<FishingRod, Holder> values = new();

        public static void increment_FishingRodUses(this FishingRod mine)
        {
            values.GetOrCreateValue(mine).Value++;
        }

        public static int get_FishingRodUses(this FishingRod mine)
        {
            return values.GetOrCreateValue(mine).Value;
        }
    }
}
