/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Taxes;

internal sealed class DataKeys
{
    // farmer
    internal const string SeasonIncome = "SeasonIncome";
    internal const string BusinessExpenses = "BusinessExpenses";
    internal const string PercentDeductions = "PercentDeductions";
    internal const string DebtOutstanding = "DebtOutstanding";

    internal const string LatestDueIncomeTax = "LatestDueIncomeTax";
    internal const string LatestOutstandingIncomeTax = "LatestOutstandingIncomeTax";
    internal const string LatestTaxDeductions = "LatestTaxDeductions";
    internal const string LatestDuePropertyTax = "LatestDuePropertyTax";
    internal const string LatestOutstandingPropertyTax = "LatestOutstandingPropertyTax";
    internal const string LatestAmountWithheld = "LatestAmountWithheld";

    // farm
    internal const string AgricultureValue = "AgricultureValue";
    internal const string LivestockValue = "LivestockValue";
    internal const string BuildingValue = "BuildingValue";
    internal const string UsedTiles = "UsedTiles";
    internal const string UsableTiles = "UsableTiles";
}
