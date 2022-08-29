/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using Common.Events;
using Extensions;
using GameLoop;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class HostPeerConnectedEvent : PeerConnectedEvent
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && Context.IsMainPlayer;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal HostPeerConnectedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    public override bool Enable() => false;

    /// <inheritdoc />
    public override bool Disable() => false;

    /// <inheritdoc />
    protected override void OnPeerConnectedImpl(object? sender, PeerConnectedEventArgs e)
    {
        if (Game1.getFarmer(e.Peer.PlayerID).HasProfession(Profession.Conservationist))
            Manager.Enable<ConservationismDayEndingEvent>();
    }
}