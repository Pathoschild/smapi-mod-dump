/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

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

        //specific functions
        string GetCurrentFogType();
    }
    
    public class ClimatesOfFerngillAPI : IClimatesOfFerngillAPI
    {
        private WeatherConditions CurrentConditions;
        private WeatherConfig Options;

        public void LoadData(WeatherConditions cond, WeatherConfig opt)
        {
            CurrentConditions = cond;
            Options = opt;
        }

        public ClimatesOfFerngillAPI(WeatherConditions cond, WeatherConfig opt)
        {
            LoadData(cond, opt);
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

        public string GetCurrentFogType()
        {
            return CurrentConditions.GetCurrentFogTypeDesc();
        }
    }
}
