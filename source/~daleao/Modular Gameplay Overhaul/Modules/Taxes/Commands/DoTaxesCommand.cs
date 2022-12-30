/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Taxes.Commands;

#region using directives

using DaLion.Shared.Commands;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Stardew;
using static System.FormattableString;

#endregion using directives

[UsedImplicitly]
internal sealed class DoTaxesCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="DoTaxesCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal DoTaxesCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "calculate", "do" };

    /// <inheritdoc />
    public override string Documentation =>
        "Check accounting stats for the current season-to-date, or the closing season if checking on the 1st day of the season.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (!Enum.TryParse<Season>(Game1.currentSeason, true, out var currentSeason))
        {
            Log.E($"Failed to parse the current season {Game1.currentSeason}");
            return;
        }

        var player = Game1.player;
        var forClosingSeason = Game1.dayOfMonth == 1;
        var seasonIncome = player.Read<int>(DataFields.SeasonIncome);
        var deductibleExpenses = player.Read<int>(DataFields.BusinessExpenses);
        var deductiblePct = ProfessionsModule.IsEnabled && player.professions.Contains(Farmer.mariner)
            ? forClosingSeason
                ? player.Read<float>(DataFields.PercentDeductions)
                // ReSharper disable once PossibleLossOfFraction
                : player.Read<int>(DataFields.ConservationistTrashCollectedThisSeason) / ProfessionsModule.Config.TrashNeededPerTaxBonusPct / 100f
            : 0f;
        var taxable = (int)((seasonIncome - deductibleExpenses) * (1f - deductiblePct));

        var dueF = 0f;
        var bracket = 0f;
        for (var i = 0; i < 7; i++)
        {
            bracket = RevenueService.Brackets[i];
            var threshold = RevenueService.Thresholds[bracket];
            if (taxable > threshold)
            {
                dueF += threshold * bracket;
                taxable -= threshold;
            }
            else
            {
                dueF += taxable * bracket;
                break;
            }
        }

        var dueI = (int)Math.Round(dueF);
        var debt = player.Read<int>(DataFields.DebtOutstanding);
        Log.I(
            "Accounting " + (forClosingSeason ? "report" : "projections") + " for the " +
            (forClosingSeason ? $"closing {currentSeason.Previous()}" : $"current {currentSeason}") + " season:" +
            $"\n\t- Income (season-to-date): {seasonIncome}g" +
            $"\n\t- Business expenses: {deductibleExpenses}g" +
            CurrentCulture($"\n\t- Eligible deductions: {deductiblePct:0%}") +
            $"\n\t- Taxable amount: {taxable}g" +
            CurrentCulture($"\n\t- Current tax bracket: {bracket:0%}") +
            $"\n\t- Due amount: {dueI}g." +
            $"\n\t- Outstanding debt: {debt}g." +
            $"\nRequested on {Game1.currentSeason} {Game1.dayOfMonth}, year {Game1.year}.");
    }
}
