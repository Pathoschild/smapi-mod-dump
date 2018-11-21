using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;

namespace ClimatesOfFerngillRebuild
{
    internal class ForceDays
    {
        private static readonly Dictionary<SDate, int> _forceDays = new Dictionary<SDate, int>
        {
            { new SDate(1,"spring"), Game1.weather_sunny },
            { new SDate(2, "spring"), Game1.weather_sunny },
            { new SDate(3, "spring"), Game1.weather_rain },
            { new SDate(4, "spring"), Game1.weather_sunny },
            { new SDate(13, "spring"), Game1.weather_festival },
            { new SDate(24, "spring"), Game1.weather_festival },
            { new SDate(1, "summer"), Game1.weather_sunny },
            { new SDate(11, "summer"), Game1.weather_festival },
            { new SDate(13, "summer"), Game1.weather_lightning},
            { new SDate(26, "summer"), Game1.weather_lightning },
            { new SDate(28, "summer"), Game1.weather_festival },
            { new SDate(1,"fall"), Game1.weather_sunny },
            { new SDate(16,"fall"), Game1.weather_festival },
            { new SDate(27,"fall"), Game1.weather_festival },
            { new SDate(1,"winter"), Game1.weather_sunny },
            { new SDate(8, "winter"), Game1.weather_festival },
            { new SDate(14, "winter"), Game1.weather_festival },
            { new SDate(15, "winter"), Game1.weather_festival },
            { new SDate(16, "winter"), Game1.weather_festival },
            { new SDate(25, "winter"), Game1.weather_festival }
        };

        public static bool CheckForForceDay(Descriptions Desc, SDate Target, IMonitor mon, bool verbose)
        {
            foreach (KeyValuePair<SDate, int> entry in _forceDays)
            {
                if (entry.Key.Day == Target.Day && entry.Key.Season == Target.Season)
                {
                    if (verbose) mon.Log($"Setting a forced value for tommorow: {Desc.DescribeInGameWeather(entry.Value)} for {entry.Key.Season} {entry.Key.Day}");
                    Game1.weatherForTomorrow = entry.Value;
                    Game1.netWorldState.Value.WeatherForTomorrow = entry.Value;
                    return true;
                }
            }
            return false;
        }
    }
}
