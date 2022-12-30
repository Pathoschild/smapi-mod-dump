/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
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
    ///     Raised when storage data is requested for a storage type.
    /// </summary>
    public event EventHandler<IStorageTypeRequestedEventArgs> StorageTypeRequested;

    /// <summary>
    ///     Adds all applicable config options to an existing GMCM for this storage data.
    /// </summary>
    /// <param name="manifest">A manifest to describe the mod.</param>
    /// <param name="storage">The storage to configure for.</param>
    public void AddConfigOptions(IManifest manifest, IStorageData storage);
}