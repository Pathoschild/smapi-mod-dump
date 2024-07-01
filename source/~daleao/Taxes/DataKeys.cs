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
    internal const string AccruedIncomeTax = "AccruedIncomeTax";
    internal const string OutstandingIncomeTax = "OutstandingIncomeTax";
    internal const string AccruedPropertyTax = "AccruedPropertyTax";
    internal const string OutstandingPropertyTax = "OutstandingPropertyTax";
    internal const string OvernightDebit = "OvernightDebit";
    internal const string Withheld = "Withheld";

    // farm
    internal const string AgricultureValue = "AgricultureValue";
    internal const string LivestockValue = "LivestockValue";
    internal const string BuildingValue = "BuildingValue";
    internal const string UsedTiles = "UsedTiles";
    internal const string UsableTiles = "UsableTiles";
}
