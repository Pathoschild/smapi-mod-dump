/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;

namespace StardewArchipelago.Constants.Vanilla
{
    public static class Days
    {
        public const string MONDAY = "Monday";
        public const string TUESDAY = "Tuesday";
        public const string WEDNESDAY = "Wednesday";
        public const string THURSDAY = "Thursday";
        public const string FRIDAY = "Friday";
        public const string SATURDAY = "Saturday";
        public const string SUNDAY = "Sunday";

        public static readonly string[] DaysOfWeek = { MONDAY, TUESDAY, WEDNESDAY, THURSDAY, FRIDAY, SATURDAY, SUNDAY };

        public static string GetDayOfWeekName(int day)
        {
            var dayOfWeek = day % 7;
            switch (dayOfWeek)
            {
                case 0:
                    return SUNDAY;
                case 1:
                    return MONDAY;
                case 2:
                    return TUESDAY;
                case 3:
                    return WEDNESDAY;
                case 4:
                    return THURSDAY;
                case 5:
                    return FRIDAY;
                case 6:
                    return SATURDAY;
            }

            throw new ArgumentException($"Invalid day: {day}");
        }
    }
}
