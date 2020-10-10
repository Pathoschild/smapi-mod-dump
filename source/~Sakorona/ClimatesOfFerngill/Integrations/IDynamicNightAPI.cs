/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

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
