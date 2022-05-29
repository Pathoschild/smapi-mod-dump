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

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Content;
using GameLoop;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class HostPeerConnectedEvent : PeerConnectedEvent
{
    /// <inheritdoc />
    protected override void OnPeerConnectedImpl(object sender, PeerConnectedEventArgs e)
    {
        EventManager.Enable(typeof(ToggledUltimateModMessageReceivedEvent),
            typeof(RequestGlobalEventModMessageReceivedEvent), typeof(RequestUpdateDataModMessageReceivedEvent),
            typeof(RequestUpdateHostStateModMessageReceivedEvent));

        if (Game1.getFarmer(e.Peer.PlayerID).HasProfession(Profession.Aquarist))
            EventManager.Enable(typeof(HostFishPondDataRequestedEvent));

        if (Game1.getFarmer(e.Peer.PlayerID).HasProfession(Profession.Conservationist))
            EventManager.Enable(typeof(HostConservationismDayEndingEvent));
    }
}