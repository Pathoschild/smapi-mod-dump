/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Taxes.Extensions;

#region using directives

using Common;
using Common.Data;
using StardewValley;
using System;
using static System.FormattableString;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public static class FarmerExtensions
{
    /// <summary>Calculate due income tax for the player.</summary>
    public static int DoTaxes(this Farmer farmer)
    {
        var income = ModDataIO.ReadFrom<int>(farmer, "SeasonIncome");
        var deductible = ModDataIO.ReadFrom<float>(farmer, "DeductionPct");
        var taxable = (int)(income * (1f - deductible));
        var bracket = Framework.Utils.GetTaxBracket(taxable);
        var due = (int)Math.Round(taxable * bracket);
        Log.I(
            $"Accounting results for {farmer.Name} over the closing {Game1.game1.GetPrecedingSeason()} season, year {Game1.year}:" +
            $"\n\t- Total income: {income}g" +
            CurrentCulture($"\n\t- Tax deductions: {deductible:p0}") +
            $"\n\t- Taxable income: {taxable}g" +
            CurrentCulture($"\n\t- Tax bracket: {bracket:p0}") +
            $"\n\t- Total due income tax: {due}g."
        );
        return due;
    }
}