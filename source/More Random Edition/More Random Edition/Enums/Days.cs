/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley;
using System.Collections.Generic;

namespace Randomizer
{
    /// <summary>
    /// Represents the days of the week - can get the current day with
    /// Game1.Date.TotalDays / 7
    /// </summary>
    public enum Days
    {
        Monday = 0,
        Tuesday = 1,
        Wednesday = 2,
        Thursday = 3,
        Friday = 4,
        Saturday = 5,
        Sunday = 6
    }

    public class DayFunctions
    {
        private const string ConditionPrefix = "DAY_OF_WEEK";

        public static Days GetCurrentDay()
        {
            return (Days)(Game1.Date.TotalDays % 7);
        }

        public static bool IsWeekday(Days day)
        {
            return day < Days.Saturday;
        }

        public static bool IsWeekend(Days day)
        {
            return day > Days.Friday;
        }

        /// <summary>
        /// Gets the appearance condition for the given day
        /// </summary>
        /// <param name="day">The day to get the condition for</param>
        /// <returns>The condition string</returns>
        public static string GetCondition(Days day)
        {
            return $"{ConditionPrefix} {day}";
        }

        /// <summary>
        /// Gets the appearance condition for weekdays
        /// </summary>
        /// <returns>The condition string</returns>
        public static string GetConditionForWeekday()
        {
            List<Days> weekdays = new()
            {
                Days.Monday,
                Days.Tuesday,
                Days.Wednesday,
                Days.Thursday,
                Days.Friday
            };
            return $"{ConditionPrefix} {string.Join(" ", weekdays)}";
        }

        /// <summary>
        /// Gets the appearance condition for weekends
        /// </summary>
        /// <returns>The condition string</returns>
        public static string GetConditionForWeekend()
        {
            List<Days> weekends = new()
            {
                Days.Saturday,
                Days.Sunday
            };
            return $"{ConditionPrefix} {string.Join(" ", weekends)}";
        }
    }
}
