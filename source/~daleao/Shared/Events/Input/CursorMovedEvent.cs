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

#endregion using directives

/// <summary>Wrapper for <see cref="IInputEvents.CursorMoved"/> allowing dynamic enabling / disabling.</summary>
internal abstract class CursorMovedEvent : ManagedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CursorMovedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected CursorMovedEvent(EventManager manager)
        : base(manager)
    {
        manager.ModEvents.Input.CursorMoved += this.OnCursorMoved;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        this.Manager.ModEvents.Input.CursorMoved -= this.OnCursorMoved;
    }

    /// <inheritdoc cref="IInputEvents.CursorMoved"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnCursorMoved(object? sender, CursorMovedEventArgs e)
    {
        if (this.IsEnabled)
        {
            this.OnCursorMovedImpl(sender, e);
        }
    }

    /// <inheritdoc cref="OnCursorMoved"/>
    protected abstract void OnCursorMovedImpl(object? sender, CursorMovedEventArgs e);
}
