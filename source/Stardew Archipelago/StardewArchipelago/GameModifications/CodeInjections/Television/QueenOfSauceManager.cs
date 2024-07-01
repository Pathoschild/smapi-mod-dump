/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewArchipelago.GameModifications.Seasons;
using StardewArchipelago.Serialization;
using StardewValley;

namespace StardewArchipelago.GameModifications.CodeInjections.Television
{
    public class QueenOfSauceManager
    {
        private const int ONE_WEEK = 7;
        private const int ONE_MONTH = ONE_WEEK * 4;
        private const int ONE_YEAR = ONE_MONTH * 4;
        private const int TWO_YEARS = ONE_YEAR * 2;
        private const int YEAR_LOOP = 2;

        private readonly ArchipelagoStateDto _state;

        public QueenOfSauceManager(ArchipelagoStateDto state)
        {
            _state = state;
        }

        public List<int> GetAllRerunRecipes()
        {
            return GetAllRerunRecipes(GetCurrentWeekNumber());
        }

        public List<int> GetAllRerunRecipes(int currentWeek)
        {
            var validSeasons = SeasonsRandomizer.ValidSeasons;
            var validRerunRecipes = new List<int>();
            var seasonsOrder = _state.SeasonsOrder;
            for (var seasonIndex = 0; seasonIndex < validSeasons.Length; seasonIndex++)
            {
                var season = validSeasons[seasonIndex];
                if (!seasonsOrder.Contains(season))
                {
                    continue;
                }

                var hasCompletedCurrentSeasonBefore = seasonsOrder.Take(seasonsOrder.Count - 1).Contains(season);
                var maxWeekSeen = hasCompletedCurrentSeasonBefore ? 4 : currentWeek;

                for (var week = 0; week < maxWeekSeen; week++)
                {
                    validRerunRecipes.Add((seasonIndex * 4) + week + 1); // year1
                    validRerunRecipes.Add((seasonIndex * 4) + week + 16 + 1); // year2
                }
            }

            return validRerunRecipes;
        }

        public int GetCurrentWeekNumber()
        {
            return GetCurrentWeekNumber((int)Game1.stats.DaysPlayed);
        }

        public int GetCurrentWeekNumber(int daysPlayed)
        {
            var currentDayOfMonth = GetCurrentDayOfMonth(daysPlayed);
            return currentDayOfMonth / ONE_WEEK; // 0-3
        }

        public void GetCurrentDateComponents(out int year, out int week)
        {
            GetCurrentDateComponents((int)Game1.stats.DaysPlayed, out year, out week);
        }

        public void GetCurrentDateComponents(int daysPlayed, out int year, out int week)
        {
            var currentDayOfMonth = GetCurrentDayOfMonth(daysPlayed, out var currentYear);
            year = currentYear % YEAR_LOOP; // 0 is year1, 1 is year2
            week = currentDayOfMonth / ONE_WEEK; // 0-3
        }

        private int GetCurrentDayOfMonth(int daysPlayed)
        {
            return GetCurrentDayOfMonth(daysPlayed, out _);
        }

        private int GetCurrentDayOfMonth(int daysPlayed, out int currentYear)
        {
            var zeroIndexedDay = daysPlayed - 1;
            currentYear = zeroIndexedDay / ONE_YEAR;
            while (zeroIndexedDay < 0)
            {
                zeroIndexedDay += ONE_YEAR;
                currentYear += 1;
            }
            return zeroIndexedDay % ONE_MONTH;
        }
    }
}
