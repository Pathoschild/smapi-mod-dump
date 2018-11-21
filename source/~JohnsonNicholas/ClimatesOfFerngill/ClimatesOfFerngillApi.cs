namespace ClimatesOfFerngillRebuild
{
    public interface IClimatesOfFerngillAPI
    {
        string GetCurrentWeatherName();
    }
    
    public class ClimatesOfFerngillAPI : IClimatesOfFerngillAPI
    {
        private WeatherConditions CurrentConditions;

        public void LoadData(WeatherConditions Cond) => CurrentConditions = Cond;

        public ClimatesOfFerngillAPI(WeatherConditions cond)
        {
            LoadData(cond);
        }

        public string GetCurrentWeatherName()
        {
            return CurrentConditions.Weathers[(int)CurrentConditions.GetCurrentConditions()].ConditionName;
        }

        public void SetWeather(string weather)
        {
            //TODO: Handle processing.
        }

        public double GetTodaysHigh()
        {
            return CurrentConditions.TodayHigh;
        }

        public double GetTodaysLow()
        {
            return CurrentConditions.TodayLow;
        }

    }
}
