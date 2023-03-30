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

/// <summary>The runtime state for Tax variables.</summary>
internal sealed class State
{
    internal int LatestDueIncomeTax { get; set; }

    internal int LatestOutstandingIncomeTax { get; set; }

    internal float LatestTaxDeductions { get; set; }

    internal int LatestDuePropertyTax { get; set; }

    internal int LatestOutstandingPropertyTax { get; set; }

    internal int LatestAmountWithheld { get; set; }
}
