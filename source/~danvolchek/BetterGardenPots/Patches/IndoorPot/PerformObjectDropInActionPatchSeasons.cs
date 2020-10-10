/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using StardewValley;

namespace BetterGardenPots.Patches.IndoorPot
{
    internal class PerformObjectDropInActionPatchSeasons
    {
        private static bool wasOutdoors;

        public static void Prefix()
        {
            wasOutdoors = Game1.currentLocation.IsOutdoors;
            Game1.currentLocation.IsOutdoors = false;
        }

        public static void Postfix()
        {
            Game1.currentLocation.IsOutdoors = wasOutdoors;
        }
    }
}
