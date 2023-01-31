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

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

#endregion using directivese

/// <summary>Generates a new instance of the <see cref="TextureProvider"/> record.</summary>
/// <param name="Load">A delegate callback for loading the initial instance of the content asset.</param>
/// <param name="Priority">The priority for an asset load when multiple apply for the same asset.</param>
public record TextureProvider(Func<Texture2D> Load, AssetLoadPriority Priority) : IAssetProvider
{
    /// <inheritdoc />
    public void Provide(AssetRequestedEventArgs e)
    {
        e.LoadFrom(this.Load, this.Priority);
    }
}
