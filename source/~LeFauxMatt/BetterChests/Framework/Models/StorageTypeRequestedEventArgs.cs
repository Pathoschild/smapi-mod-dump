/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Models;

using System.Collections.Generic;
using StardewMods.Common.Integrations.BetterChests;

/// <inheritdoc />
internal sealed class StorageTypeRequestedEventArgs : IStorageTypeRequestedEventArgs
{
    private readonly List<KeyValuePair<IStorageData, int>> _prioritizedTypes = new();
    private readonly IList<IStorageData> _storageTypes;

    /// <summary>
    ///     Initializes a new instance of the <see cref="StorageTypeRequestedEventArgs" /> class.
    /// </summary>
    /// <param name="context">The context object for the storage.</param>
    /// <param name="storageTypes">The types loaded for the storage.</param>
    public StorageTypeRequestedEventArgs(object context, IList<IStorageData> storageTypes)
    {
        this.Context = context;
        this._storageTypes = storageTypes;
    }

    /// <inheritdoc />
    public object Context { get; }

    /// <inheritdoc />
    public void Load(IStorageData data, int priority)
    {
        this._prioritizedTypes.Add(new(data, priority));
        this._prioritizedTypes.Sort((t1, t2) => -t1.Value.CompareTo(t2.Value));
        this._storageTypes.Clear();
        foreach (var (storageType, _) in this._prioritizedTypes)
        {
            this._storageTypes.Add(storageType);
        }
    }
}