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
using System.Collections.Generic;
using StardewValley;

namespace StardewVariableSeasons
{
    public sealed class Seasons
    {
        public int GenNextChangeDate()
        {
            var rnd = new Random();
            var rndNum = rnd.Next(100);

            var map = new List<(int, int)>
            {
                (0, 22),
                (2, 23),
                (7, 24),
                (12, 25),
                (22, 26),
                (37, 27),
                (52, 1),
                (62, 2),
                (67, 3),
                (72, 4),
                (74, 5),
                (75, 6)
            };

            foreach (var (upper, result) in map)
            {
                if (rndNum <= upper)
                    return result;
            }

            return 28;
        }
        private static int GetSeasonId(string seasonName)
        {
            var map = new List<(string, int)>
            {
                ("spring", 0),
                ("summer", 1),
                ("fall", 2),
                ("winter", 3)
            };

            foreach (var (season, num) in map)
            {
                if (seasonName == season)
                    return num;
            }

            return GetSeasonId(Game1.currentSeason);
        }
        
        private static string GetSeasonById(int seasonId)
        {
            if (seasonId > 3)
                seasonId = 0;
            else if (seasonId < 0)
                seasonId = 3;
                    
            var map = new List<(int, string)>
            {
                (0, "spring"),
                (1, "summer"),
                (2, "fall"),
                (3, "winter")
            };

            foreach (var (num, season) in map)
            {
                if (seasonId == num)
                    return season;
            }

            return Game1.currentSeason;
        }

        public string Next(string season)
        {
            return GetSeasonById(GetSeasonId(season) + 1);
        }
        public string Prev(string season)
        {
            return GetSeasonById(GetSeasonId(season) - 1);
        }
    }
}