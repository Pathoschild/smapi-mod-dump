/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

#if DEBUG
namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using Common;
using Common.Events;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class DebugModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal DebugModMessageReceivedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        var command = e.Type.Split('/')[1];
        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            Log.W($"Unknown player {e.FromPlayerID} sent debug {command} message.");
            return;
        }

        switch (command)
        {
            case "Request":
                Log.D($"Player {e.FromPlayerID} requested debug information.");
                var what = e.ReadAs<string>();
                switch (what)
                {
                    case "EventsHooked":
                        var response = Manager.Hooked.Aggregate("",
                            (current, next) => current + "\n\t- " + next.GetType().Name);
                        ModEntry.Broadcaster.Message(response, "Debug/Response", e.FromPlayerID);

                        break;
                }

                break;

            case "Response":
                Log.D($"Player {e.FromPlayerID} responded to {command} debug information.");
                ModEntry.Broadcaster.ResponseReceived.TrySetResult(e.ReadAs<string>());

                break;
        }
    }
}
#endif