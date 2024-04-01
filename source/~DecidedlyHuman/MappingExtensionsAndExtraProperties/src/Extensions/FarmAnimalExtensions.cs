/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace MappingExtensionsAndExtraProperties.Extensions;

public static class FarmAnimalExtensions
{
    public static bool IsMeepFarmAnimal(this FarmAnimal animal)
    {
        return animal.Name.StartsWith("DH.MEEP.SpawnedAnimal_");
    }
}
