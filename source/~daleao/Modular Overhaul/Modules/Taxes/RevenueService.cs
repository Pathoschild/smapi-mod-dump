/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Taxes;

#region using directives

using System.Collections.Immutable;
using System.Linq;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Stardew;
using static System.FormattableString;

#endregion using directives

/// <summary>Responsible for collecting federal taxes and administering the Ferngill Revenue Code.</summary>
internal static class RevenueService
{
    internal static ImmutableDictionary<int, float> TaxByIncomeBrackets { get; } = TaxesModule.Config.IncomeBrackets
        .Zip(TaxesModule.Config.IncomeTaxPerBracket, (key, value) => new { key, value })
        .ToImmutableDictionary(p => p.key, p => p.value);

    /// <summary>Calculates due income tax for the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The amount of income tax due in gold, along with other relevant stats.</returns>
    internal static (int Due, int Income, int Expenses, float Deductions, int Taxable) CalculateTaxes(Farmer farmer)
    {
        var income = farmer.Read<int>(DataKeys.SeasonIncome);
        var expenses = Math.Min(farmer.Read<int>(DataKeys.BusinessExpenses), income);
        var deductions = farmer.Read<float>(DataKeys.PercentDeductions);
        var taxable = (int)((income - expenses) * (1f - deductions));

        var dueF = 0f;
        var tax = 0f;
        var temp = taxable;
        foreach (var bracket in TaxByIncomeBrackets.Keys)
        {
            tax = TaxByIncomeBrackets[bracket];
            if (temp > bracket)
            {
                dueF += bracket * tax;
                temp -= bracket;
            }
            else
            {
                dueF += temp * tax;
                break;
            }
        }

        var dueI = (int)Math.Round(dueF);
        Log.I(
            $"Accounting results for {farmer.Name} over the closing {SeasonExtensions.Previous()} season, year {Game1.year}:" +
            $"\n\t- Season income: {income}g" +
            $"\n\t- Business expenses: {expenses}g" +
            CurrentCulture($"\n\t- Eligible deductions: {deductions:0.0%}") +
            $"\n\t- Taxable amount: {taxable}g" +
            CurrentCulture($"\n\t- Tax bracket: {tax:0.0%}") +
            $"\n\t- Due amount: {dueI}g.");
        return (dueI, income, expenses, deductions, taxable);
    }
}
