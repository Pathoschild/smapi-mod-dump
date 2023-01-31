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
using StardewValley.TerrainFeatures;

namespace CatGiftsRedux.Framework.Pickers;

/// <summary>
/// Picks a random on-farm crop.
/// </summary>
internal static class OnFarmCropPicker
{
    /// <summary>
    /// Picks a random on-farm (living) crop.
    /// </summary>
    /// <param name="random">Random to use.</param>
    /// <returns>The product of a random crop.</returns>
    internal static Item? Pick(Random random)
    {
        ModEntry.ModMonitor.DebugOnlyLog("Picked Random Farm Crop");

        Farm? farm = Game1.getFarm();

        List<HoeDirt>? hoedirt = farm.terrainFeatures.Values.OfType<HoeDirt>().Where((dirt) => dirt.crop is not null && !dirt.crop.dead.Value).ToList();

        if (hoedirt.Count == 0)
        {
            return null;
        }

        int tries = 3;
        do
        {
            Crop? crop = Utility.GetRandom(hoedirt, random).crop;

            if (Utils.ForbiddenFromRandomPicking(crop.indexOfHarvest.Value))
            {
                continue;
            }

            if (DataToItemMap.IsActuallyRing(crop.indexOfHarvest.Value))
            {
                return new Ring(crop.indexOfHarvest.Value);
            }

            return crop.programColored.Value
                ? new ColoredObject(crop.indexOfHarvest.Value, 1, crop.tintColor.Value)
                : new SObject(crop.indexOfHarvest.Value, 1);
        }
        while (tries-- > 0);
        return null;
    }
}
