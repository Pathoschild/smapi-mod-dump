/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

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

        //specific functions
        string GetCurrentFogType();
    }
}
