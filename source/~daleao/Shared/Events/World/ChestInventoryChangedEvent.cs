/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Shared.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.ChestInventoryChanged"/> allowing dynamic enabling / disabling.</summary>
public abstract class ChestInventoryChangedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ChestInventoryChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected ChestInventoryChangedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.World.ChestInventoryChanged += this.OnChestInventoryChanged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.World.ChestInventoryChanged -= this.OnChestInventoryChanged;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IWorldEvents.ChestInventoryChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnChestInventoryChanged(object? sender, ChestInventoryChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnChestInventoryChangedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnChestInventoryChanged"/>
    protected abstract void OnChestInventoryChangedImpl(object? sender, ChestInventoryChangedEventArgs e);
}
