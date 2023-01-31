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

/// <summary>Wrapper for <see cref="IWorldEvents.LocationListChanged"/> allowing dynamic enabling / disabling.</summary>
internal abstract class LocationListChangedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="LocationListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected LocationListChangedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.World.LocationListChanged += this.OnLocationListChanged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.World.LocationListChanged -= this.OnLocationListChanged;
    }

    /// <inheritdoc cref="IWorldEvents.LocationListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnLocationListChanged(object? sender, LocationListChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnLocationListChangedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnLocationListChanged"/>
    protected abstract void OnLocationListChangedImpl(object? sender, LocationListChangedEventArgs e);
}
