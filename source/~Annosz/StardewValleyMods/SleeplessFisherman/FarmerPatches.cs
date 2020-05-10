using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleeplessFisherman
{
    class FarmerPatches
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static bool doEmote_Prefix(Farmer __instance, int whichEmote)
        {
            try
            {
                if ((__instance is Farmer) && ((__instance as Farmer).CurrentTool is FishingRod)
                    && ((__instance as Farmer).CurrentTool as FishingRod).isFishing && whichEmote == 24)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(doEmote_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
    }
}
