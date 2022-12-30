/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Commands;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class OverHealCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="OverHealCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal OverHealCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "overheal" };

    /// <inheritdoc />
    public override string Documentation => "Heals the player, allowing health to go beyond the maximum value.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var player = Game1.player;
        if (args.Length == 0 || !int.TryParse(args[0], out var amount))
        {
            amount = (int)(player.maxHealth * 1.2f);
        }

        player.health = Math.Min(amount, (int)(player.maxHealth * 1.2f));
    }
}
