/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests;

using System;
using StardewMods.BetterChests.Framework;
using StardewMods.Common.Integrations.BetterChests;

/// <inheritdoc />
public sealed class BetterChestsApi : IBetterChestsApi
{
    private readonly IStorageData _default;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BetterChestsApi" /> class.
    /// </summary>
    /// <param name="defaultChest">Default data for any storage.</param>
    public BetterChestsApi(IStorageData defaultChest)
    {
        this._default = defaultChest;
    }

    /// <inheritdoc />
    public event EventHandler<IStorageTypeRequestedEventArgs>? StorageTypeRequested
    {
        add => Storages.StorageTypeRequested += value;
        remove => Storages.StorageTypeRequested -= value;
    }

    /// <inheritdoc />
    public void AddConfigOptions(IManifest manifest, IStorageData storage)
    {
        Config.SetupSpecificConfig(manifest, storage);
    }
}