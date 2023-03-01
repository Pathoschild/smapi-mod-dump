/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace GiantCropFertilizer;

/// <summary>
/// Holds useful and extension methods for this mod.
/// </summary>
internal static class GCFUtils
{
    /// <summary>
    /// Fixes the Giant Crop Fertilizer's ID on a specific map.
    /// </summary>
    /// <param name="loc">Map to fix.</param>
    /// <param name="storedID">The old id.</param>
    /// <param name="newID">The new id.</param>
    internal static void FixIDsInLocation(this GameLocation? loc, int storedID, int newID)
    {
        if (loc is null)
        {
            return;
        }

        foreach (TerrainFeature terrainfeature in loc.terrainFeatures.Values)
        {
            if (terrainfeature is HoeDirt dirt && dirt.fertilizer.Value == storedID)
            {
                dirt.fertilizer.Value = newID;
            }
        }
        foreach (SObject obj in loc.Objects.Values)
        {
            if (obj is IndoorPot pot && pot.hoeDirt?.Value?.fertilizer?.Value == storedID)
            {
                pot.hoeDirt.Value.fertilizer.Value = newID;
            }
        }
    }
}
