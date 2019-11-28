namespace TwilightShards.WeatherIllnesses.Integrations
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
}
