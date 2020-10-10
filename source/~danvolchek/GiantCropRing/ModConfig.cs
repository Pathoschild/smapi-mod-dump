/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

namespace GiantCropRing
{
    internal class ModConfig
    {
        public double cropChancePercentWithRing { get; set; } = 0.05;
        public bool shouldWearingBothRingsDoublePercentage { get; set; } = true;
        public double percentOfDayNeededToWearRingToTriggerEffect { get; set; } = 0.5;

        public int cropRingPrice { get; set; } = 5000;
    }
}
