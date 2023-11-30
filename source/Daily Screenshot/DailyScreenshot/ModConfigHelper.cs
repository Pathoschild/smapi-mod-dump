/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CompSciLauren/stardew-valley-daily-screenshot-mod
**
*************************************************/

using static DailyScreenshot.ModTrigger;

namespace DailyScreenshot
{

    /// <summary>
    /// Helper methods for checking and updating settings in the Config.
    /// </summary>
    public class ModConfigHelper
    {
        /// <summary>
        /// Checks if a specific weather condition is present in the WeatherFlags
        /// </summary>
        /// <param name="weather">WeatherFlags to check</param>
        /// <param name="targetWeather">The weather condition to check for</param>
        /// <returns>True if the weather condition is present, otherwise false</returns>
        public static bool IsWeatherConditionEnabled(ModTrigger.WeatherFlags weather, ModTrigger.WeatherFlags targetWeather)
        {
            // Check if the specific weather condition is present
            return (weather & targetWeather) != 0;
        }

        /// <summary>
        /// Updates weather with the new value for targetWeather.
        /// </summary>
        /// <param name="weather">WeatherFlags to check</param>
        /// <param name="targetWeather">The weather condition to check for</param>
        /// <param name="val">What the new value should be</param>
        /// <returns>Updated value for weather</returns>
        public static ModTrigger.WeatherFlags UpdateWeatherCondition(ModTrigger.WeatherFlags weather, ModTrigger.WeatherFlags targetWeather, bool val)
        {
            if (val)
            {
                // If targetWeather is true, add it to weather
                weather |= targetWeather;
            }
            else
            {
                // If targetWeather is false, remove it from weather
                weather &= ~targetWeather;
            }

            return weather;
        }

        /// <summary>
        /// Checks if a specific location condition is present in the LocationFlags
        /// </summary>
        /// <param name="location">LocationFlags to check</param>
        /// <param name="targetLocation">The location condition to check for</param>
        /// <returns>True if the location condition is present, otherwise false</returns>
        public static bool IsLocationConditionEnabled(ModTrigger.LocationFlags location, ModTrigger.LocationFlags targetLocation)
        {
            // Check if the specific location condition is present
            return (location & targetLocation) != 0;
        }

        /// <summary>
        /// Updates location with the new value for targetLocation.
        /// </summary>
        /// <param name="location">LocationFlags to check</param>
        /// <param name="targetLocation">The location condition to check for</param>
        /// <param name="val">What the new value should be</param>
        /// <returns>Updated value for location</returns>
        public static ModTrigger.LocationFlags UpdateLocationCondition(ModTrigger.LocationFlags location, ModTrigger.LocationFlags targetLocation, bool val)
        {
            if (val)
            {
                // If targetLocation is true, add it to location
                location |= targetLocation;
            }
            else
            {
                // If targetLocation is false, remove it from location
                location &= ~targetLocation;
            }

            return location;
        }

        /// <summary>
        /// Checks if a specific date condition is present in the DateFlags
        /// </summary>
        /// <param name="date">DateFlags to check</param>
        /// <param name="targetDate">The date condition to check for</param>
        /// <returns>True if the date condition is present, otherwise false</returns>
        public static bool IsDateConditionEnabled(ModTrigger.DateFlags date, ModTrigger.DateFlags targetDate)
        {
            // Check if the specific date condition is present
            return (date & targetDate) != 0;
        }

        /// <summary>
        /// Checks whether the new value for the date condition is actually a new value or if it's the same as what's already present in the config.
        /// </summary>
        /// <param name="date">DateFlags to check</param>
        /// <param name="targetDate">The date condition to check for</param>
        /// <param name="val">What the new value should be</param>
        /// <returns>True if date condition is already set, otherwise false</returns>
        public static bool IsDateConditionAlreadySet(ModTrigger.DateFlags date, ModTrigger.DateFlags targetDate, bool val)
        {
            if (val)
            {
                // If trying to set the date to true, check if it's already true
                return (date & targetDate) != 0;
            }
            else
            {
                // If trying to set the date to false, check if it's already false
                return (date & targetDate) == 0;
            }
        }

        /// <summary>
        /// Updates date with the new value for targetDate.
        /// </summary>
        /// <param name="date">DateFlags to check</param>
        /// <param name="targetDate">The date condition to check for</param>
        /// <param name="val">What the new value should be</param>
        /// <returns>Updated value for date</returns>
        public static ModTrigger.DateFlags UpdateDateCondition(ModTrigger.DateFlags date, ModTrigger.DateFlags targetDate, bool val)
        {
            // If updating a specific day or other flags, proceed as before
            if (val)
            {
                // If targetDate is true, add it to date
                date |= targetDate;
            }
            else
            {
                // If targetDate is false, remove it from date
                date &= ~targetDate;
            }

            return date;
        }

        /// <summary>
        /// Checks if a specific fileName condition is present in the FileNameFlags
        /// </summary>
        /// <param name="fileName">FileNameFlags to check</param>
        /// <param name="targetDate">The fileName condition to check for</param>
        /// <returns>True if the fileName condition is present, otherwise false</returns>
        public static bool IsFileNameConditionEnabled(ModRule.FileNameFlags fileName, ModRule.FileNameFlags targetFileName)
        {
            // Check if the specific fileName condition is present
            return (fileName & targetFileName) != 0;
        }

        /// <summary>
        /// Updates fileName with the new value for targetFileName.
        /// </summary>
        /// <param name="fileName">FileNameFlags to check</param>
        /// <param name="targetFileName">The fileName condition to check for</param>
        /// <param name="val">What the new value should be</param>
        /// <returns>Updated value for fileName</returns>
        public static ModRule.FileNameFlags UpdateFileNameCondition(ModRule.FileNameFlags fileName, ModRule.FileNameFlags targetFileName, bool val)
        {
            if (val)
            {
                // If targetFileName is true, add it to fileName
                fileName |= targetFileName;
            }
            else
            {
                // If targetFileName is false, remove it from fileName
                fileName &= ~targetFileName;
            }

            return fileName;
        }
    }

}