/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KyuubiRan/TimeWatch
**
*************************************************/

using StardewValley;

namespace TimeWatch.Utils;

internal static class GameTimeUtils
{
    public static int TimeOfDay
    {
        get => Game1.timeOfDay;
        set => Game1.timeOfDay = value;
    }

    public static int TimeOfMinutes => TimeOfDay % 100;
    public static int TimeOfHours => TimeOfDay / 100;

    public static int CanSeek(int minutes)
    {
        var gt = GameTimeSpan.WorldNow;
        gt.AddMinutes(minutes);
        return (gt - GameTimeSpan.WorldNow).TotalMinutes;
    }

    /// <summary>
    /// Seek the time but do not update game objects.
    /// Only can seek in [06:00 ~ 24:00]
    /// </summary>
    /// <param name="minutes">Seek by minutes, can be negative.</param>
    /// <returns>Success seeked minutes</returns>
    public static int SeekTime(int minutes)
    {
        var isPlus = minutes >= 0;

        if ((TimeOfDay >= 2400 && isPlus) || (TimeOfDay <= 600 && !isPlus))
            return 0;

        var gt = GameTimeSpan.WorldNow;

        var maximumAddable = isPlus ? 2400 - TimeOfDay : TimeOfDay - 600;
        var seeked = isPlus ? Math.Min(maximumAddable, minutes) : Math.Max(-TimeOfDay, minutes);

        gt.AddMinutes(seeked);
        gt.ApplyToWorldTime();

        return seeked;
    }

    /// <summary>
    /// Seek the time and update game objects.
    /// Only can seek in [06:00 ~ 24:00]
    /// </summary>
    /// <param name="cnt">Seek by time unit count, cannot be negative.</param>
    /// <returns>Success seeked cnt</returns>
    public static int PerformUpdateTime(int cnt)
    {
        if (cnt <= 0)
            return 0;
        
        if (TimeOfDay is < 600 or >= 2400)
            return 0;

        var i = 0;
        for (; i < cnt; ++i)
        {
            if (TimeOfDay >= 2400)
                return i;

            Game1.performTenMinuteClockUpdate();
        }

        return i;
    }
}