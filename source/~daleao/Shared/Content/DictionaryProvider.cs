/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Content;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using StardewModdingAPI.Events;

#endregion using directivese

/// <summary>Generates a new instance of the <see cref="DictionaryProvider{TKey,TValue}"/> record.</summary>
/// <typeparam name="TKey">The type of the keys in the data dictionary.</typeparam>
/// <typeparam name="TValue">The type of the values in the data dictionary.</typeparam>
/// <param name="Load">A delegate callback for loading the initial instance of the content asset.</param>
/// <param name="Priority">The priority for an asset load when multiple apply for the same asset.</param>
public record DictionaryProvider<TKey, TValue>(Func<Dictionary<TKey, TValue>>? Load, AssetLoadPriority Priority) : IAssetProvider
    where TKey : notnull
{
    /// <inheritdoc />
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Preference for inner functions.")]
    public void Provide(AssetRequestedEventArgs e)
    {
        e.LoadFrom(this.Load ?? fallback, this.Priority);
        Dictionary<TKey, TValue> fallback()
        {
            return new Dictionary<TKey, TValue>();
        }
    }
}
