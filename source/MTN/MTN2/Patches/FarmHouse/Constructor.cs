using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.FarmHousePatches
{
    public class ConstructorFarmHousePatch
    {
        private static CustomFarmManager farmManager;
        private static int SwappedID;

        public ConstructorFarmHousePatch(CustomFarmManager farmManager) {
            ConstructorFarmHousePatch.farmManager = farmManager;
            SwappedID = -1;
        }

        public static void Prefix() {
            if (!farmManager.Canon && farmManager.FurnitureLayout < 5 && farmManager.FurnitureLayout >= 0) {
                SwappedID = Game1.whichFarm;
                Game1.whichFarm = farmManager.FurnitureLayout;
            }
        }

        public static void Postfix(FarmHouse __instance) {
            if (farmManager.Canon) return;

            if (SwappedID != -1) {
                Game1.whichFarm = SwappedID;
                SwappedID = -1;
            }

            if (farmManager.LoadedFarm.FurnitureList != null) {
                foreach (Furniture f in farmManager.LoadedFarm.FurnitureList) {
                    __instance.furniture.Add(f);
                }
            }

            if (farmManager.LoadedFarm.ObjectList != null) {
                foreach (StardewValley.Object o in farmManager.LoadedFarm.ObjectList) {
                    //__instance.objects.Add(o);
                }
            }
        }
    }
}
