namespace TwilightShards.WeatherIllnesses.Integrations
{
    public interface IClimatesOfFerngillAPI
    {
        string GetCurrentWeatherName();
        double GetTodaysHigh();
        double GetTodaysLow();
    }
}
