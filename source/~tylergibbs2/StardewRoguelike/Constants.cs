/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace StardewRoguelike
{
    internal static class Constants
    {
        public static readonly string SaveFile = "Roguelike_311053312";

        public static readonly List<string> ValidMineMaps = new() {
            "2", "3", "4", "5", "6", "7",
            "8", "9", "11", "13", "15",
            "21", "23", "25", "26", "27",
            "custom-1", "custom-2", "custom-3",
            "custom-4", "custom-5", "custom-6",
            "custom-7", "custom-8", "custom-9"
        };

        public static readonly List<string> MapsWithWater = new()
        {
            "custom-1", "custom-2", "custom-3", "custom-8"
        };


        public static readonly List<int> ScalingOrder = new() { 6, 12, 18, 24, 30, 36, 42, 48 };
        public static readonly int DangerousThreshold = 24;

        public static readonly List<int> FloorsIncreaseGoldMax = new() { 6 };
        public static readonly List<int> FloorsIncreaseGoldMin = new() { 24 };

        internal readonly static int[] RandomDebuffIds = new int[]
        {
            12, 17, 13,
            18, 14, 19,
            25, 26, 27
        };

        internal readonly static int[] RandomBuffIds = new int[]
        {
            20, 21, 22,
            28
        };

        public static readonly int MinimumMonstersPerFloor = 5;
        public static readonly int MaximumMonstersPerFloorPreLoop = 15;
        public static readonly int MaximumMonstersPerFloorPostLoop = 30;

        public static readonly int MaximumPerkCount = 12;

        public static readonly List<int> PossibleFish = new() { 155, 269, 698, 795, 838, 128, 129 };
    }
}
