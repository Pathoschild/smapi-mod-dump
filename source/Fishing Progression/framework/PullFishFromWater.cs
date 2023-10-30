/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chudders1231/SDV-FishingProgression
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;
using StardewModdingAPI;
using System.Runtime.CompilerServices;

namespace FishingProgression.framework
{
    internal class PullFishFromWater
    {

        private static IMonitor Monitor;

        // call this method from your Entry class
        internal static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        // patches need to be static!
        internal static bool pullFishFromWater_Prefix(FishingRod __instance, ref bool caughtDouble)
        {
            try
            {
                double rnd = Game1.random.Next(0, 100);
                float chance = Math.Clamp(__instance.getLastFarmerToUse().FishingLevel * Globals.Config.DoubleHookChance, 0, 100);

                Log.Debug($"RND: {rnd} - Chance: {chance} - FL: {__instance.getLastFarmerToUse().FishingLevel}");

                if (Globals.Config.EnableDoubleHook && rnd <= chance)
                {
                    caughtDouble = true;
                }   
                return true; // don't run original logic
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(pullFishFromWater_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
