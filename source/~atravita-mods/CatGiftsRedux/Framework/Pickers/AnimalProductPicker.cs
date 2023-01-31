/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Extensions;

using AtraCore.Framework.ItemManagement;

using AtraShared.Utils.Extensions;

using StardewValley.Objects;

namespace CatGiftsRedux.Framework.Pickers;

/// <summary>
/// Tries to pick a random animal product.
/// </summary>
internal static class AnimalProductPicker
{
    /// <summary>
    /// Pick a random animal product.
    /// </summary>
    /// <param name="random">The seeded random.</param>
    /// <returns>An item.</returns>
    internal static Item? Pick(Random random)
    {
        ModEntry.ModMonitor.DebugOnlyLog("Picked Animal Products");

        Dictionary<string, string>? content = Game1.content.Load<Dictionary<string, string>>("Data\\FarmAnimals");
        if (content.Count == 0)
        {
            return null;
        }

        int tries = 3;
        do
        {
            KeyValuePair<string, string> randomAnimal = content.ElementAt(random.Next(content.Count));
            if (int.TryParse(randomAnimal.Value.GetNthChunk('/', 2), out int id) && id > 0)
            {
                if (Utils.ForbiddenFromRandomPicking(id))
                {
                    continue;
                }

                if (DataToItemMap.IsActuallyRing(id))
                {
                    return new Ring(id);
                }

                return new SObject(id, 1);
            }
        }
        while (tries-- > 0);
        return null;
    }
}
