/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Core.ConfigMenu;

/// <summary>Constructs the GenericModConfigMenu integration.</summary>
internal sealed partial class GenericModConfigMenuCore
{
    /// <summary>Register the Taxes menu.</summary>
    private void RegisterTaxes()
    {
        this
            .AddPage(OverhaulModule.Taxes.Namespace, () => "Tax Settings")

            .AddNumberField(
                () => "Income Tax Ceiling",
                () => "The taxable percentage of shipped products at the highest tax bracket.",
                config => config.Taxes.IncomeTaxCeiling,
                (config, value) => config.Taxes.IncomeTaxCeiling = value,
                0f,
                2f)
            .AddNumberField(
                () => "Annual Interest",
                () =>
                    "The interest rate charged annually over any outstanding debt. Interest is accrued daily at a rate of 1/112 the annual rate.",
                config => config.Taxes.AnnualInterest,
                (config, value) => config.Taxes.AnnualInterest = value,
                0f,
                2f)
            .AddCheckbox(
                () => "Deductible Building Expenses",
                () => "Whether or not any gold spent constructing farm buildings is tax-deductible.",
                config => config.Taxes.DeductibleBuildingExpenses,
                (config, value) => config.Taxes.DeductibleBuildingExpenses = value)
            .AddCheckbox(
                () => "Deductible Tool Expenses",
                () => "Whether or not any gold spent upgrading tools is tax-deductible.",
                config => config.Taxes.DeductibleBuildingExpenses,
                (config, value) => config.Taxes.DeductibleBuildingExpenses = value);
    }
}
