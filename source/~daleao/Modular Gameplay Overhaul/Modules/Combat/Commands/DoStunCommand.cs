/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Commands;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Core.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Collections;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class DoStunCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="DoStunCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal DoStunCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "stun" };

    /// <inheritdoc />
    public override string Documentation => "Stuns all nearby monsters.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0 || !int.TryParse(args[0], out var duration))
        {
            duration = 100000;
        }

        Game1.currentLocation.characters.OfType<Monster>().ForEach(m => m.Stun(duration));
    }
}
