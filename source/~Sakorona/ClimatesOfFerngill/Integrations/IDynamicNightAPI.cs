using StardewModdingAPI.Utilities;

namespace ClimatesOfFerngillRebuild.Integrations
{
    public interface IDynamicNightAPI
    {
        int GetSunriseTime();
        int GetSunsetTime();
        int GetSolarNoon();
        int GetLateAfternoon();
        int GetEarlyMorning();
        int GetAstroTwilightTime();
        int GetMorningAstroTwilightTime();
        int GetCivilTwilightTime();
        int GetMorningCivilTwilightTime();
        int GetNavalTwilightTime();
        int GetMorningNavalTwilightTime();
        int GetAnySunsetTime(SDate date);
        int GetAnySunriseTime(SDate date);
    }
}
