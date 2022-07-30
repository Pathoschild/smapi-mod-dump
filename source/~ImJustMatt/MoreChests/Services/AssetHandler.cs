/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

#nullable disable

namespace StardewMods.MoreChests.Services;

using System.Collections.Generic;
using StardewModdingAPI;
using StardewMods.FuryCore.Interfaces;

/// <inheritdoc cref="StardewMods.FuryCore.Interfaces.IModService" />
internal class AssetHandler : IModService, IAssetLoader
{
    public AssetHandler(IModServices services)
    {
    }

    private IDictionary<string, string> EmptyData = new Dictionary<string, string>();

    /// <inheritdoc />
    public bool CanLoad<T>(IAssetInfo asset)
    {
        return asset.AssetNameEquals($"{MoreChests.ModUniqueId}/Chests");
    }

    /// <inheritdoc />
    public T Load<T>(IAssetInfo asset)
    {
        return (T)this.EmptyData;
    }
}