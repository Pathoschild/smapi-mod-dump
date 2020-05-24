using FarmAnimalVarietyRedux.Models;
using System.Collections.Generic;
using System.Linq;

namespace FarmAnimalVarietyRedux
{
    /// <summary>Provides basic FAVR apis.</summary>
    public class Api : IApi
    {
        /// <summary>Get all <see cref="AnimalData"/>.</summary>
        /// <returns>All <see cref="AnimalData"/>.</returns>
        public List<Animal> GetAllAnimals()
        {
            return ModEntry.Instance.Animals
                .ToList();
        }

        /// <summary>Get all the buyable <see cref="Animal"/>s.</summary>
        /// <returns>All the buyable <see cref="Animal"/>s.</returns>
        public List<Animal> GetAllBuyableAnimals()
        {
            return ModEntry.Instance.Animals
                .Where(animal => animal.Data.Buyable)
                .ToList();
        }

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
                if (animal.SubTypes.Where(subType => subType.Name.ToLower() == subTypeName.ToLower()).Any())
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
                var subType = animal.SubTypes.Where(subType => subType.Name.ToLower() == subTypeName.ToLower()).FirstOrDefault();
                if (subType != null)
                    return subType;
            }

            return null;
        }
    }
}
