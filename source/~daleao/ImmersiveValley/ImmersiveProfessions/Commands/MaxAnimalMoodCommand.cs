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
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class MaxAnimalMoodCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal MaxAnimalMoodCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "max_animal_mood", "max_mood", "happy_animals", "happy" };

    /// <inheritdoc />
    public override string Documentation => $"Max-out the mood of all owned animals. Relevant for {Profession.Producer}s.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var animals = Game1.getFarm().getAllFarmAnimals().Where(a =>
            a.ownerID.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer).ToList();
        var count = animals.Count;

        if (count <= 0)
        {
            Log.W("You don't own any animals.");
            return;
        }

        foreach (var animal in animals) animal.happiness.Value = 255;
        Log.I($"Maxed the mood of {count} animals");
    }
}