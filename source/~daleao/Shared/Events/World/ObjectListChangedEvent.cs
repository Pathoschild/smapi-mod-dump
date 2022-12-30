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

/// <summary>Wrapper for <see cref="IWorldEvents.ObjectListChanged"/> allowing dynamic enabling / disabling.</summary>
internal abstract class ObjectListChangedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ObjectListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected ObjectListChangedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.World.ObjectListChanged += this.OnObjectListChanged;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.World.ObjectListChanged -= this.OnObjectListChanged;
    }

    /// <inheritdoc cref="IWorldEvents.ObjectListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnObjectListChanged(object? sender, ObjectListChangedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnObjectListChangedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnObjectListChanged"/>
    protected abstract void OnObjectListChangedImpl(object? sender, ObjectListChangedEventArgs e);
}
