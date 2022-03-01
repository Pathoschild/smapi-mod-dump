/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.FuryCore.Interfaces.GameObjects;

/// <summary>
///     Represents a terrain feature that is interactable.
/// </summary>
public interface ITerrainFeature : IGameObject
{
    /// <summary>
    ///     Checks if the terrain feature is ready for harvesting.
    /// </summary>
    /// <returns>True if the terrain can be harvested.</returns>
    bool CanHarvest();

    /// <summary>
    ///     Attempts to drop an item from this terrain feature.
    /// </summary>
    /// <returns>True if the item was dropped.</returns>
    bool TryHarvest();
}