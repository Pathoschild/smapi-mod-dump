using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using TwilightShards.Common;
using TwilightShards.Stardew.Common;

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
