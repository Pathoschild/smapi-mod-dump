/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using Common;
using Common.Events;
using GameLoop;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class HostRequestedModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && Context.IsMainPlayer;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal HostRequestedModMessageReceivedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    public override bool Enable() => false;

    /// <inheritdoc />
    public override bool Disable() => false;

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || e.Type != "RequestHost") return;

        var split = e.ReadAs<string>().Split('/');
        var request = split[0];
        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            Log.W($"Received {request} request from unknown player {e.FromPlayerID}.");
            return;
        }

        switch (request)
        {
            case "Conservationism":
                Log.D($"{who.Name} requested Conservationism event subscription.");
                Manager.Enable<ConservationismDayEndingEvent>();
                break;
            case "HuntIsOn":
                Log.D($"Prestiged treasure hunter {who.Name} is hunting for treasure.");
                Manager.Enable<PrestigeTreasureHuntUpdateTickedEvent>();
                break;
        }
    }
}