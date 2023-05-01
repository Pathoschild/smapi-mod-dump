/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using StardewValley.TerrainFeatures;

namespace AtraShared.Integrations.Interfaces;

/// <summary>
/// The bits of the GrowableGiantCrops API I use.
/// Copy from https://github.com/atravita-mods/StardewMods/blob/main/GrowableGiantCrops/IGrowableGiantCropsAPI.cs 
/// if you want the full thing.
/// </summary>
public interface IGrowableGiantCrops
{
    /// <summary>
    /// Given a grass starter, returns the matching grass instance.
    /// </summary>
    /// <param name="starter">Grass starter.</param>
    /// <returns>Grass.</returns>
    public Grass? GetMatchingGrass(SObject starter);
}
