/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/calebstein1/StardewVariableSeasons
**
*************************************************/

using StardewValley;

namespace StardewVariableSeasons
{
    public static class ShopStockPatches
    {
        public static void Prefix(ref Season item_season)
        {
            var nextSeason = SeasonUtils.GetNextSeason(Game1.season);

            if (item_season == nextSeason)
                item_season = Game1.season;
        }
    }
}