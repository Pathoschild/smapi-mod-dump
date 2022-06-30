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

#endregion region using directives

/// <summary>Wrapper for <see cref="ISpecializedEvents.UnvalidatedUpdateTicked"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class UnvalidatedUpdateTickedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected UnvalidatedUpdateTickedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="ISpecializedEvents.UnvalidatedUpdateTicked"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnUnvalidatedUpdateTicked(object? sender, UnvalidatedUpdateTickedEventArgs e)
    {
        if (IsHooked) OnUnvalidatedUpdateTickedImpl(sender, e);
    }

    /// <inheritdoc cref="OnUnvalidatedUpdateTicked" />
    protected abstract void OnUnvalidatedUpdateTickedImpl(object? sender, UnvalidatedUpdateTickedEventArgs e);
}