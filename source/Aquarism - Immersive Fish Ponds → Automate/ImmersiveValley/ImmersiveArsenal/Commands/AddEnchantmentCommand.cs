/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Commands;

#region using directives

using Common;
using Common.Commands;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class AddEnchantmentCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal AddEnchantmentCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "add_enchants";

    /// <inheritdoc />
    public override string Documentation => "Add the specified enchantment to the selected weapon." + GetUsage();

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (Game1.player.CurrentTool is not MeleeWeapon weapon)
        {
            Log.W("You must select a weapon first.");
            return;
        }

        while (args.Any())
        {
            BaseEnchantment? enchantment = args[0].ToLower() switch
            {
                "artful" => new ArchaeologistEnchantment(),
                "bugkiller" => new BugKillerEnchantment(),
                "crusader" => new CrusaderEnchantment(),
                "vampiric" => new VampiricEnchantment(),
                "haymaker" => new HaymakerEnchantment(),
                "magic" or "starburst" => new MagicEnchantment(), // not implemented
                _ => null
            };

            if (enchantment is null)
            {
                Log.W($"Ignoring unknown enchantment {args[0]}.");
                return;
            }

            if (!enchantment.CanApplyTo(weapon))
            {
                Log.W($"Cannot apply {enchantment.GetDisplayName()} enchantment to {weapon.DisplayName}.");
                return;
            }

            weapon.enchantments.Add(enchantment);
            Log.I($"Applied {enchantment.GetDisplayName()} enchantment to {weapon.DisplayName}.");

            args = args.Skip(1).ToArray();
        }
    }

    /// <summary>Tell the dummies how to use the console command.</summary>
    private string GetUsage()
    {
        var result = $"\n\nUsage: {Handler.EntryCommand} {Trigger} <enchantment>";
        result += "\n\nParameters:";
        result += "\n\t- <enchantment>: a tool enchantment";
        result += "\n\nExample:";
        result += $"\n\t- {Handler.EntryCommand} {Trigger} powerful";
        return result;
    }
}