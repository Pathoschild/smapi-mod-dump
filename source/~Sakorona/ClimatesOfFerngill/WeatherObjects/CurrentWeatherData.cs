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