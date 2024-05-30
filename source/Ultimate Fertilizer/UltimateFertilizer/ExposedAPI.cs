/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/foxwhite25/Stardew-Ultimate-Fertilizer
**
*************************************************/

using StardewValley;
using StardewValley.TerrainFeatures;

namespace UltimateFertilizer;

public class ExposedApi : IUltimateFertilizerApi {
    public bool ApplyFertilizerOnDirt(HoeDirt dirt, string itemId, Farmer who) {
        return ModEntry.Plant.ApplyFertilizerOnDirt(dirt, itemId, who);
    }

    public bool IsFertilizerApplied(HoeDirt dirt, string itemId) {
        return dirt.fertilizer.Value.Contains(itemId);
    }

    public void RegisterFertilizerType(IEnumerable<string> itemIds) {
        ModEntry.Fertilizers.Add(itemIds.ToList());
    }
}