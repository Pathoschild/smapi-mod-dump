/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework;

using System;
using StardewMods.Common.Integrations.BetterChests;

/// <inheritdoc />
public sealed class Api : IBetterChestsApi
{
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