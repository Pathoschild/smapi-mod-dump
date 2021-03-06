/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/unidarkshin/Stardew-Mods
**
*************************************************/

using System;

namespace ExtremeFishingOverhaul {
    public class ModConfig
    {
        public int seed { get; set; } = (int)(DateTime.Now.Ticks & 0x0000FFFF);
        public int maxFishingLevel { get; set; } = 10;
        public int minFishingLevel { get; set; } = 1;
        public bool maxLevOverride { get; set; } = false;
        public int maxFish { get; set; } = 1000;

    }
}