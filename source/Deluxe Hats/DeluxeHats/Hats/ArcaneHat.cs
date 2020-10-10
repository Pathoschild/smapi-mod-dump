/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/domsim1/stardew-valley-deluxe-hats-mod
**
*************************************************/

using StardewValley;

namespace DeluxeHats.Hats
{
    public static class ArcaneHat
    {
        public const string Name = "Arcane Hat";
        public const string Description = "Random chance to delay time.";
        private const double arcaneSetbackTimerChance = 0.0008f;
        public static void Activate()
        {
            HatService.OnUpdateTicked = (e) =>
            {
                if (Game1.player.hasMenuOpen || !Game1.player.canMove || !Game1.game1.IsActive)
                {
                    return;
                }
                if (Game1.random.NextDouble() < (arcaneSetbackTimerChance + (Game1.player.DailyLuck / 2000.0)))
                {
                    Game1.gameTimeInterval = 0;
                }
            };
        }

        public static void Disable()
        {
        }
    }
}
