/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Content;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IContentEvents.AssetRequested"/> allowing dynamic enabling / disabling.</summary>
internal abstract class AssetRequestedEvent : BaseEvent
{
    /// <inheritdoc cref="IContentEvents.AssetRequested"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnAssetRequested(object sender, AssetRequestedEventArgs e)
    {
        if (enabled.Value || GetType().Name.StartsWith("Static")) OnAssetRequestedImpl(sender, e);
    }

    /// <inheritdoc cref="OnAssetRequested" />
    protected abstract void OnAssetRequestedImpl(object sender, AssetRequestedEventArgs e);
}