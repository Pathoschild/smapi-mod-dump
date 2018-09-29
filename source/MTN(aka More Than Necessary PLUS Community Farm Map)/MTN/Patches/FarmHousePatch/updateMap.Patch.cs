using Harmony;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.Patches.FarmHousePatch {
    public class updateMapPatch {
        public static void Postfix(FarmHouse __instance) {
            if (!Memory.isCustomFarmLoaded) return;

            if (__instance is Cabin) {

            } else {
                GameLocation theMap = Game1.getLocationFromName("FarmHouse");
                int farmHouseExitX = 0;
                int farmHouseExitY = 0;
                theMap.warps.Clear();
                switch (Game1.MasterPlayer.houseUpgradeLevel) {
                    case 0:
                        farmHouseExitX = 3;
                        farmHouseExitY = 12;
                        break;
                    case 1:
                        farmHouseExitX = 9;
                        farmHouseExitY = 12;
                        break;
                    case 2:
                    case 3:
                        farmHouseExitX = 12;
                        farmHouseExitY = 21;
                        break;
                }
                theMap.warps.Add(new Warp(farmHouseExitX, farmHouseExitY, "Farm", Memory.loadedFarm.farmHousePorchX(), Memory.loadedFarm.farmHousePorchY(), false));
                if (Game1.MasterPlayer.houseUpgradeLevel == 3) {
                    theMap.warps.Add(new Warp(4, 25, "Cellar", 3, 2, false));
                    theMap.warps.Add(new Warp(5, 25, "Cellar", 4, 2, false));
                }
            }
        }
    }
}
