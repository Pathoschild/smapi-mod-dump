using StardewValley;
using System;

namespace DeluxeHats.Hats
{
    public static class KnightsHelmet
    {
        public const string Name = "Knight's Helmet";
        private const int knightsResilience = 10;
        private const int kightsImmunity = 4;
        public static void Activate()
        {
            Game1.player.resilience += knightsResilience;
            Game1.player.immunity += kightsImmunity;
        }

        public static void Disable()
        {
            Game1.player.resilience -= knightsResilience;
            Game1.player.immunity -= kightsImmunity;
        }
    }
}
