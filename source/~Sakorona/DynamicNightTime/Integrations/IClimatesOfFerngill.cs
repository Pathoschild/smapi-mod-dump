/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

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
