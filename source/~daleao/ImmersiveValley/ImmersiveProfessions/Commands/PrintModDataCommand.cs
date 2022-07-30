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
using Common.Data;
using JetBrains.Annotations;
using StardewValley;
using System;
using static System.FormattableString;
using static System.String;

#endregion using directives

[UsedImplicitly]
internal sealed class PrintModDataCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal PrintModDataCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "data";

    /// <inheritdoc />
    public override string Documentation => "Print the current value of all mod data fields.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var message = $"Farmer {Game1.player.Name}'s mod data:";
        var value = ModDataIO.ReadFrom(Game1.player, "EcologistItemsForaged");
        message += "\n\t- " +
                   (!IsNullOrEmpty(value)
                       ? $"EcologistItemsForaged: {value} ({ModEntry.Config.ForagesNeededForBestQuality - int.Parse(value)} needed for best quality)"
                       : "Mod data does not contain an entry for EcologistItemsForaged.");

        value = ModDataIO.ReadFrom(Game1.player, "GemologistMineralsCollected");
        message += "\n\t- " +
                   (!IsNullOrEmpty(value)
                       ? $"GemologistMineralsCollected: {value} ({ModEntry.Config.MineralsNeededForBestQuality - int.Parse(value)} needed for best quality)"
                       : "Mod data does not contain an entry for GemologistMineralsCollected.");

        value = ModDataIO.ReadFrom(Game1.player, "ProspectorHuntStreak");
        message += "\n\t- " +
                   (!IsNullOrEmpty(value)
                       ? $"ProspectorHuntStreak: {value} (affects treasure quality)"
                       : "Mod data does not contain an entry for ProspectorHuntStreak.");

        value = ModDataIO.ReadFrom(Game1.player, "ScavengerHuntStreak");
        message += "\n\t- " +
                   (!IsNullOrEmpty(value)
                       ? $"ScavengerHuntStreak: {value} (affects treasure quality)"
                       : "Mod data does not contain an entry for ScavengerHuntStreak.");

        value = ModDataIO.ReadFrom(Game1.player, "ConservationistTrashCollectedThisSeason");
        message += "\n\t- " +
                   (!IsNullOrEmpty(value)
                       ? CurrentCulture(
                           // ReSharper disable once PossibleLossOfFraction
                           $"ConservationistTrashCollectedThisSeason: {value} (expect a {Math.Min(int.Parse(value) / ModEntry.Config.TrashNeededPerTaxBonusPct / 100f, ModEntry.Config.ConservationistTaxBonusCeiling):p0} tax deduction next season)")
                       : "Mod data does not contain an entry for ConservationistTrashCollectedThisSeason.");

        value = ModDataIO.ReadFrom(Game1.player, "ConservationistActiveTaxBonusPct");
        message += "\n\t- " +
                   (!IsNullOrEmpty(value)
                       ? CurrentCulture($"ConservationistActiveTaxBonusPct: {float.Parse(value):p0}")
                       : "Mod data does not contain an entry for ConservationistActiveTaxBonusPct.");

        Log.I(message);
    }
}