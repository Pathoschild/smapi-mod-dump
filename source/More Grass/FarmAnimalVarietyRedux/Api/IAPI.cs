using FarmAnimalVarietyRedux.Models;
using System.Collections.Generic;

using SObject = StardewValley.Object;

namespace FarmAnimalVarietyRedux
{
    /// <summary>Provides basic FAVR apis.</summary>
    public interface IApi
    {
        /// <summary>Get all the <see cref="Animal"/>s.</summary>
        /// <returns>All <see cref="Animal"/>s.</returns>
        List<Animal> GetAllAnimals();

        /// <summary>Get all the <see cref="Animal"/>s as a list of <see cref="StardewValley.Object"/>.</summary>
        /// <returns>All the buyable <see cref="Animal"/>s as a list of <see cref="StardewValley.Object"/>.</returns>
        List<SObject> GetAllAnimalObjects();

        /// <summary>Get all the buyable <see cref="Animal"/>s as a list of <see cref="StardewValley.Object"/>.</summary>
        /// <returns>All the buyable <see cref="Animal"/>s as a lsit of <see cref="StardewValley.Object"/>.</returns>
        List<SObject> GetAllBuyableAnimalObjects();

        /// <summary>Get a <see cref="Animal"/> by animal name.</summary>
        /// <param name="animalName">The name of the animal to get data for.</param>
        /// <returns>The <see cref="Animal"/> with the passed name.</returns>
        Animal GetAnimalByName(string animalName);

        /// <summary>Get a <see cref="Animal"/> by animal sub type name.</summary>
        /// <param name="subTypeName">The name of a sub type of the animal to get data for.</param>
        /// <returns>The <see cref="Animal"/> with a sub tyoe with the passed name.</returns>
        Animal GetAnimalBySubTypeName(string subTypeName);

        /// <summary>Get a <see cref="AnimalSubType"/> by animal sub type name.</summary>
        /// <param name="subTypeName">The name of the sub type of the animal to get data for.</param>
        /// <returns>The <see cref="AnimalSubType"/> with the passed name.</returns>
        AnimalSubType GetAnimalSubTypeByName(string subTypeName);
    }
}
