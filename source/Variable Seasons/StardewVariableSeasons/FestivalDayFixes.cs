/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/calebstein1/StardewVariableSeasons
**
*************************************************/

using System;
using StardewValley;

namespace StardewVariableSeasons
{
    public static class FestivalDayFixes
    {
        public static void LoadFestPrefix(ref string festival)
        {
            festival = $"{ModEntry.SeasonByDay.ToString().ToLower()}{Game1.dayOfMonth}";
        }

        public static void ResetSeasonPrefix(out Season __state)
        {
            __state = Game1.season;
            Game1.season = ModEntry.SeasonByDay;
            Game1.currentSeason = ModEntry.SeasonByDay.ToString().ToLower();
            Game1.Date.Season = ModEntry.SeasonByDay;
            Game1.Date.SeasonKey = ModEntry.SeasonByDay.ToString().ToLower();
        }
        
        public static void ResetSeasonPostfix(Season __state)
        {
            Game1.season = __state;
            Game1.currentSeason = Game1.season.ToString().ToLower();
            Game1.Date.Season = Game1.season;
            Game1.Date.SeasonKey = Game1.season.ToString().ToLower();
        }
    }
}