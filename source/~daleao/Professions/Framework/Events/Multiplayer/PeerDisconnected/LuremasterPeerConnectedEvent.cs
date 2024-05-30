/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Events.Multiplayer.PeerConnected;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.DayStarted;
using DaLion.Professions.Framework.Events.GameLoop.TimeChanged;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="LuremasterPeerConnectedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class LuremasterPeerConnectedEvent(EventManager? manager = null)
    : PeerConnectedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnPeerConnectedImpl(object? sender, PeerConnectedEventArgs e)
    {
        if (!Game1.getFarmer(e.Peer.PlayerID).HasProfession(Profession.Luremaster))
        {
            return;
        }

        this.Manager.Enable(
            typeof(LuremasterDayStartedEvent),
            typeof(LuremasterTimeChangedEvent));
        this.Disable();
    }
}
