/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Enchantments.Commands;

#region using directives

using System.Linq;
using System.Text;
using DaLion.Overhaul.Modules.Enchantments.Gemstone;
using DaLion.Overhaul.Modules.Enchantments.Melee;
using DaLion.Overhaul.Modules.Enchantments.Ranged;
using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using DaLion.Overhaul.Modules.Weapons.VirtualProperties;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
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
    public override string[] Triggers { get; } = { "add_enchants", "add", "enchant" };

    /// <inheritdoc />
    public override string Documentation =>
        "Add the specified enchantments to the selected weapon or slingshot." + this.GetUsage();

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.W("No enchantment was specified.");
            return;
        }

        var tool = Game1.player.CurrentTool;
        if (tool is not (MeleeWeapon or Slingshot))
        {
            Log.W("You must select a weapon or slingshot first.");
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
                "haymaker" => new HaymakerEnchantment(),
                "carving" => new CarvingEnchantment(),
                "cleaving" => new CleavingEnchantment(),
                "energized" or "thunderlords" => new EnergizedEnchantment(),
                "exploding" or "blasting" => new ExplodingEnchantment(),
                "tribute" or "gold" => new TributeEnchantment(),
                "r_artful" => new NewArtfulEnchantment(),
                "bloodthirsty" => new BloodthirstyEnchantment(),

                // slingshot enchants
                "engorging" or "glutton" => new EngorgingEnchantment(),
                "gatling" => new GatlingEnchantment(),
                "preserving" => new Ranged.PreservingEnchantment(),
                "quincy" => new QuincyEnchantment(),
                "spreading" => new SpreadingEnchantment(),

                // vanilla
                "artful" => new ArtfulEnchantment(),
                "bugkiller" => new BugKillerEnchantment(),
                "crusader" => new CrusaderEnchantment(),
                "vampiric" => new VampiricEnchantment(),
                "magic" or "sunburst" => new MagicEnchantment(),

                _ => null,
            };

            if (enchantment is null)
            {
                Log.W($"Ignoring unknown enchantment {args[0]}.");
                args = args.Skip(1).ToArray();
                continue;
            }

            if (enchantment is GarnetEnchantment && !Globals.GarnetIndex.HasValue)
            {
                Log.W("You must have the Rings module enabled and the Garnet item loaded to use the Garnet enchantment.");
                args = args.Skip(1).ToArray();
                continue;
            }

            if (!enchantment.CanApplyTo(tool))
            {
                Log.W($"Cannot apply {args[0].FirstCharToUpper()} enchantment to {tool.DisplayName}.");
                args = args.Skip(1).ToArray();
                continue;
            }

            tool.AddEnchantment(enchantment);
            Log.I($"Applied {args[0].FirstCharToUpper()} enchantment to {tool.DisplayName}.");

            args = args.Skip(1).ToArray();
        }

        switch (tool)
        {
            case MeleeWeapon weapon when WeaponsModule.IsEnabled:
                weapon.Invalidate();
                break;
            case Slingshot slingshot when SlingshotsModule.IsEnabled:
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
