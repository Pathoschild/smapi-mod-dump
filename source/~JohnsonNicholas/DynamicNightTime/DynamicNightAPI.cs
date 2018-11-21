using TwilightShards.Stardew.Common;

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
    }

    public class DynamicNightAPI : IDynamicNightAPI
    {
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
