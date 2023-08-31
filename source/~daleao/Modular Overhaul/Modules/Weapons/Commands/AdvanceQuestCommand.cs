/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Commands;

#region using directives

using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class AdvanceQuestCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="AdvanceQuestCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal AdvanceQuestCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "advance_quest", "advance", "adv" };

    /// <inheritdoc />
    public override string Documentation => "Forcefully advances the specified quest-line (either Clint's Forge or Viego's Curse / Yoba's Virtues).";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        var player = Game1.player;
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.W("You must specify a quest-line to advance (either \"Forge\" or \"Curse\".");
            return;
        }

        switch (args[0].ToLowerInvariant())
        {
            case "forge":
            case "clint":
            case "dwarven":
            case "legacy":
                if (player.hasQuest((int)Quest.ForgeIntro))
                {
                    player.completeQuest((int)Quest.ForgeIntro);
                }

                if (!player.mailReceived.Contains("clintForge"))
                {
                    player.mailReceived.Add("clintForge");
                }

                break;
            case "hero":
            case "ruin":
            case "dawn":
            case "curse":
            case "viego":
            case "yoba":
            case "virtues":
            case "chivalry":
            case "purification":
                if (player.Read<VirtueQuestState>(DataKeys.VirtueQuestState) == VirtueQuestState.NotStarted)
                {
                    if (!player.hasQuest((int)Quest.CurseIntro))
                    {
                        player.addQuest((int)Quest.CurseIntro);
                        return;
                    }

                    player.completeQuest((int)Quest.CurseIntro);
                    player.Write(DataKeys.InspectedHonor, null);
                    player.Write(DataKeys.InspectedCompassion, null);
                    player.Write(DataKeys.InspectedWisdom, null);
                    player.Write(DataKeys.InspectedGenerosity, null);
                    player.Write(DataKeys.InspectedValor, null);
                    WeaponsModule.State.VirtuesQuest ??= new VirtueQuest();
                    player.Write(DataKeys.VirtueQuestState, VirtueQuestState.InProgress.ToString());
                    return;
                }

                if (player.Read<VirtueQuestState>(DataKeys.VirtueQuestState) == VirtueQuestState.InProgress)
                {
                    WeaponsModule.State.VirtuesQuest ??= new VirtueQuest();
                    player.Write(DataKeys.ProvenHonor, int.MaxValue.ToString());
                    player.Write(DataKeys.ProvenCompassion, int.MaxValue.ToString());
                    player.Write(DataKeys.ProvenWisdom, int.MaxValue.ToString());
                    player.Write(DataKeys.ProvenGenerosity, int.MaxValue.ToString());
                    Virtue.List.ForEach(virtue => WeaponsModule.State.VirtuesQuest.UpdateVirtueProgress(virtue));
                }

                break;
            case "honor":
                {
                    if (args.Length == 1 || int.TryParse(args[1], out var amount))
                    {
                        amount = 1;
                    }

                    player.Increment(DataKeys.ProvenHonor, amount);
                    WeaponsModule.State.VirtuesQuest?.UpdateVirtueProgress(Virtue.Honor);
                }

                break;
            case "compassion":
                {
                    if (args.Length == 1 || int.TryParse(args[1], out var amount))
                    {
                        amount = 1;
                    }

                    player.Increment(DataKeys.ProvenCompassion, amount);
                    WeaponsModule.State.VirtuesQuest?.UpdateVirtueProgress(Virtue.Compassion);
                }

                break;
            case "wisdom":
                {
                    if (args.Length == 1 || int.TryParse(args[1], out var amount))
                    {
                        amount = 1;
                    }

                    player.Increment(DataKeys.ProvenWisdom, amount);
                    WeaponsModule.State.VirtuesQuest?.UpdateVirtueProgress(Virtue.Wisdom);
                }

                break;
            case "generosity":
                {
                    if (args.Length == 1 || int.TryParse(args[1], out var amount))
                    {
                        amount = 1;
                    }

                    player.Increment(DataKeys.ProvenGenerosity, amount);
                    WeaponsModule.State.VirtuesQuest?.UpdateVirtueProgress(Virtue.Generosity);
                }

                break;
            case "valor":
                {
                    Game1.stats.specificMonstersKilled["Green Slime"] = 1000;
                    Game1.stats.specificMonstersKilled["Shadow Brute"] = 150;
                    Game1.stats.specificMonstersKilled["Bat"] = 200;
                    Game1.stats.specificMonstersKilled["Skeleton"] = 50;
                    Game1.stats.specificMonstersKilled["Bug"] = 125;
                    Game1.stats.specificMonstersKilled["Duggy"] = 30;
                    Game1.stats.specificMonstersKilled["Dust Spirit"] = 500;
                    Game1.stats.specificMonstersKilled["Rock Crab"] = 60;
                    Game1.stats.specificMonstersKilled["Mummy"] = 100;
                    Game1.stats.specificMonstersKilled["Pepper Rex"] = 50;
                    Game1.stats.specificMonstersKilled["Serpent"] = 250;
                    Game1.stats.specificMonstersKilled["Magma Sprite"] = 150;
                }

                break;
        }
    }
}
