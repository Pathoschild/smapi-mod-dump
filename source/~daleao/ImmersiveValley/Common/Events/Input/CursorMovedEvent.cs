/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IInputEvents.CursorMoved"/> allowing dynamic enabling / disabling.</summary>
internal abstract class CursorMovedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected CursorMovedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IInputEvents.CursorMoved"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnCursorMoved(object? sender, CursorMovedEventArgs e)
    {
        if (IsEnabled) OnCursorMovedImpl(sender, e);
    }

    /// <inheritdoc cref="OnCursorMoved" />
    protected abstract void OnCursorMovedImpl(object? sender, CursorMovedEventArgs e);
}