using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.EventPatches
{
    public class setExitLocationPatch
    {
        private static CustomFarmManager farmManager;

        public setExitLocationPatch(CustomFarmManager farmManager) {
            setExitLocationPatch.farmManager = farmManager;
        }

        public static void Prefix(Event __instance, string location, ref int x, ref int y) {
            if (!farmManager.Canon && location == "Farm") {
                x = farmManager.FarmHousePorch.X + 2 - Utility.getFarmerNumberFromFarmer(Game1.player);
                y = farmManager.FarmHousePorch.Y;
            }
        }
    }
}
