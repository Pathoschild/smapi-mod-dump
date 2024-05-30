/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Commands;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="EnrageCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
[Debug]
internal sealed class EnrageCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["enrage"];

    /// <inheritdoc />
    public override string Documentation =>
        "Clear the player's cache of new levels for the specified skills, or all vanilla skills if none are specified.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        if (Game1.player.HasProfession(Profession.Brute))
        {
            State.BruteRageCounter += 20;
        }

        return true;
    }
}
