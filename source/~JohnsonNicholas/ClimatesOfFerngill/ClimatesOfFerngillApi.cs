using System;

namespace ClimatesOfFerngillRebuild
{
    public interface IClimatesOfFerngillAPI
    {
        //generic functions
        string GetCurrentWeatherName();
        double? GetTodaysHigh();
        double? GetTodaysLow();
        string GetCurrentClimateName();

        //rain functions
        bool IsVariableRain();
        int GetCurrentRainTotal();
        long GetAmtOfRainSinceDay1();
        int GetDaysSinceRainedLast();
        int GetCurrentRain();

        //streak functions
        string GetCurrentWeatherStreak();
        int GetNumDaysOfStreak();
    }
    
    public class ClimatesOfFerngillAPI : IClimatesOfFerngillAPI
    {
        private WeatherConditions CurrentConditions;
        private WeatherConfig Options;

        public void LoadData(WeatherConditions Cond, WeatherConfig Opt)
        {
            CurrentConditions = Cond;
            Options = Opt;
        }

        public ClimatesOfFerngillAPI(WeatherConditions cond, WeatherConfig Opt)
        {
            LoadData(cond, Opt);
        }

        public string GetCurrentClimateName()
        {
            return Options.ClimateType;
        }

        public string GetCurrentWeatherStreak()
        {
            return CurrentConditions.trackerModel.CurrentStreak.CurrentWeather ?? "Unset";
        }

        public int GetNumDaysOfStreak()
        {
            return CurrentConditions.trackerModel.CurrentStreak.NumDaysOfWeather;
        }

        public long GetAmtOfRainSinceDay1()
        {
            return CurrentConditions.trackerModel.AmtOfRainSinceDay1;
        }

        public int GetDaysSinceRainedLast()
        {
            return CurrentConditions.trackerModel.DaysSinceRainedLast;
        }

        public int GetCurrentRain()
        {
            return CurrentConditions.AmtOfRainDrops;
        }

        public bool IsVariableRain()
        {
            return CurrentConditions.IsVariableRain;
        }

        public int GetCurrentRainTotal()
        {
            return CurrentConditions.TodayRain;
        }

        public string GetCurrentWeatherName()
        {
            return CurrentConditions.Weathers[(int)CurrentConditions.GetCurrentConditions()].ConditionName;
        }

        public double? GetTodaysHigh()
        {
            return CurrentConditions.TodayHigh;
        }

        public double? GetTodaysLow()
        {
            return CurrentConditions.TodayLow;
        }

    }
}
