/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Taxes.Commands;

#region using directives

using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;

#endregion using directives

[UsedImplicitly]
internal sealed class SetExpensesCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="SetExpensesCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetExpensesCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "set_expenses", "expenses" };

    /// <inheritdoc />
    public override string Documentation => "Set the player's current business expenses to the specified value.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length == 0 || !int.TryParse(args[0], out _))
        {
            Log.W("You must specify an integer value.");
            return;
        }

        if (args.Length > 1)
        {
            Log.W("Additional arguments will be ignored.");
        }

        Game1.player.Write(DataFields.BusinessExpenses, args[0]);
        Log.I($"{Game1.player.Name}'s season income has been set to {args[0]}.");
    }
}
