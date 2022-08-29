/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.BetterChests;

using System;

/// <summary>
///     API for Better Chests.
/// </summary>
public interface IBetterChestsApi
{
    /// <summary>
    ///     Adds all applicable config options to an existing GMCM for this storage data.
    /// </summary>
    /// <param name="manifest">A manifest to describe the mod.</param>
    /// <param name="storage">The storage to configure for.</param>
    public void AddConfigOptions(IManifest manifest, IStorageData storage);

    /// <summary>
    ///     Registers a chest type based on any object containing the mod data key-value pair.
    /// </summary>
    /// <param name="predicate">A function which returns true for valid storages.</param>
    /// <param name="storage">The storage data.</param>
    public void RegisterChest(Func<object, bool> predicate, IStorageData storage);
}