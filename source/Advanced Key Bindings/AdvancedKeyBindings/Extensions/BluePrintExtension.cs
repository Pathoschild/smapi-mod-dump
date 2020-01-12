using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Buildings;

namespace AdvancedKeyBindings.Extensions
{
    public static class BluePrintExtension
    {
        public static List<Building> GetUpgradeableBuildings(this BluePrint bluePrint)
        {
            var upgradableBuildings = new List<Building>();
            
            foreach (var building in Game1.getFarm().buildings)
            {
                if (bluePrint.nameOfBuildingToUpgrade != null&&bluePrint.nameOfBuildingToUpgrade.Equals(building.buildingType))
                {
                    upgradableBuildings.Add(building);

                }
            }

            return upgradableBuildings;
        }
    }
}