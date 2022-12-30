/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.Multiplayer;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.GameLoop;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class HostRequestedModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <summary>Initializes a new instance of the <see cref="HostRequestedModMessageReceivedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal HostRequestedModMessageReceivedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && Context.IsMainPlayer;

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != Manifest.UniqueID || e.Type != "RequestHost")
        {
            return;
        }

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
            case "HuntIsOn":
                Log.D($"Prestiged treasure hunter {who.Name} is hunting for treasure.");
                this.Manager.Enable<PrestigeTreasureHuntUpdateTickedEvent>();
                break;
        }
    }
}
