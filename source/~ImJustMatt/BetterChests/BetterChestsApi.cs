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
using System.Collections.Generic;
using StardewModdingAPI;
using StardewMods.BetterChests.Helpers;
using StardewMods.Common.Integrations.BetterChests;

/// <inheritdoc />
public class BetterChestsApi : IBetterChestsApi
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BetterChestsApi" /> class.
    /// </summary>
    /// <param name="storageTypes">A dictionary of all registered storage types.</param>
    public BetterChestsApi(Dictionary<Func<object, bool>, IStorageData> storageTypes)
    {
        this.StorageTypes = storageTypes;
    }

    private Dictionary<Func<object, bool>, IStorageData> StorageTypes { get; }

    /// <inheritdoc />
    public void AddConfigOptions(IManifest manifest, IStorageData storage)
    {
        ConfigHelper.SetupSpecificConfig(manifest, storage);
    }

    /// <inheritdoc />
    public void RegisterChest(Func<object, bool> predicate, IStorageData storage)
    {
        this.StorageTypes[predicate] = storage;
    }
}