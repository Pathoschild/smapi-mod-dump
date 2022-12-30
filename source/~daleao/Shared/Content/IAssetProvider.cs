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

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Interface for a class which provides an asset.</summary>
public interface IAssetProvider
{
    /// <summary>Provides the asset.</summary>
    /// <param name="e">Event arguments for an <see cref="IContentEvents.AssetRequested"/> event.</param>
    void Provide(AssetRequestedEventArgs e);
}
