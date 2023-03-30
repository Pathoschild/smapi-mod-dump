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

using Weapons.Extensions;
using DaLion.Shared.Commands;
using StardewValley.Tools;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class RefreshWeaponCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="RefreshWeaponCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal RefreshWeaponCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "refresh_weapon", "refresh" };

    /// <inheritdoc />
    public override string Documentation =>
        "Refreshes the stats of the currently selected weapon, randomizing if necessary.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (Game1.player.CurrentTool is not MeleeWeapon weapon || weapon.isScythe())
        {
            Log.W("You must select a weapon first.");
            return;
        }

        var bias = 0d;
        if (args.Length > 0 && double.TryParse(args[0], out bias))
        {
        }

        weapon.RandomizeDamage(bias);
        MeleeWeapon_Stats.Invalidate(weapon);
        Log.I($"Refreshed the stats of {weapon.Name}.");
    }
}
