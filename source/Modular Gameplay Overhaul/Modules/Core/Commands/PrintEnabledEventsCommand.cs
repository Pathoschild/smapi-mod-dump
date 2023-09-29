/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Core.Commands;

#region using directives

using System.Linq;
using System.Text;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using DaLion.Shared.Events;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class PrintEnabledEventsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="PrintEnabledEventsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal PrintEnabledEventsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "print_events", "events" };

    /// <inheritdoc />
    public override string Documentation => "Prints all currently subscribed mod events.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        var message = new StringBuilder("Enabled events:");
        var events = ModEntry.EventManager.Enabled.Cast<ManagedEvent>().ToList();
        events.Sort();
        message = events.Aggregate(
            message,
            (current, next) => current.Append("\n\t- " + next.GetType().Name));
        Log.I(message.ToString());
    }
}
