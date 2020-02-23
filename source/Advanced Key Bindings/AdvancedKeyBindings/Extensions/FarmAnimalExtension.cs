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