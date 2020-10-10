/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using StardewValley;

namespace DynamicNightTime.Patches
{
    class IsDarkOutPatch
    {
        public static void Postfix(ref bool __result)
        {
            bool IsBeforeSunrise = Game1.timeOfDay < DynamicNightTime.GetSunriseTime();
            bool IsPastSunset = Game1.timeOfDay > Game1.getModeratelyDarkTime();

            __result = ((IsBeforeSunrise) || (IsPastSunset));
        }
    }
}
