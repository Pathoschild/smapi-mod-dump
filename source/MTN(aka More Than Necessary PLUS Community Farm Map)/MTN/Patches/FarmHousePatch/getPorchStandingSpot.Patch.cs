using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;

namespace MTN.Patches.FarmHousePatch
{
    class getPorchStandingSpotPatch
    {
        public static bool Prefix(FarmHouse __instance)
        {
            if (Memory.isCustomFarmLoaded)
            {
                return false;
            }
            return true;
        }

        public static void Postfix(FarmHouse __instance, ref Point __result)
        {
            if (!Memory.isCustomFarmLoaded) return;

            int num = __instance.farmerNumberOfOwner;
            if (num == 0 || num == 1)
            {
                //__result = new Point(Memory.loadedFarm, Memory.getActiveFarmhouseYpoint());
                __result = Memory.loadedFarm.getFarmHousePorch();
                return;
            }
            __result = new Point(-1000, -1000);
        }
    }
}
