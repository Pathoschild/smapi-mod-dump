/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Commands;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using DaLion.Shared.Commands;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class AddEnchantmentsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="AddEnchantmentsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal AddEnchantmentsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "add_enchants", "add", "enchant" };

    /// <inheritdoc />
    public override string Documentation => "Add the specified enchantment to the selected tool." + this.GetUsage();

    /// <inheritdoc />
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1012:Opening braces should be spaced correctly", Justification = "Paradoxical.")]
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.W("No enchantment was specified.");
            return;
        }

        if (Game1.player.CurrentTool is not ({ } tool and (Axe or Hoe or Pickaxe or WateringCan or FishingRod or MeleeWeapon)) ||
            (Game1.player.CurrentTool is MeleeWeapon weapon && !weapon.isScythe()))
        {
            Log.W("You must select a tool first.");
            return;
        }

        while (args.Length > 0)
        {
            BaseEnchantment? enchantment = args[0].ToLower() switch
            {
                "auto-hook" or "autohook" => new AutoHookEnchantment(),
                "arch" or "archaeologist" => new ArchaeologistEnchantment(),
                "bottomless" => new BottomlessEnchantment(),
                "efficient" => new EfficientToolEnchantment(),
                "generous" => new GenerousEnchantment(),
                "master" => new MasterEnchantment(),
                "powerful" => new PowerfulEnchantment(),
                "preserving" => new PreservingEnchantment(),
                "reaching" => new ReachingToolEnchantment(),
                "shaving" => new ShavingEnchantment(),
                "swift" => new SwiftToolEnchantment(),
                "haymaker" => new HaymakerEnchantment(),
                _ => null,
            };

            if (enchantment is null)
            {
                Log.W($"Ignoring unknown enchantment {args[0]}.");
                args = args.Skip(1).ToArray();
                continue;
            }

            if (!enchantment.CanApplyTo(tool))
            {
                Log.W($"Cannot apply {enchantment.GetDisplayName()} enchantment to {tool.DisplayName}.");
                args = args.Skip(1).ToArray();
                continue;
            }

            tool.AddEnchantment(enchantment);
            Log.I($"Applied {enchantment.GetDisplayName()} enchantment to {tool.DisplayName}.");

            args = args.Skip(1).ToArray();
        }
    }

    /// <summary>Tell the dummies how to use the console command.</summary>
    private string GetUsage()
    {
        var result = new StringBuilder($"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers[0]} <enchantment>");
        result.Append("\n\nParameters:");
        result.Append("\n\t- <enchantment>: a tool enchantment");
        result.Append("\n\nExample:");
        result.Append($"\n\t- {this.Handler.EntryCommand} {this.Triggers[0]} powerful");
        return result.ToString();
    }
}
