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
internal sealed class HostPeerDisconnectedEvent : PeerDisconnectedEvent
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && Context.IsMainPlayer;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal HostPeerDisconnectedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    public override bool Enable() => false;

    /// <inheritdoc />
    public override bool Disable() => false;

    /// <inheritdoc />
    protected override void OnPeerDisconnectedImpl(object? sender, PeerDisconnectedEventArgs e)
    {
        if (!Game1.game1.DoesAnyPlayerHaveProfession(Profession.Conservationist, out _))
            Manager.Disable<ConservationismDayEndingEvent>();
    }
}