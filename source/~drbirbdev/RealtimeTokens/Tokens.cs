/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using BirbCore.Attributes;

namespace RealtimeFramework;

[SToken]
internal class Tokens
{

    [SToken.Token]
    public static IEnumerable<string> Hour()
    {
        yield return "" + DateTime.Now.Hour;
    }

    [SToken.Token]
    public static IEnumerable<string> DayOfMonth()
    {
        yield return "" + DateTime.Today.Day;
    }

    [SToken.Token]
    public static IEnumerable<string> DayOfWeek()
    {
        yield return "" + ((int)DateTime.Today.DayOfWeek + 1);
    }

    [SToken.Token]
    public static IEnumerable<string> DayOfYear()
    {
        yield return "" + DateTime.Today.DayOfYear;
    }

    [SToken.Token]
    public static IEnumerable<string> Month()
    {
        yield return "" + DateTime.Today.Month;
    }

    [SToken.Token]
    public static IEnumerable<string> Year()
    {
        yield return "" + DateTime.Today.Year;
    }

    [SToken.Token]
    public static IEnumerable<string> WeekdayLocal()
    {
        yield return ModEntry.Instance.I18N.Get("time.weekday." + DateTime.Today.DayOfWeek);
    }

    [SToken.Token]
    public static IEnumerable<string> MonthLocal()
    {
        yield return ModEntry.Instance.I18N.Get("time.month." + DateTime.Today.Month);
    }

    [SToken.Token]
    // Because Content Patcher treats null and empty arrays as an unready token, we need to return
    // a single empty string iff we would otherwise return an empty set of values.
    public static IEnumerable<string> AllHolidays()
    {
        bool empty = true;
        foreach (string holiday in ModEntry.Api.GetAllHolidays())
        {
            empty = false;
            yield return holiday;
        }
        if (empty)
        {
            yield return "";
        }
    }

    [SToken.Token]
    public static IEnumerable<string> ComingHolidays()
    {
        bool empty = true;
        foreach (string holiday in ModEntry.Api.GetComingHolidays())
        {
            empty = false;
            yield return holiday;
        }
        if (empty)
        {
            yield return "";
        }
    }

    [SToken.Token]
    public static IEnumerable<string> CurrentHolidays()
    {
        bool empty = true;
        foreach (string holiday in ModEntry.Api.GetCurrentHolidays())
        {
            empty = false;
            yield return holiday;
        }
        if (empty)
        {
            yield return "";
        }
    }

    [SToken.Token]
    public static IEnumerable<string> PassingHolidays()
    {
        bool empty = true;
        foreach (string holiday in ModEntry.Api.GetPassingHolidays())
        {
            empty = false;
            yield return holiday;
        }
        if (empty)
        {
            yield return "";
        }
    }

    [SToken.Token]
    public static IEnumerable<string> AllHolidaysLocal()
    {
        bool empty = true;
        foreach (string holiday in ModEntry.Api.GetAllHolidays())
        {
            empty = false;
            yield return ModEntry.Api.GetLocalName(holiday);
        }
        if (empty)
        {
            yield return "";
        }
    }

    [SToken.Token]
    public static IEnumerable<string> ComingHolidaysLocal()
    {
        bool empty = true;
        foreach (string holiday in ModEntry.Api.GetComingHolidays())
        {
            empty = false;
            yield return ModEntry.Api.GetLocalName(holiday);
        }
        if (empty)
        {
            yield return "";
        }
    }

    [SToken.Token]
    public static IEnumerable<string> CurrentHolidaysLocal()
    {
        bool empty = true;
        foreach (string holiday in ModEntry.Api.GetCurrentHolidays())
        {
            empty = false;
            yield return ModEntry.Api.GetLocalName(holiday);
        }
        if (empty)
        {
            yield return "";
        }
    }

    [SToken.Token]
    public static IEnumerable<string> PassingHolidaysLocal()
    {
        bool empty = true;
        foreach (string holiday in ModEntry.Api.GetPassingHolidays())
        {
            empty = false;
            yield return ModEntry.Api.GetLocalName(holiday);
        }
        if (empty)
        {
            yield return "";
        }
    }
}
