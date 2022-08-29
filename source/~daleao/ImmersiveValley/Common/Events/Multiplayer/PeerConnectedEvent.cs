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

/// <summary>Wrapper for <see cref="IMultiplayerEvents.PeerConnected"/> allowing dynamic enabling / disabling.</summary>
internal abstract class PeerConnectedEvent : ManagedEvent
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && base.IsEnabled;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected PeerConnectedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc cref="IMultiplayerEvents.PeerConnected"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    internal void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
    {
        if (IsEnabled) OnPeerConnectedImpl(sender, e);
    }

    /// <inheritdoc cref="OnPeerConnected" />
    protected abstract void OnPeerConnectedImpl(object? sender, PeerConnectedEventArgs e);
}