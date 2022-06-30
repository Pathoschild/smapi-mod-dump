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

/// <summary>Wrapper for <see cref="IPlayerEvents.LevelChanged"/> allowing dynamic hooking / unhooking.</summary>
internal abstract class LevelChangedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected LevelChangedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IPlayerEvents.LevelChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnLevelChanged(object? sender, LevelChangedEventArgs e)
    {
        if (IsHooked) OnLevelChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnLevelChanged" />
    protected abstract void OnLevelChangedImpl(object? sender, LevelChangedEventArgs e);
}