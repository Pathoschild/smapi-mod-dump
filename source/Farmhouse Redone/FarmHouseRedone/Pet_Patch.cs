using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;

namespace FarmHouseRedone
{
    class Pet_warpToFarmHouse_Patch
    {
        public static void Postfix(Farmer who, Pet __instance)
        {
            if (__instance.isSleepingOnFarmerBed && __instance.currentLocation is FarmHouse)
            {
                __instance.position.Value = (StardewValley.Utility.PointToVector2(FarmHouseStates.getBedSpot(StardewValley.Utility.getHomeOfFarmer(who))) + new Microsoft.Xna.Framework.Vector2(-1f, 0f)) * 64f;
            }
        }
    }
}
