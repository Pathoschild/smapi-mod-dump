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
using AtraShared.Wrappers;

using StardewValley.Objects;

namespace CatGiftsRedux.Framework.Pickers;

/// <summary>
/// Picks an appropriate seasonal fruit.
/// </summary>
internal static class SeasonalFruitPicker
{
    internal static Item? Pick(Random random)
    {
        ModEntry.ModMonitor.DebugOnlyLog("Picked Seasonal Fruit");

        List<KeyValuePair<int, string>>? fruittrees = Game1.content.Load<Dictionary<int, string>>(@"Data\fruitTrees")
                                      .Where((kvp) => kvp.Value.GetNthChunk('/', 1).Contains(Game1.currentSeason, StringComparison.OrdinalIgnoreCase))
                                      .ToList();

        if (fruittrees.Count == 0)
        {
            return null;
        }

        int tries = 3;
        do
        {
            KeyValuePair<int, string> fruit = fruittrees[random.Next(fruittrees.Count)];

            if (int.TryParse(fruit.Value.GetNthChunk('/', 2), out int id))
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
        while (tries-- > 3);
        return null;
    }
}
