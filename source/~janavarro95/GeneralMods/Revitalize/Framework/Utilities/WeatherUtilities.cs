/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace Omegasis.Revitalize.Framework.Utilities
{
    /// <summary>
    /// Deals with usefull utilities to help determine the weather and the effects it has.
    /// </summary>
    public class WeatherUtilities
    {

        public static bool IsItRaining()
        {

            return Game1.isRaining;
        }

        public static bool IsItSnowing()
        {
            return Game1.isSnowing;
        }

        public static bool IsStorm()
        {
            return Game1.isLightning;
        }

        public static bool IsWedding()
        {
            return Game1.weddingToday;
        }

        public static bool IsSunny()
        {
            return Game1.weatherIcon == Game1.weather_sunny;
        }

        public static bool IsWindyDay()
        {
            return Game1.isDebrisWeather;
        }

        public static bool IsFestivalWeather()
        {
            return Game1.weatherIcon == Game1.weather_festival;
        }

        /// <summary>
        /// Checks if the weather for the day is good weather. Aka no rain or snow.
        /// </summary>
        /// <returns></returns>
        public static bool IsClearWeather()
        {
            return IsWindyDay() || IsSunny() || IsWedding() || IsFestivalWeather();
        }

        /// <summary>
        /// Checks if the weather for the day is unclear.
        /// </summary>
        /// <returns></returns>
        public static bool IsWetWeather()
        {
            return IsItRaining() || IsItSnowing() || IsStorm();
        }

        public static bool IsWeatherGoodForWindmills()
        {
            return IsWindyDay() || IsStorm();
        }

    }
}
