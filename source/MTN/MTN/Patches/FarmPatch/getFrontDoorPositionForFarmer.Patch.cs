using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.Patches.FarmPatch
{
    //[HarmonyPatch(typeof(Farm))]
    //[HarmonyPatch("getFrontDoorPositionForFarmer")]
    class getFrontDoorPositionForFarmerPatch
    {
        static void Postfix(Farm __instance, ref Point __result)
        {
            if (Game1.whichFarm > 4)
            {
                __result = Memory.loadedFarm.getFarmHousePorch();
            }
        }
    }
}
