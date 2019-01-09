using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.FarmHousePatches
{
    public class updateMapPatch
    {
        private static CustomFarmManager farmManager;

        public updateMapPatch(CustomFarmManager farmManager) {
            updateMapPatch.farmManager = farmManager;
        }

        public static void Postfix(FarmHouse __instance) {
            if (farmManager.Canon) return;

            if (__instance is Cabin) {
                // TO DO
            } else {
                int X = 0;
                int Y = 0;
                __instance.warps.Clear();
                switch (Game1.MasterPlayer.houseUpgradeLevel) {
                    case 0:
                        X = 3;
                        Y = 12;
                        break;
                    case 1:
                        X = 9;
                        Y = 12;
                        break;
                    case 2:
                    case 3:
                        X = 12;
                        Y = 21;
                        __instance.warps.Add(new Warp(4, 25, "Cellar", 3, 2, false));
                        __instance.warps.Add(new Warp(5, 25, "Cellar", 4, 2, false));
                        break;
                }
                __instance.warps.Add(new Warp(X, Y, "Farm", farmManager.FarmHousePorch.X, farmManager.FarmHousePorch.Y, false));
            }
        }
    }
}
