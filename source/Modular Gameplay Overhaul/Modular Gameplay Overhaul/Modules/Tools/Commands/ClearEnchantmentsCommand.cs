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
using DaLion.Shared.Commands;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ClearEnchantmentsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="ClearEnchantmentsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal ClearEnchantmentsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "clear_enchants", "clear", "reset" };

    /// <inheritdoc />
    public override string Documentation => "Remove all enchantments from the selected weapon or slingshot.";

    /// <inheritdoc />
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1012:Opening braces should be spaced correctly", Justification = "Paradoxical.")]
    public override void Callback(string[] args)
    {
        if (Game1.player.CurrentTool is not ({ } tool and (Axe or Hoe or Pickaxe or WateringCan or FishingRod)))
        {
            Log.W("You must select a tool first.");
            return;
        }

        foreach (var enchantment in tool.enchantments)
        {
            tool.RemoveEnchantment(enchantment);
        }

        Log.I($"Removed all enchantments from {tool.DisplayName}.");
    }
}
