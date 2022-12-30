/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Commands;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Commands;

#endregion using directives

[UsedImplicitly]
internal sealed class SetRegisteredUltimateCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="SetRegisteredUltimateCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetRegisteredUltimateCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "set_ult" };

    /// <inheritdoc />
    public override string Documentation => "Change the player's currently registered Special Ability.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length > 1)
        {
            Log.W("Additional arguments beyond the first will be ignored.");
            return;
        }

        if (args[0].ToLowerInvariant() is "clear" or "null")
        {
            Game1.player.Set_Ultimate(null);
            return;
        }

        if (!Game1.player.professions.Any(p => p is >= 26 and < 30))
        {
            Log.W("You don't have any 2nd-tier combat professions.");
            return;
        }

        if (!Ultimate.TryFromName(args[0], true, out var ultimate) &&
            !(Profession.TryFromLocalizedName(args[0], true, out var profession) &&
             Ultimate.TryFromValue(profession, out ultimate)))
        {
            Log.W("You must enter a valid 2nd-tier combat profession or special ability name.");
            return;
        }

        if (!Game1.player.HasProfession(ultimate.Profession))
        {
            Log.W("You don't have this profession.");
            return;
        }

        Game1.player.Set_Ultimate(ultimate);
    }
}
