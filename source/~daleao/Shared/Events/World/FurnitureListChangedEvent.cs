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

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.FurnitureListChanged"/> allowing dynamic enabling / disabling.</summary>
internal abstract class FurnitureListChangedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="FurnitureListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected FurnitureListChangedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.World.FurnitureListChanged += this.OnFurnitureListChanged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.World.FurnitureListChanged -= this.OnFurnitureListChanged;
    }

    /// <inheritdoc cref="IWorldEvents.FurnitureListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnFurnitureListChanged(object? sender, FurnitureListChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnFurnitureListChangedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnFurnitureListChanged"/>
    protected abstract void OnFurnitureListChangedImpl(object? sender, FurnitureListChangedEventArgs e);
}
