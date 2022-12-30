/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.LargeTerrainFeatureListChanged"/> allowing dynamic enabling / disabling.</summary>
internal abstract class LargeTerrainFeatureListChangedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="LargeTerrainFeatureListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected LargeTerrainFeatureListChangedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.World.LargeTerrainFeatureListChanged += this.OnLargeTerrainFeatureListChanged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.World.LargeTerrainFeatureListChanged -= this.OnLargeTerrainFeatureListChanged;
    }

    /// <inheritdoc cref="IWorldEvents.LargeTerrainFeatureListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnLargeTerrainFeatureListChanged(object? sender, LargeTerrainFeatureListChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnLargeTerrainFeatureListChangedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnLargeTerrainFeatureListChanged"/>
    protected abstract void OnLargeTerrainFeatureListChangedImpl(
        object? sender, LargeTerrainFeatureListChangedEventArgs e);
}
