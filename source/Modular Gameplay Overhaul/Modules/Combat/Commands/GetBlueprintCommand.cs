/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Commands;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Commands;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Networking;

#endregion using directives

[UsedImplicitly]
internal sealed class GetBlueprintCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="GetBlueprintCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal GetBlueprintCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "get_blueprint", "blueprint", "get_bp", "bp" };

    /// <inheritdoc />
    public override string Documentation => "Adds a random Dwarvish Blueprint to the local player.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length > 0)
        {
            Log.W("Additional parameters will be ignored.");
        }

        if (!JsonAssetsIntegration.DwarvishBlueprintIndex.HasValue)
        {
            Log.W("Dwarvish Blueprint item was not initialized correctly.");
            return;
        }

        var player = Game1.player;
        var allBlueprints = new List<int>
        {
            WeaponIds.ElfBlade,
            WeaponIds.ForestSword,
            WeaponIds.DwarfSword,
            WeaponIds.DwarfHammer,
            WeaponIds.DwarfDagger,
            WeaponIds.DragontoothCutlass,
            WeaponIds.DragontoothClub,
            WeaponIds.DragontoothShiv,
        };

        if (args.Length > 0)
        {
            switch (args[0].ToLowerInvariant())
            {
                case "all":
                    player.Write(DataKeys.BlueprintsFound, string.Join(',', allBlueprints));
                    Log.I($"Added all Dwarvish Blueprints to {player.Name}.");
                    return;
                case "none":
                    player.Write(DataKeys.BlueprintsFound, null);
                    Log.I($"Removed all Dwarvish Blueprints from {player.Name}.");
                    return;
                default:
                    var found = player.Read(DataKeys.BlueprintsFound).ParseList<int>().ToHashSet();
                    for (var i = 0; i < args.Length; i++)
                    {
                        switch (args[i].ToLowerInvariant())
                        {
                            case "elfblade":
                            case "elfdagger":
                            case "elvenblade":
                            case "elvendagger":
                                if (found.Contains(WeaponIds.ElfBlade))
                                {
                                    Log.W($"{player.Name} has already found the Elven Blade blueprint.");
                                    break;
                                }

                                player.Append(DataKeys.BlueprintsFound, WeaponIds.ElfBlade.ToString());
                                Log.I($"Added the Elven Blade Blueprint to {player.Name}.");
                                break;
                            case "elfsword":
                            case "elvensword":
                            case "forestsword":
                                if (found.Contains(WeaponIds.ForestSword))
                                {
                                    Log.W($"{player.Name} has already found the Elven Sword blueprint.");
                                    break;
                                }

                                player.Append(DataKeys.BlueprintsFound, WeaponIds.ForestSword.ToString());
                                Log.I($"Added the Elven Sword Blueprint to {player.Name}.");
                                break;
                            case "dwarfsword":
                            case "dwarvensword":
                                if (found.Contains(WeaponIds.DwarfSword))
                                {
                                    Log.W($"{player.Name} has already found the Dwarven Sword blueprint.");
                                    break;
                                }

                                player.Append(DataKeys.BlueprintsFound, WeaponIds.DwarfSword.ToString());
                                Log.I($"Added the Dwarven Sword Blueprint to {player.Name}.");
                                break;
                            case "dwarfdagger":
                            case "dwarvendagger":
                                if (found.Contains(WeaponIds.DwarfDagger))
                                {
                                    Log.W($"{player.Name} has already found the Dwarven Dagger blueprint.");
                                    break;
                                }

                                player.Append(DataKeys.BlueprintsFound, WeaponIds.DwarfDagger.ToString());
                                Log.I($"Added the Dwarven Dagger Blueprint to {player.Name}.");
                                break;
                            case "dwarfclub":
                            case "dwarvenclub":
                            case "dwarfhammer":
                            case "dwarvenhammer":
                                if (found.Contains(WeaponIds.DwarfHammer))
                                {
                                    Log.W($"{player.Name} has already found the Dwarven Hammer blueprint.");
                                    break;
                                }

                                player.Append(DataKeys.BlueprintsFound, WeaponIds.DwarfHammer.ToString());
                                Log.I($"Added the Dwarven Hammer Blueprint to {player.Name}.");
                                break;
                            case "dragonsword":
                            case "dragoncutlass":
                            case "dragontoothsword":
                            case "dragontoothcutlass":
                                if (found.Contains(WeaponIds.DragontoothCutlass))
                                {
                                    Log.W($"{player.Name} has already found the Dragontooth Cutlass blueprint.");
                                    break;
                                }

                                player.Append(DataKeys.BlueprintsFound, WeaponIds.DragontoothCutlass.ToString());
                                Log.I($"Added the Dragontooth Cutlass Blueprint to {player.Name}.");
                                break;
                            case "dragondagger":
                            case "dragonshiv":
                            case "dragontoothdagger":
                            case "dragontoothshiv":
                                if (found.Contains(WeaponIds.DragontoothShiv))
                                {
                                    Log.W($"{player.Name} has already found the Dragontooth Shiv blueprint.");
                                    break;
                                }

                                player.Append(DataKeys.BlueprintsFound, WeaponIds.DragontoothShiv.ToString());
                                Log.I($"Added the Dragontooth Shiv Blueprint to {player.Name}.");
                                break;
                            case "dragonclub":
                            case "dragontoothclub":
                                if (found.Contains(WeaponIds.DragontoothClub))
                                {
                                    Log.W($"{player.Name} has already found the Dragontooth Club blueprint.");
                                    break;
                                }

                                player.Append(DataKeys.BlueprintsFound, WeaponIds.DragontoothClub.ToString());
                                Log.I($"Added the Dragontooth Club Blueprints to {player.Name}.");
                                break;
                            default:
                                Log.W($"Ignoring unknown weapon '{args[i]}'.");
                                break;
                        }
                    }

                    player.holdUpItemThenMessage(new SObject(JsonAssetsIntegration.DwarvishBlueprintIndex.Value, 1));
                    if (Context.IsMultiplayer && Game1.player.mailReceived.Contains("clintForge"))
                    {
                        Broadcaster.SendPublicChat(I18n.Blueprint_Found_Global(player.Name));
                    }

                    player.addItemToInventoryBool(new SObject(JsonAssetsIntegration.DwarvishBlueprintIndex.Value, 1));
                    return;
            }
        }

        var notFound = allBlueprints.Except(player.Read(DataKeys.BlueprintsFound).ParseList<int>()).ToArray();
        var chosen = Game1.random.Next(notFound.Length);
        player.Append(DataKeys.BlueprintsFound, notFound[chosen].ToString());
        ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
        player.holdUpItemThenMessage(new SObject(JsonAssetsIntegration.DwarvishBlueprintIndex.Value, 1));
        if (Context.IsMultiplayer && Game1.player.mailReceived.Contains("clintForge"))
        {
            Broadcaster.SendPublicChat(I18n.Blueprint_Found_Global(player.Name));
        }

        player.addItemToInventoryBool(new SObject(JsonAssetsIntegration.DwarvishBlueprintIndex.Value, 1));
    }
}
