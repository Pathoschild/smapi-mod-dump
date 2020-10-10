/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/StardewValleyModding
**
*************************************************/

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
