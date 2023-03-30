/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System.Runtime.CompilerServices;
using StardewValley;

namespace Circuit.VirtualProperties
{
    public static class NPCIsSwapped
    {
        internal class Holder { public bool Value = false; }

        internal static ConditionalWeakTable<NPC, Holder> Values = new();

        public static void set_NPCIsSwapped(this NPC npc, bool newValue)
        {
            Holder holder = Values.GetOrCreateValue(npc);
            holder.Value = newValue;
        }

        public static bool get_NPCIsSwapped(this NPC npc)
        {
            Holder holder = Values.GetOrCreateValue(npc);
            return holder.Value;
        }
    }
}
