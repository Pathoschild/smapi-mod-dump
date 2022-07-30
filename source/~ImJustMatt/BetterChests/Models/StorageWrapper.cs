/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Models;

using StardewMods.Common.Integrations.BetterChests;

/// <summary>
///     A wrapper for a Storage Object.
/// </summary>
internal class StorageWrapper
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="StorageWrapper" /> class.
    /// </summary>
    /// <param name="storage">The storage object.</param>
    public StorageWrapper(IStorageObject storage)
    {
        this.Storage = storage;
    }

    /// <summary>
    ///     Gets the storage object.
    /// </summary>
    public IStorageObject Storage { get; }
}