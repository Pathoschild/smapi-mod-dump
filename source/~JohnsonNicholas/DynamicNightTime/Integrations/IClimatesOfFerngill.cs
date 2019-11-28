namespace DynamicNightTime.Integrations
{ 
    public interface IClimatesOfFerngillAPI
    {
        string GetCurrentWeatherName();
        double? GetTodaysHigh();
        double? GetTodaysLow();
        string GetCurrentClimateName();
        bool IsVariableRain();
        int GetCurrentRainTotal();
        long GetAmtOfRainSinceDay1();
        int GetDaysSinceRainedLast();
        int GetCurrentRain();
        string GetCurrentWeatherStreak();
        int GetNumDaysOfStreak();
    }
}
