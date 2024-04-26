/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using StardewValley;

namespace StardewWebApi.Game.Animals;

public static class AnimalUtilities
{
    public static FarmAnimal? GetFarmAnimalByName(string name)
    {
        var nameLower = name.ToLower();
        FarmAnimal? foundAnimal = null;

        Utility.ForEachLocation((location) =>
        {
            foreach (var animal in location.animals.Values)
            {
                if (animal.Name.ToLower() == nameLower)
                {
                    foundAnimal = animal;
                    return false;
                }
            }
            return true;
        });

        return foundAnimal;
    }
}