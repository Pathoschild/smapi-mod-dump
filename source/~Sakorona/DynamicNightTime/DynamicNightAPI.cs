using StardewModdingAPI.Utilities;

namespace DynamicNightTime
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

    public class DynamicNightAPI : IDynamicNightAPI
    {
        public int GetAnySunsetTime(SDate date) => DynamicNightTime.GetSunsetForDay(date).ReturnIntTime();
        public int GetAnySunriseTime(SDate date) => DynamicNightTime.GetSunriseForDay(date).ReturnIntTime();
        public int GetSunriseTime() => DynamicNightTime.GetSunriseTime();
        public int GetSunsetTime() => DynamicNightTime.GetSunset().ReturnIntTime();
        public int GetAstroTwilightTime() => DynamicNightTime.GetAstroTwilight().ReturnIntTime();
        public int GetMorningAstroTwilightTime() => DynamicNightTime.GetMorningAstroTwilight().ReturnIntTime();
        public int GetCivilTwilightTime() => DynamicNightTime.GetCivilTwilight().ReturnIntTime();
        public int GetMorningCivilTwilightTime() => DynamicNightTime.GetMorningCivilTwilight().ReturnIntTime();
        public int GetNavalTwilightTime() => DynamicNightTime.GetNavalTwilight().ReturnIntTime();
        public int GetMorningNavalTwilightTime() => DynamicNightTime.GetMorningNavalTwilight().ReturnIntTime();
        public int GetSolarNoon() => DynamicNightTime.GetSolarNoon().ReturnIntTime();
        public int GetLateAfternoon() => DynamicNightTime.GetBeginningOfLateAfternoon().ReturnIntTime();
        public int GetEarlyMorning() => DynamicNightTime.GetEndOfEarlyMorning().ReturnIntTime();
    }
}
