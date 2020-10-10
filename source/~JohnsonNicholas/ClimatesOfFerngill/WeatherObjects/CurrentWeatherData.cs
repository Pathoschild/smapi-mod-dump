/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

using System;

namespace ClimatesOfFerngillRebuild
{
    public class CurrentWeatherData
    {
        public string CurrentWeather { get; set; }
        public int NumDaysOfWeather { get; set; }
        public CurrentWeatherData()
        {
            CurrentWeather = "";
            NumDaysOfWeather = 0;
        }
        public CurrentWeatherData(string curr, int days)
        {
            CurrentWeather = curr;
            NumDaysOfWeather = days;
        }
        public CurrentWeatherData(CurrentWeatherData c)
        {
            CurrentWeather = c.CurrentWeather;
            NumDaysOfWeather = c.NumDaysOfWeather;
        }
    }
}