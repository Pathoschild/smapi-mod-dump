/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace AtraShared.ConstantsAndEnums;

#pragma warning disable SA1602 // Enumeration items should be documented. Should be obvious enough

/// <summary>
/// Seasons as flags, typically used for season constraints.
/// </summary>
[Flags]
public enum StardewSeasons : uint
{
    /// <summary>
    /// No season constraints.
    /// </summary>
    None = 0,

    /// <summary>
    /// Spring.
    /// </summary>
    Spring = 0b1,

    /// <summary>
    /// Summer.
    /// </summary>
    Summer = 0b10,

    /// <summary>
    /// Fall.
    /// </summary>
    Fall = 0b100,

    /// <summary>
    /// Winter.
    /// </summary>
    Winter = 0b1000,

    /// <summary>
    /// Every season.
    /// </summary>
    All = Spring | Summer | Fall | Winter,
}

/// <summary>
/// Extensions for the seasons enum.
/// </summary>
public static class SeasonExtensions
{
    /// <summary>
    /// Parses a list of strings into the season enum.
    /// </summary>
    /// <param name="seasonList">List of strings of seasons...</param>
    /// <returns>Stardew Seasons.</returns>
    public static StardewSeasons ParseSeasonList(this List<string> seasonList)
    {
        StardewSeasons season = StardewSeasons.None;
        foreach (string? seasonstring in seasonList)
        {
            if (Enum.TryParse(seasonstring, ignoreCase: true, out StardewSeasons s))
            {
                season |= s;
            }
        }
        return season;
    }

    /// <summary>
    /// Gets the StardewSeason enum for the current location.
    /// </summary>
    /// <param name="loc">GameLocation.</param>
    /// <returns>Season.</returns>
#warning - need to fix in Stardew 1.6.
    public static StardewSeasons GetSeasonFromGame(GameLocation? loc)
        => Utility.getSeasonNumber(Game1.GetSeasonForLocation(loc)) switch
        {
            0 => StardewSeasons.Spring,
            1 => StardewSeasons.Summer,
            2 => StardewSeasons.Fall,
            3 => StardewSeasons.Winter,
            _ => StardewSeasons.None,
        };
}

/// <summary>
/// Weathers as flags....
/// </summary>
[Flags]
public enum StardewWeather : uint
{
    /// <summary>
    /// No weather contraints.
    /// </summary>
    None = 0,

    /// <summary>
    /// Sunny weather.
    /// </summary>
    Sunny = 0b1,

    /// <summary>
    /// Rain
    /// </summary>
    Rainy = 0b10,

    /// <summary>
    /// Storming.
    /// </summary>
    Stormy = 0b100,

    /// <summary>
    /// Snowing (winter only).
    /// </summary>
    Snowy = 0b1000,

    /// <summary>
    /// Windy weather, usually leaves blowing around the screen.
    /// </summary>
    Windy = 0b10000,

    /// <summary>
    /// All weathers.
    /// </summary>
    All = Sunny | Rainy | Stormy | Snowy | Windy,
}
#pragma warning restore SA1602 // Enumeration items should be documented

/// <summary>
/// Extensions for the weather enum.
/// </summary>
public static class WeatherExtensions
{
    /// <summary>
    /// Gets a list of strings and parses them to the weatherenum.
    /// </summary>
    /// <param name="weatherList">List of strings of weathers....</param>
    /// <returns>Enum.</returns>
    public static StardewWeather ParseWeatherList(this List<string> weatherList)
    {
        StardewWeather weather = StardewWeather.None;
        foreach (string? weatherstring in weatherList)
        {
            if (Enum.TryParse(weatherstring, ignoreCase: true, out StardewWeather w))
            {
                weather |= w;
            }
        }
        return weather;
    }
}