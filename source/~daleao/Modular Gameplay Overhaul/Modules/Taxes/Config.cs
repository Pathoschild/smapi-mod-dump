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

using Newtonsoft.Json;

#endregion using directives

/// <summary>The user-configurable settings for Taxes.</summary>
public sealed class Config : Shared.Configs.Config
{
    /// <summary>
    ///     Gets the interest rate charged annually over any outstanding debt. Interest is accrued daily at a rate of 1/112 the
    ///     annual rate.
    /// </summary>
    [JsonProperty]
    public float AnnualInterest { get; internal set; } = 0.11f;

    /// <summary>Gets the taxable percentage of shipped products at the highest tax bracket.</summary>
    [JsonProperty]
    public float IncomeTaxCeiling { get; internal set; } = 0.37f;

    /// <summary>Gets a value indicating whether or not any gold spent on animal purchases and supplies should be tax-deductible.</summary>
    [JsonProperty]
    public bool DeductibleAnimalExpenses { get; internal set; } = true;

    /// <summary>Gets a value indicating whether or not any gold spent constructing farm buildings should be tax-deductible.</summary>
    [JsonProperty]
    public bool DeductibleBuildingExpenses { get; internal set; } = true;

    /// <summary>Gets a value indicating whether or not any gold spent on seed purchases should be tax-deductible.</summary>
    [JsonProperty]
    public bool DeductibleSeedExpenses { get; internal set; } = true;

    /// <summary>Gets a value indicating whether or not any gold spent on tool purchases and upgrades should be tax-deductible.</summary>
    [JsonProperty]
    public bool DeductibleToolExpenses { get; internal set; } = true;

    /// <summary>Gets a value indicating the list of extra objects that should be tax-deductible.</summary>
    [JsonProperty]
    public string[] DeductibleObjects { get; internal set; } = Array.Empty<string>();
}
