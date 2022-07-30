/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

#nullable disable

namespace HelpForHire.Chores;

using System.Collections.Generic;
using System.Linq;
using StardewValley;

internal class PetAnimals : GenericChore
{
    public PetAnimals(ServiceLocator serviceLocator)
        : base("pet-animals", serviceLocator)
    {
    }

    protected override bool DoChore()
    {
        var animalsPetted = false;

        foreach (var farmAnimal in PetAnimals.GetFarmAnimals())
        {
            farmAnimal.pet(Game1.player);
            animalsPetted = true;
        }

        return animalsPetted;
    }

    protected override bool TestChore()
    {
        return PetAnimals.GetFarmAnimals().Any();
    }

    private static IEnumerable<FarmAnimal> GetFarmAnimals()
    {
        return
            from farmAnimal in Game1.getFarm().getAllFarmAnimals()
            where !farmAnimal.wasPet.Value
            select farmAnimal;
    }
}