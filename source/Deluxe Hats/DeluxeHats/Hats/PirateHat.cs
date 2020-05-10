using System;

namespace DeluxeHats.Hats
{
    public static class PirateHat
    {
        public const string Name = "Pirate Hat";
        private const int pirateTreasureChanceMultiplyer = 2;
        public static void Activate()
        {
            StardewValley.Tools.FishingRod.baseChanceForTreasure *= pirateTreasureChanceMultiplyer;
        }

        public static void Disable()
        {
            StardewValley.Tools.FishingRod.baseChanceForTreasure /= pirateTreasureChanceMultiplyer;
        }
    }
}
