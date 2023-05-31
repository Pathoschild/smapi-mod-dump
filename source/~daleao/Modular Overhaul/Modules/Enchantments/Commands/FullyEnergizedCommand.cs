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

using DaLion.Overhaul.Modules.Enchantments.Melee;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class FullyEnergizedCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="FullyEnergizedCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal FullyEnergizedCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "energize" };

    /// <inheritdoc />
    public override string Documentation => "Fully energizes the player, if they currently hold an Energize-enchanted weapon.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        var energized = (Game1.player.CurrentTool as MeleeWeapon)?.GetEnchantmentOfType<EnergizedEnchantment>();
        if (energized is null)
        {
            Log.W("An Energized weapon is not equipped.");
            return;
        }

        energized.Energy = 100;
    }
}
