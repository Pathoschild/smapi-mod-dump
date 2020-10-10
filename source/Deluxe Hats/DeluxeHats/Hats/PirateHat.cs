/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/domsim1/stardew-valley-deluxe-hats-mod
**
*************************************************/

namespace DeluxeHats.Hats
{
    public static class PirateHat
    {
        public const string Name = "Pirate Hat";
        public const string Description = "Double the chance to find treasure while fishing";
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
