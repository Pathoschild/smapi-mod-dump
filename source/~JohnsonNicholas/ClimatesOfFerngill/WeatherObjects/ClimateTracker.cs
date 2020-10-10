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
using TwilightShards.Common;

namespace ClimatesOfFerngillRebuild
{
    public class ClimateTracker
    {
        public int DaysSinceRainedLast { get; set;} = 0;
        public int AmtOfRainInCurrentStreak { get; set;}
        public long AmtOfRainSinceDay1 { get; set;}
        public CurrentWeatherData CurrentStreak { get; set; }
	    public bool IsWeatherSystem { get; set; }
        public string WeatherSystemType { get; set; }
        public RangePair TempsOnNextDay { get; set; }
        public int WeatherSystemDays { get; set; }
	    
        public ClimateTracker()
        {
            DaysSinceRainedLast = 0;
            AmtOfRainInCurrentStreak = 0;
            AmtOfRainSinceDay1 = 0;
            CurrentStreak = new CurrentWeatherData("", 0);
	        IsWeatherSystem = false;
            WeatherSystemType = "";
            WeatherSystemDays = 0;
            TempsOnNextDay = new RangePair();
        }

        public ClimateTracker(int daysSinceLast, int amtInStreak, long TotalRain, string currentWeather, int numDays, bool weatherSystem, string wst, int weatSDays, RangePair c)
        {
            DaysSinceRainedLast = daysSinceLast;
            AmtOfRainInCurrentStreak = amtInStreak;
            AmtOfRainSinceDay1 = TotalRain;
            CurrentStreak = new CurrentWeatherData(currentWeather, numDays);
	        IsWeatherSystem = weatherSystem;
            WeatherSystemType = wst;
            WeatherSystemDays = weatSDays;
            TempsOnNextDay = new RangePair(c);
        }

        public ClimateTracker(ClimateTracker c)
        {
            DaysSinceRainedLast = c.DaysSinceRainedLast;
            AmtOfRainInCurrentStreak = c.AmtOfRainInCurrentStreak;
            AmtOfRainSinceDay1 = c.AmtOfRainSinceDay1;
            CurrentStreak = new CurrentWeatherData(c.CurrentStreak);
            IsWeatherSystem = c.IsWeatherSystem;
            WeatherSystemType = c.WeatherSystemType;
            WeatherSystemDays = c.WeatherSystemDays;
            TempsOnNextDay = new RangePair(c.TempsOnNextDay);
        }
		
	    public override string ToString(){
                 string s = $"Current weather streak is {CurrentStreak.CurrentWeather} for {CurrentStreak.NumDaysOfWeather} days. Overall, we have had {DaysSinceRainedLast} days since last rain, and {AmtOfRainInCurrentStreak} raindrops in the current rain streak.";
			       s += Environment.NewLine + $"Weather System status : {IsWeatherSystem} with weather {WeatherSystemType}, and {WeatherSystemDays} days in system. and a total save-long rain total of {AmtOfRainSinceDay1} raindrops. Saved temperature data is {TempsOnNextDay}";
			
			return s;
		}
    }
}
