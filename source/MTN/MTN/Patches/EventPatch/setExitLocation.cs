using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using StardewValley;

namespace MTN.Patches.EventPatch
{
    class setExitLocationPatch
    {
        public static bool Prefix(Event __instance, string location, ref int x, ref int y)
        {
            if (Memory.isCustomFarmLoaded && location == "Farm") {
                x = Memory.loadedFarm.farmHousePorchX() + 2 - Utility.getFarmerNumberFromFarmer(Game1.player);
                y = Memory.loadedFarm.farmHousePorchY();
            }
            return true;
        }
    }
}
