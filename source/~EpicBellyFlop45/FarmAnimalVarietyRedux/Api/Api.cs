using FarmAnimalVarietyRedux.Models;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

using SObject = StardewValley.Object;

namespace FarmAnimalVarietyRedux
{
    /// <summary>Provides basic FAVR apis.</summary>
    public class Api : IApi
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Get all <see cref="AnimalData"/>.</summary>
        /// <returns>All <see cref="AnimalData"/>.</returns>
        public List<Animal> GetAllAnimals()
        {
            return ModEntry.Instance.Animals
                .ToList();
        }

        /// <summary>Get all the <see cref="Animal"/>s as a list of <see cref="StardewValley.Object"/>.</summary>
        /// <returns>All the buyable <see cref="Animal"/>s as a list of <see cref="StardewValley.Object"/>.</returns>
        public List<SObject> GetAllAnimalObjects() => ConvertAnimalsToObjects(GetAllAnimals()).ToList();

        /// <summary>Get all the buyable <see cref="Animal"/>s as a list of <see cref="StardewValley.Object"/>.</summary>
        /// <returns>All the buyable <see cref="Animal"/>s as a lsit of <see cref="StardewValley.Object"/>.</returns>
        public List<SObject> GetAllBuyableAnimalObjects() => ConvertAnimalsToObjects(GetAllAnimals().Where(animal => animal.Data.Buyable)).ToList();

        /// <summary>Get a <see cref="AnimalData"/> by animal name.</summary>
        /// <param name="animalName">The name of the animal to get data for.</param>
        /// <returns>The <see cref="AnimalData"/> with the passed name.</returns>
        public Animal GetAnimalByName(string animalName)
        {
            return ModEntry.Instance.Animals
                .Where(animal => animal.Name.ToLower() == animalName.ToLower())
                .FirstOrDefault();
        }

        /// <summary>Get a <see cref="AnimalData"/> by animal sub type name.</summary>
        /// <param name="subTypeName">The name of a sub type of the animal to get data for.</param>
        /// <returns>The <see cref="AnimalData"/> with a sub tyoe with the passed name.</returns>
        public Animal GetAnimalBySubTypeName(string subTypeName)
        {
            foreach (var animal in ModEntry.Instance.Animals)
            {
                if (animal.Data.Types.Where(subType => subType.Name.ToLower() == subTypeName.ToLower()).Any())
                    return animal;
            }

            return null;
        }

        /// <summary>Get a <see cref="AnimalSubType"/> by animal sub type name.</summary>
        /// <param name="subTypeName">The name of the sub type of the animal to get data for.</param>
        /// <returns>The <see cref="AnimalSubType"/> with the passed name.</returns>
        public AnimalSubType GetAnimalSubTypeByName(string subTypeName)
        {
            foreach (var animal in ModEntry.Instance.Animals)
            {
                var subType = animal.Data.Types.Where(subType => subType.Name.ToLower() == subTypeName.ToLower()).FirstOrDefault();
                if (subType != null)
                    return subType;
            }

            return null;
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Converts a list of <see cref="Animal"/> to a list of <see cref="StardewValley.Object"/>.</summary>
        /// <param name="animals">The animals to convert.</param>
        /// <returns>A list of <see cref="StardewValley.Object"/> representing the passed list of <see cref="Animal"/>.</returns>
        private IEnumerable<SObject> ConvertAnimalsToObjects(IEnumerable<Animal> animals)
        {
            foreach (var animal in animals)
            {
                if (animal.Data.AnimalShopInfo == null)
                    continue;

                SObject animalObject = new SObject(100, 1, false, animal.Data.AnimalShopInfo.BuyPrice, 0);
                animalObject.Name = animal.Name;
                animalObject.displayName = animal.Name;

                // check if animal has valid home
                bool hasValidHome = false;
                for (int i = 0; i < animal.Data.Buildings.Count; i++)
                {
                    var building = animal.Data.Buildings[i];
                    if (Game1.getFarm().isBuildingConstructed(building))
                    {
                        hasValidHome = true;
                        break;
                    }
                }

                if (!hasValidHome)
                    animalObject.Type = $"Requires construction of at least one of the following buildings: {string.Join(", ", animal.Data.Buildings)}";
                else
                    animalObject.Type = null;

                yield return animalObject;
            }
        }
    }
}
