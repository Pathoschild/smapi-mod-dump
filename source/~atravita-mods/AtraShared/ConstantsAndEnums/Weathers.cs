/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Numerics;

using CommunityToolkit.Diagnostics;
using NetEscapades.EnumGenerators;

namespace AtraShared.ConstantsAndEnums;

/// <summary>
/// Weathers as flags....
/// </summary>
[Flags]
[EnumExtensions]
public enum StardewWeather : byte
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

/// <summary>
/// Extensions for the weather enum.
/// </summary>
public static partial class WeatherExtensions
{
    /// <summary>
    /// Gets a list of strings and parses them to the weatherenum.
    /// </summary>
    /// <param name="weatherList">List of strings of weathers....</param>
    /// <returns>Enum.</returns>
    public static StardewWeather ParseWeatherList(this IEnumerable<string> weatherList)
    {
        Guard.IsNotNull(weatherList);
        StardewWeather weather = StardewWeather.None;
        foreach (string? weatherstring in weatherList)
        {
            if (StardewWeatherExtensions.TryParse(name: weatherstring.AsSpan().Trim(), value: out StardewWeather w, ignoreCase: true))
            {
                weather |= w;
            }
        }
        return weather;
    }

    public static int CountWeathers(this StardewWeather weather)
        => BitOperations.PopCount((uint)weather);
}
