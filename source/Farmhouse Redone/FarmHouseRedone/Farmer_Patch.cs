using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Locations;
using Harmony;

namespace FarmHouseRedone
{
    class Farmer_Ctor_Patch
    {
        public static void Postfix(Farmer __instance)
        {
            Logger.Log("Running constructor patch...");
            Logger.Log("Current bed before patch is " + __instance.mostRecentBed.ToString());
            if (__instance.currentLocation != null && __instance.currentLocation is FarmHouse)
            {
                Logger.Log("Current location is a farmhouse; getting bed spot from this farmhouse.");
                __instance.mostRecentBed = StardewValley.Utility.PointToVector2(FarmHouseStates.getBedSpot(__instance.currentLocation as FarmHouse)) * 64f;
            }
            else
            {
                Logger.Log("Current location was not a farmhouse; getting bed spot from " + __instance.name + "'s farmhouse...");
                foreach (GameLocation location in Game1.locations)
                {
                    if (location is FarmHouse && (location as FarmHouse).owner == __instance)
                    {
                        Logger.Log("Found " + __instance.name + "'s farmhouse.");
                        __instance.mostRecentBed = StardewValley.Utility.PointToVector2(FarmHouseStates.getBedSpot(location as FarmHouse)) * 64f;
                    }
                }
            }
            Logger.Log("Most recent bed: " + __instance.mostRecentBed.ToString());
        }
    }
}