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

/// <summary>Wrapper for <see cref="IGameLoopEvents.DayStarted"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class DayStartedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected DayStartedEvent(EventManager manager)
        : base(manager) { }

    /// <summary>Raised after a new in-game day starts, or after connecting to a multiplayer world.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        if (IsHooked) OnDayStartedImpl(sender, e);
    }

    /// <inheritdoc cref="OnDayStarted" />
    protected abstract void OnDayStartedImpl(object? sender, DayStartedEventArgs e);
}