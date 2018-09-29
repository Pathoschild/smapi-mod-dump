using System;

namespace ExtremeFishingOverhaul {
    public class ModConfig
    {
        public int seed { get; set; } = (int)(DateTime.Now.Ticks & 0x0000FFFF);
        public int maxFishingLevel { get; set; } = 10;
        public int minFishingLevel { get; set; } = 1;
        public bool maxLevOverride { get; set; } = false;
        public int maxFish { get; set; } = 2028;

    }
}