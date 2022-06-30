/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Commands;

#region using directives

using Common;
using Common.Commands;
using Framework;
using JetBrains.Annotations;
using StardewValley;
using static System.String;

#endregion using directives

[UsedImplicitly]
internal sealed class PrintProfessionsCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal PrintProfessionsCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "professions";

    /// <inheritdoc />
    public override string Documentation => "List the player's current professions.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (!Game1.player.professions.Any())
        {
            Log.I($"Farmer {Game1.player.Name} doesn't have any professions.");
            return;
        }

        var message = $"Farmer {Game1.player.Name}'s professions:";
        foreach (var pid in Game1.player.professions)
        {
            string name;
            if (Profession.TryFromValue(pid > 100 ? pid - 100 : pid, out var profession))
                name = profession.StringId + (pid > 100 ? " (P)" : Empty);
            else if (ModEntry.CustomProfessions.ContainsKey(pid))
                name = ModEntry.CustomProfessions[pid].StringId;
            else
                name = $"Unknown profession {pid}";

            message += "\n\t- " + name;
        }

        Log.I(message);
    }
}