/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Content;

#region using directives

using System.Collections.Generic;
using StardewModdingAPI.Events;

#endregion using directivese

/// <summary>Generates a new instance of the <see cref="ModDictionaryProvider{TKey,TValue}"/> record.</summary>
/// <typeparam name="TKey">The type of the keys in the data dictionary.</typeparam>
/// <typeparam name="TValue">The type of the values in the data dictionary.</typeparam>
/// <param name="GetPath">A delegate which returns the relative path to the JSON dictionary inside the mod folder.</param>
/// <param name="Priority">The priority for an asset load when multiple apply for the same asset.</param>
public record ModDictionaryProvider<TKey, TValue>(Func<string> GetPath, AssetLoadPriority Priority = AssetLoadPriority.Medium) : IAssetProvider
    where TKey : notnull
{
    /// <inheritdoc />
    public void Provide(AssetRequestedEventArgs e)
    {
        e.LoadFromModFile<Dictionary<TKey, TValue>>(this.GetPath.Invoke(), this.Priority);
    }
}
