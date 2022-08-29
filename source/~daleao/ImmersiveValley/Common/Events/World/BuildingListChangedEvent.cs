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

#endregion region using directives

/// <summary>Wrapper for <see cref="IWorldEvents.BuildingListChanged"/> allowing dynamic enabling / disabling.</summary>
internal abstract class BuildingListChangedEvent : ManagedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected BuildingListChangedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IWorldEvents.BuildingListChanged"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnBuildingListChanged(object? sender, BuildingListChangedEventArgs e)
    {
        if (IsEnabled) OnBuildingListChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnBuildingListChanged" />
    protected abstract void OnBuildingListChangedImpl(object? sender, BuildingListChangedEventArgs e);
}