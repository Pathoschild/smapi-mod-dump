/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Drachenkaetzchen/AdvancedKeyBindings
**
*************************************************/

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