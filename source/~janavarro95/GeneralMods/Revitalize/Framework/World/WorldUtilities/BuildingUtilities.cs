/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Buildings;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities
{
    /// <summary>
    /// Utilities dealing with buildings.
    /// </summary>
    public class BuildingUtilities
    {

        /// <summary>
        /// Checks to see if a tier 2 or higher barn or coop has been built. Used for unlocking stuff like the Hay Maker.
        /// </summary>
        /// <returns></returns>
        public static bool HasBuiltTier2OrHigherBarnOrCoop()
        {
            bool hasBuiltBuilding = IsAnyBuildingWithBuildingTypeConstructedOnTheFarm(new List<string>()
            {
                "Big Barn", //Big barn
                "Deluxe Barn", // Deluxe barn.
                "Big Coop", //Big coop
                "Deluxe Coop" //Deluxe coop.
            });
            if (hasBuiltBuilding == true && RevitalizeModCore.SaveDataManager.shopSaveData.animalShopSaveData.getHasBuiltTier2OrHigherBarnOrCoop() == false)
            {
                RevitalizeModCore.SaveDataManager.shopSaveData.animalShopSaveData.setHasBuiltTier2OrHigherBarnOrCoop();
            }
            return hasBuiltBuilding;
        }

        /// <summary>
        /// Checks to see if a given building type has been constructed on the farm.
        /// </summary>
        /// <param name="BuildingType"></param>
        /// <returns></returns>
        public static bool isBuildingTypeConstructedOnTheFarm(string BuildingType)
        {
            foreach (Building building in Game1.getFarm().buildings)
            {
                if (building.buildingType.Value.Equals(BuildingType))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Checks to see if at least one building on the farm with the given building type has been constructed. This is the english name on the wiki.
        /// </summary>
        /// <param name="buildingTypes"></param>
        /// <returns></returns>
        public static bool IsAnyBuildingWithBuildingTypeConstructedOnTheFarm(List<string> buildingTypes)
        {
            foreach (Building building in Game1.getFarm().buildings)
            {
                if (buildingTypes.Contains(building.buildingType.Value))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
