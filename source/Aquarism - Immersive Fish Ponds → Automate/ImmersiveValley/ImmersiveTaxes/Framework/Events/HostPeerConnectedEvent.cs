/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Taxes.Framework.Events;

#region using directives

using Common.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class HostPeerConnectedEvent : PeerConnectedEvent
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && Context.IsMainPlayer;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref=EventManager"/> instance that manages this event.</param>
    internal HostPeerConnectedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    public override bool Enable() => false;

    /// <inheritdoc />
    protected override void OnPeerConnectedImpl(object? sender, PeerConnectedEventArgs e)
    {
        if (e.Peer.IsSplitScreen && e.Peer.ScreenID.HasValue)
            ModEntry.Events.EnableForScreen(e.Peer.ScreenID.Value, typeof(TaxDayEndingEvent),
                typeof(TaxDayStartedEvent));
    }
}