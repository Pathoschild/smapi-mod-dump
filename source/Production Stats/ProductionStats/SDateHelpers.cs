/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlameHorizon/ProductionStats
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace ProductionStats;

internal static class SDateHelpers
{
    private const int DaysInWeek = 7;
    private const int DaysInSeason = 28;
    private const int SeasonsPerYear = 4;
    private const int DaysInYear = DaysInSeason * SeasonsPerYear;

    /// <summary>
    /// The number of weeks elapsed since 1st of Spring year 1.
    /// 1st of Spring year 1 will return 1.
    /// </summary>
    /// <param name="date">Date from which week will be calculated.</param>
    /// <returns>Week since 1st of Spring year 1.</returns>
    public static int Week(this SDate date)
        => (int)Math.Ceiling(date.DaysSinceStart / 7d);

    /// <summary>
    /// Create instance of the lowest possible date in the game.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="allowDayZero">Whether to allow 0 spring Y1 as a valid date.</param>
    /// <returns>Lowest possible date in the game.</returns>
    public static SDate MinValue(this SDate _, bool allowZeroDay)
    {
        if (allowZeroDay)
        {
            return new SDate(0, StardewValley.Season.Spring, 1);
        }

        return new SDate(1, StardewValley.Season.Spring, 1);
    }

    /// <summary>
    /// Get a new date with the given number of weeks added.
    /// </summary>
    /// <param name="offset">The number of weeks to add.</param>
    /// <returns>Returns the resulting date.</returns>
    public static SDate AddWeeks(this SDate date, int value)
        => date.AddDays(value * DaysInWeek);

    /// <summary>
    /// Get a new date with the given number of seasons added.
    /// </summary>
    /// <param name="offset">The number of seasons to add.</param>
    /// <returns>Returns the resulting date.</returns>
    public static SDate AddSeasons(this SDate date, int value)
        => date.AddDays(value * DaysInSeason);

    /// <summary>
    /// Get a new date with the given number of years added.
    /// </summary>
    /// <param name="offset">The number of years to add.</param>
    /// <returns>Returns the resulting date.</returns>
    public static SDate AddYears(this SDate date, int value)
        => date.AddDays(value * DaysInYear);

    /// <summary>
    /// Determines whether the current date falls within 
    /// a specified range of dates.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <param name="start">The start date of the range.</param>
    /// <param name="end">The end date of the range.</param>
    /// <returns>
    /// True if the current date falls between the start 
    /// and end dates (inclusive); otherwise, false.
    /// </returns>
    public static bool IsBetween(this SDate date, SDate start, SDate end)
    {
        return date.DaysSinceStart >= start.DaysSinceStart
            && date.DaysSinceStart <= end.DaysSinceStart;
    }

    /// <summary>
    /// Returns the Monday's date for the given date in that week.
    /// </summary>
    /// <param name="date">The date to find the Monday's date in that week.</param>
    /// <returns>The Monday in the given week.</returns>
    public static SDate FirstWeekday(this SDate date)
    {
        // Determine the offset needed to reach the
        // first day of the week (Monday).
        // If the current day is Sunday, the offset is set to 7,
        // otherwise, it's the difference between
        // the current day of the week and
        // Monday (DayOfWeek.Monday is 1, so -1 is subtracted).
        int offset = date.DayOfWeek == DayOfWeek.Sunday
            ? 7
            : (int)date.DayOfWeek - 1;

        return date.AddDays(-offset);
    }

    /// <summary>
    /// Returns the Sunday's date for the given date in that week.
    /// </summary>
    /// <param name="date">The date to find the Sunday's date in that week.</param>
    /// <returns>The Sunday in the given week.</returns
    public static SDate LastWeekday(this SDate date)
        => date.AddDays(7 - (int)date.DayOfWeek);
}
