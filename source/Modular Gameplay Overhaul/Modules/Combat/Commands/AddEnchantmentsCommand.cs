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

using System.Linq;
using System.Text;
using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using StardewValley;
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
    public override string[] Triggers { get; } = { "add_enchantments", "add_ench", "enchant", "ench" };

    /// <inheritdoc />
    public override string Documentation =>
        "Add the specified enchantments to the currently selected tool." + this.GetUsage();

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.W("No enchantment was specified.");
            return;
        }

        var tool = Game1.player.CurrentTool;
        if (tool is null or not (MeleeWeapon or Slingshot))
        {
            Log.W("You must select a tool first.");
            return;
        }

        while (args.Length > 0)
        {
            BaseEnchantment? enchantment = args[0].ToLower() switch
            {
                // forges
                "ruby" => new RubyEnchantment(),
                "aquamarine" => new AquamarineEnchantment(),
                "jade" => new JadeEnchantment(),
                "emerald" => new EmeraldEnchantment(),
                "amethyst" => new AmethystEnchantment(),
                "topaz" => new TopazEnchantment(),
                "diamond" => new DiamondEnchantment(),
                "garnet" => new GarnetEnchantment(),

                // weapon enchants
                "artful" when tool is MeleeWeapon => new MeleeArtfulEnchantment(),
                "carving" => new CarvingEnchantment(),
                "cleaving" => new CleavingEnchantment(),
                "energized" or "thunderlords" => new EnergizedEnchantment(),
                "explosive" or "blasting" => new ExplosiveEnchantment(),
                "bloodthirsty" or "vampiric" => new BloodthirstyEnchantment(),
                "steadfast" => new SteadfastEnchantment(),
                "mammonite" or "mammon" => new MammoniteEnchantment(),
                "wabbajack" or "wabba" or "wab" => new WabbajackEnchantment(),

                // slingshot enchants
                "artful" when tool is Slingshot => new RangedArtfulEnchantment(),
                "engorging" or "glutton" or "magnum" => new MagnumEnchantment(),
                "gatling" => new GatlingEnchantment(),
                "preserving" when tool is Slingshot => new Enchantments.PreservingEnchantment(),
                "quincy" => new QuincyEnchantment(),
                "spreading" => new SpreadingEnchantment(),

                // vanilla weapon enchants
                "haymaker" => new HaymakerEnchantment(),
                "v_artful" => new ArtfulEnchantment(),
                "v_bugkiller" => new BugKillerEnchantment(),
                "v_crusader" => new CrusaderEnchantment(),
                "v_vampiric" => new VampiricEnchantment(),
                "v_magic" or "v_sunburst" => new MagicEnchantment(),

                _ => null,
            };

            if (enchantment is null)
            {
                Log.W($"Ignoring unknown enchantment {args[0]}.");
                args = args.Skip(1).ToArray();
                continue;
            }

            if (enchantment is GarnetEnchantment && !JsonAssetsIntegration.GarnetIndex.HasValue)
            {
                Log.W("Can't apply Garnet Enchantment because the Garnet gemstone was not loaded.");
                args = args.Skip(1).ToArray();
                continue;
            }

            if (!enchantment.CanApplyTo(tool))
            {
                Log.W($"Cannot apply {args[0].FirstCharToUpper()} enchantment to {tool.DisplayName}.");
                args = args.Skip(1).ToArray();
                continue;
            }

            // ensure old enchantments are replaced correctly
            tool.enchantments
                .Where(e => !e.IsForge() && !e.IsSecondaryEnchantment())
                .ForEach(e => tool.RemoveEnchantment(e));

            tool.AddEnchantment(enchantment);
            Log.I($"Applied {enchantment.GetType().Name} to {tool.DisplayName}.");

            args = args.Skip(1).ToArray();
        }

        switch (tool)
        {
            case MeleeWeapon weapon:
                weapon.Invalidate();
                break;
            case Slingshot slingshot:
                slingshot.Invalidate();
                break;
        }
    }

    /// <summary>Tell the dummies how to use the console command.</summary>
    private string GetUsage()
    {
        var result = new StringBuilder($"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers[0]} <enchantment>");
        result.Append("\n\nParameters:");
        result.Append("\n\t- <enchantment>: a weapon or slingshot enchantment");
        result.Append("\n\nExample:");
        result.Append($"\n\t- {this.Handler.EntryCommand} {this.Triggers[0]} vampiric");
        return result.ToString();
    }
}
