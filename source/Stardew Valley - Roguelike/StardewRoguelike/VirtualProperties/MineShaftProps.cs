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
using StardewRoguelike.ChallengeFloors;
using StardewRoguelike.Netcode;
using StardewValley.Locations;
using System.Runtime.CompilerServices;

namespace StardewRoguelike.VirtualProperties
{
    public static class MineShaftLevel
    {
        internal class Holder { public readonly NetInt Value = new(); }

        internal static ConditionalWeakTable<MineShaft, Holder> values = new();

        public static NetInt get_MineShaftLevel(this MineShaft mine)
        {
            var holder = values.GetOrCreateValue(mine);
            return holder.Value;
        }
    }

    public static class MineShaftChestFloor
    {
        internal class Holder { public readonly NetBool Value = new(); }

        internal static ConditionalWeakTable<MineShaft, Holder> values = new();

        public static NetBool get_MineShaftChestFloor(this MineShaft mine)
        {
            var holder = values.GetOrCreateValue(mine);
            return holder.Value;
        }
    }

    public static class MineShaftForgeFloor
    {
        internal class Holder { public readonly NetBool Value = new(); }

        internal static ConditionalWeakTable<MineShaft, Holder> values = new();

        public static NetBool get_MineShaftForgeFloor(this MineShaft mine)
        {
            var holder = values.GetOrCreateValue(mine);
            return holder.Value;
        }
    }

    public static class MineShaftIsChallengeFloor
    {
        internal class Holder { public readonly NetBool Value = new(); }

        internal static ConditionalWeakTable<MineShaft, Holder> values = new();

        public static NetBool get_MineShaftIsChallengeFloor(this MineShaft mine)
        {
            var holder = values.GetOrCreateValue(mine);
            return holder.Value;
        }
    }

    public static class MineShaftChallengeFloor
    {
        internal class Holder { public readonly NetRef<ChallengeBase> Value = new(); }

        internal static ConditionalWeakTable<MineShaft, Holder> values = new();

        public static NetRef<ChallengeBase> get_MineShaftChallengeFloor(this MineShaft mine)
        {
            if (mine is null)
                return null;

            var holder = values.GetOrCreateValue(mine);
            return holder.Value;
        }
    }

    public static class MineShaftCustomDestination
    {
        internal class Holder { public readonly NetVector2 Value = new(); }

        internal static ConditionalWeakTable<MineShaft, Holder> values = new();

        public static NetVector2 get_MineShaftCustomDestination(this MineShaft mine)
        {
            var holder = values.GetOrCreateValue(mine);
            return holder.Value;
        }
    }

    public static class MineShaftEntryTime
    {
        internal class Holder { public readonly NetLong Value = new(); }

        internal static ConditionalWeakTable<MineShaft, Holder> values = new();

        public static NetLong get_MineShaftEntryTime(this MineShaft mine)
        {
            var holder = values.GetOrCreateValue(mine);
            return holder.Value;
        }
    }

    public static class MineShaftUsedFortune
    {
        internal class Holder { public bool Value { get; set; } }

        internal static ConditionalWeakTable<MineShaft, Holder> values = new();

        public static void set_MineShaftUsedFortune(this MineShaft mine, bool value)
        {
            values.GetOrCreateValue(mine).Value = value;
        }

        public static bool get_MineShaftUsedFortune(this MineShaft mine)
        {
            return values.GetOrCreateValue(mine).Value;
        }
    }

    public static class MineShaftIsDarkArea
    {
        internal class Holder { public bool Value { get; set; } }

        internal static ConditionalWeakTable<MineShaft, Holder> values = new();

        public static void set_MineShaftIsDarkArea(this MineShaft mine, bool value)
        {
            values.GetOrCreateValue(mine).Value = value;
        }

        public static bool get_MineShaftIsDarkArea(this MineShaft mine)
        {
            return values.GetOrCreateValue(mine).Value;
        }
    }

    public static class MineShaftDwarfGates
    {
        internal class Holder { public readonly NetObjectList<DwarfGate> Value = new(); }

        internal static ConditionalWeakTable<MineShaft, Holder> values = new();

        public static NetObjectList<DwarfGate> get_MineShaftDwarfGates(this MineShaft mine)
        {
            var holder = values.GetOrCreateValue(mine);
            return holder.Value;
        }
    }

    public static class MineShaftLocalChests
    {
        internal class Holder { public readonly NetObjectList<NetChest> Value = new(); }

        internal static ConditionalWeakTable<MineShaft, Holder> values = new();

        public static NetObjectList<NetChest> get_MineShaftNetChests(this MineShaft mine)
        {
            var holder = values.GetOrCreateValue(mine);
            return holder.Value;
        }
    }
}
