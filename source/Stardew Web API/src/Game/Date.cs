/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using StardewValley;
using System.Text.Json.Serialization;

namespace StardewWebApi.Game;

public class Date
{
    private readonly WorldDate _date;

    public Date(int year, Season season, int day) : this(new WorldDate(year, season, day)) { }

    public Date(WorldDate date)
    {
        _date = date;
    }

    public static Date Today => new(Game1.Date);

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Season Season => _date.Season;

    public int Day => _date.DayOfMonth;

    public int Year => _date.Year;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DayOfWeek DayOfWeek => _date.DayOfWeek;

    public string ShortDayOfWeek => Game1.shortDayNameFromDayOfSeason(Day);
}