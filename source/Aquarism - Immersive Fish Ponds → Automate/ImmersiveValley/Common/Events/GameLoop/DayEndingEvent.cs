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

/// <summary>Wrapper for <see cref="IGameLoopEvents.DayEnding"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class DayEndingEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected DayEndingEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IGameLoopEvents.DayEnding"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnDayEnding(object? sender, DayEndingEventArgs e)
    {
        if (IsHooked) OnDayEndingImpl(sender, e);
    }

    /// <inheritdoc cref="OnDayEnding" />
    protected abstract void OnDayEndingImpl(object? sender, DayEndingEventArgs e);
}