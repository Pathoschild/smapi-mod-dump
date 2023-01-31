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

using AtraShared.Caching;
using AtraShared.ConstantsAndEnums;
using AtraShared.Wrappers;

using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace MoreFertilizers;

/// <summary>
/// Generalized utilities for this mod.
/// </summary>
internal static class MFUtilities
{
    /// <summary>
    /// Gets a random fertilizer taking into account the player's level.
    /// </summary>
    /// <param name="level">Int skill level.</param>
    /// <returns>Fertilizer ID (-1 if not found).</returns>
    internal static int GetRandomFertilizerFromLevel(this int level)
        => Game1.random.Next(Math.Clamp((int)(level * 1.5) + 1, 0, 16)) switch
            {
                0 => ModEntry.LuckyFertilizerID,
                1 => ModEntry.JojaFertilizerID,
                2 => ModEntry.PaddyCropFertilizerID,
                3 => ModEntry.OrganicFertilizerID,
                4 => ModEntry.FruitTreeFertilizerID,
                5 => ModEntry.SeedyFertilizerID,
                6 => ModEntry.FishFoodID,
                7 => ModEntry.RadioactiveFertilizerID,
                8 => ModEntry.DeluxeFishFoodID,
                9 => ModEntry.DomesticatedFishFoodID,
                10 => ModEntry.DeluxeJojaFertilizerID,
                11 => ModEntry.DeluxeFruitTreeFertilizerID,
                12 => ModEntry.TreeTapperFertilizerID,
                13 => ModEntry.EverlastingFertilizerID,
                14 => ModEntry.MiraculousBeveragesID,
                15 => ModEntry.BountifulBushID,
                _ => ModEntry.BountifulFertilizerID,
            };

    /// <summary>
    /// Whether HoeDirt contains a crop should be considered a Joja crop for the Joja and Organic fertilizers.
    /// </summary>
    /// <param name="dirt">HoeDirt.</param>
    /// <returns>True if the HoeDirt has a joja crop.</returns>
    internal static bool HasJojaCrop(this HoeDirt dirt)
        => dirt?.crop?.IsJojaCrop() == true;

    /// <summary>
    /// Whether the crop should be considered a Joja crop for the Joja and Organic fertilizers.
    /// </summary>
    /// <param name="crop">crop.</param>
    /// <returns>True if crop is a joja crop.</returns>
    internal static bool IsJojaCrop(this Crop crop)
    {
        if (crop?.indexOfHarvest?.Value is null)
        {
            return false;
        }

        string data = Game1Wrappers.ObjectInfo[crop.indexOfHarvest.Value];
        int index = data.IndexOf('/');
        if (index >= 0)
        {
            ReadOnlySpan<char> span = data.AsSpan(0, index).Trim();
            return span.Contains("Joja", StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }

    /// <summary>
    /// Fixes IDs for all HoeDirt in a specific location.
    /// Given the idMapping.
    /// </summary>
    /// <param name="loc">Location to fix.</param>
    /// <param name="idMapping">IDMapping to use.</param>
    internal static void FixHoeDirtInLocation(this GameLocation loc, Dictionary<int, int> idMapping)
    {
        foreach (TerrainFeature terrain in loc.terrainFeatures.Values)
        {
            if (terrain is HoeDirt dirt && dirt.fertilizer.Value != 0)
            {
                if (idMapping.TryGetValue(dirt.fertilizer.Value, out int newval))
                {
                    dirt.fertilizer.Value = newval;
                }
            }
        }
        foreach (SObject obj in loc.Objects.Values)
        {
            if (obj is IndoorPot pot && pot.hoeDirt?.Value?.fertilizer?.Value is int value && value != 0)
            {
                if (idMapping.TryGetValue(value, out int newvalue))
                {
                    pot.hoeDirt.Value.fertilizer.Value = newvalue;
                }
            }
        }
    }

    /// <summary>
    /// Returns the id and type of an SObject, or null if not found.
    /// </summary>
    /// <param name="identifier">string identifier.</param>
    /// <returns>id/type tuple, or null for not found.</returns>
    internal static int? ResolveID(string identifier)
    {
        if (!int.TryParse(identifier, out int id))
        {
            id = DataToItemMap.GetID(ItemTypeEnum.SObject, identifier);
        }

        if (id < -1 || !Game1Wrappers.ObjectInfo.TryGetValue(id, out string? data))
        {
            ModEntry.ModMonitor.Log($"{identifier} could not be resolved, skipping");
            return null;
        }

        ReadOnlySpan<char> cat = data.GetNthChunk('/', SObject.objectInfoTypeIndex);
        int index = cat.GetIndexOfWhiteSpace();
        if (index < 0 || !int.TryParse(cat[(index + 1)..], out int type) || type is not SObject.SeedsCategory)
        {
            ModEntry.ModMonitor.Log($"{identifier} with {id} does not appear to be a seed, skipping.");
            return null;
        }

        return id;
    }
}