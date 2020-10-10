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
    public static class FarmAnimalExtension
    {
        /// <summary>
        /// Gets a list of possible home buildings for this animal. Note that buildings which are full are
        /// excluded from the list 
        /// </summary>
        /// <param name="animal">The animal</param>
        /// <returns>A list of possible home buildings.</returns>
        public static List<Building> GetPossibleHomeBuildings(this FarmAnimal animal)
        {
            return Game1.getFarm().buildings.Where(building =>
                building.buildingType.Contains(animal.buildingTypeILiveIn) &&
                !((AnimalHouse) building.indoors.Value).isFull() &&
                !(building.daysOfConstructionLeft.Value > 0)).ToList();
        }
    }
}