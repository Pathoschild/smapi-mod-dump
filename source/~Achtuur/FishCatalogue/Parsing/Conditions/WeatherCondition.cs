/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Framework.Borders;
using StardewValley.Network;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishCatalogue.Parsing.Conditions;
internal class WeatherCondition : BaseCondition
{
    private static readonly Dictionary<Weather, string> WeatherItemId = new()
    {
        {Weather.Sunny,  "768"}, // solar essence
        {Weather.Rainy,  "681"}, // rain totem
        {Weather.Both,  "(H)8"}, // skull emoji
    };

    private Weather weather;

    public WeatherCondition(Weather weather)
    {
        this.weather = weather;
    }
    public override string Description()
    {
        return WeatherToText(this.weather);
    }

    protected override string ItemID()
    {
        return WeatherItemId[this.weather];
    }

    public override bool IsTrue()
    {
        LocationWeather locationWeather = Game1.currentLocation.GetWeather();
        return weather switch
        {
            Weather.Sunny => !locationWeather.IsRaining && !locationWeather.IsSnowing && !locationWeather.IsLightning && !locationWeather.IsGreenRain && !locationWeather.IsDebrisWeather,
            Weather.Rainy => locationWeather.IsRaining || locationWeather.IsLightning,
            Weather.Both => true,
            _ => true,
        };
    }

    private string WeatherToText(Weather weather)
    {
        return weather switch
        {
            Weather.Sunny => "Sunny",
            Weather.Rainy => "Rainy",
            Weather.Both => "Any",
            _ => "WTF WEATHER???",
        };
    }
}
