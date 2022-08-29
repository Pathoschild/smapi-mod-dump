/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Common.Events;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IDisplayEvents.MenuChanged"/> allowing dynamic enabling / disabling.</summary>
internal abstract class MenuChangedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected MenuChangedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IDisplayEvents.MenuChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        if (IsEnabled) OnMenuChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnMenuChanged" />
    protected abstract void OnMenuChangedImpl(object? sender, MenuChangedEventArgs e);
}