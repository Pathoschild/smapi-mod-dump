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

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.SMAPI;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class RemoveProfessionsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="RemoveProfessionsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal RemoveProfessionsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "remove_professions", "remove_profs", "remove" };

    /// <inheritdoc />
    public override string Documentation =>
        "Remove the specified professions from the player. Does not affect skill levels." + this.GetUsage();

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length == 0)
        {
            Log.W("You must specify at least one profession." + this.GetUsage());
            return;
        }

        List<int> professionsToRemove = new();
        foreach (var arg in args)
        {
            if (string.Equals(arg, "all", StringComparison.InvariantCultureIgnoreCase))
            {
                Game1.player.professions.Clear();
                LevelUpMenu.RevalidateHealth(Game1.player);
                if (professionsToRemove.Intersect(Profession.GetRange(true)).Any())
                {
                    ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
                }

                Log.I($"Removed all professions from {Game1.player.Name}.");
                break;
            }

            if (string.Equals(arg, "rogue", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(arg, "unknown", StringComparison.InvariantCultureIgnoreCase))
            {
                var range = Game1.player.professions
                    .Where(pid =>
                        !Profession.TryFromValue(pid, out _) &&
                        SCProfession.List.All(p => pid != p.Id))
                    .ToArray();

                professionsToRemove.AddRange(range);
                Log.I($"Removed unknown professions from {Game1.player.Name}.");
            }
            else if (Profession.TryFromName(arg, true, out var profession) ||
                     Profession.TryFromLocalizedName(arg, true, out profession))
            {
                professionsToRemove.Add(profession.Id);
                professionsToRemove.Add(profession.Id + 100);
                Log.I($"Removed {profession.StringId} profession from {Game1.player.Name}.");
            }
            else
            {
                var customProfession = SCProfession.List.FirstOrDefault(p =>
                    string.Equals(arg, p.StringId.TrimAll(), StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(arg, p.Title.TrimAll(), StringComparison.InvariantCultureIgnoreCase));
                if (customProfession is null)
                {
                    Log.W($"Ignoring unknown profession {arg}.");
                    continue;
                }

                professionsToRemove.Add(customProfession.Id);
                Log.I($"Removed {customProfession.StringId} profession from {Game1.player.Name}.");
            }
        }

        foreach (var pid in professionsToRemove.Distinct())
        {
            GameLocation.RemoveProfession(pid);
        }

        LevelUpMenu.RevalidateHealth(Game1.player);
        if (professionsToRemove.Intersect(Profession.GetRange(true)).Any())
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
        }
    }

    private string GetUsage()
    {
        var result =
            $"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers.First()} [--prestige] <profession1> <profession2> ... <professionN>";
        result += "\n\nParameters:";
        result +=
            "\n\t- <profession>\t- a valid profession name, `all` or `unknown`. Use `unknown` to remove rogue professions from uninstalled custom skill mods.";
        result += "\n\nExamples:";
        result += $"\n\t- {this.Handler.EntryCommand} {this.Triggers.First()} artisan brute";
        result += $"\n\t- {this.Handler.EntryCommand} {this.Triggers.First()} -p all";
        return result;
    }
}
