/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IContentEvents.AssetsInvalidated"/> allowing dynamic enabling / disabling.</summary>
internal abstract class AssetsInvalidatedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="AssetsInvalidatedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected AssetsInvalidatedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Content.AssetsInvalidated += this.OnAssetsInvalidated;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Content.AssetsInvalidated -= this.OnAssetsInvalidated;
    }

    /// <inheritdoc cref="IContentEvents.AssetsInvalidated"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    internal void OnAssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnAssetsInvalidatedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnAssetsInvalidated"/>
    protected abstract void OnAssetsInvalidatedImpl(object? sender, AssetsInvalidatedEventArgs e);
}
