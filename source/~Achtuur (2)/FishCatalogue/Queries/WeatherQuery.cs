/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FishCatalogue.Queries;

//WEATHER Here Rain Storm GreenRain


enum Weather
{
    Rain,
    Storm,
    GreenRain,
    Snow,
}

internal class WeatherQuery : IQuery
{
    private static Regex ParseRegex = new(@"^WEATHER\s+(?<location>\w+)\s+(?<weather>(\w+\s?)+)");
    public Texture2D Icon => throw new NotImplementedException();
    private string query;
    private List<Weather> weathers;
    
    public static bool IsValidQuery(string query)
    {
        return ParseRegex.IsMatch(query);
    }

    public WeatherQuery(string query)
    {
        this.query = query;
        Parse();
    }

    private void Parse()
    {
        Match match = ParseRegex.Match(query);
        if (!match.Success)
            throw new ArgumentException("Invalid query format");
        if (match.Groups["location"].Value != "Here")
            throw new ArgumentException("Location not 'Here'");
        this.weathers = match.Groups["weather"].Value
            .Split(' ')
            .Select(s => string_to_weather(s))
            .ToList();
    }

    private Weather string_to_weather(string s)
    {
        LocationWeather weather = new LocationWeather();
        switch (s)
        {
            case "Rain":
                return Weather.Rain;
            case "Storm":
                return Weather.Storm;
            case "GreenRain":
                return Weather.GreenRain;
            case "Snow":
                return Weather.Snow;
            default:
                throw new ArgumentException("Invalid weather");
        }
    }

    public string Description()
    {
        throw new NotImplementedException();
    }

    public bool IsTrue()
    {
        if (weathers is null || weathers.Count == 0)
            return true;

        LocationWeather loc_weather = Game1.currentLocation.GetWeather();
        return weathers.Any(w => IsWeather(w, loc_weather));
    }

    private bool IsWeather(Weather weather, LocationWeather loc_weather)
    {
        switch (weather)
        {
            case Weather.Rain:
                return loc_weather.IsRaining;
            case Weather.Storm:
                return loc_weather.IsLightning;
            case Weather.GreenRain:
                return loc_weather.IsGreenRain;
            case Weather.Snow:
                return loc_weather.IsSnowing;
            default:
                return false;
        }
    }
}
