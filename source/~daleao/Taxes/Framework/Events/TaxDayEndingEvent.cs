/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Taxes.Framework.Events;

#region using directives

using System.Globalization;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using DaLion.Taxes.Framework.Integrations;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="TaxDayEndingEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class TaxDayEndingEvent(EventManager? manager = null)
    : DayEndingEvent(manager ?? TaxesMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        var taxpayer = Game1.player;
        if (!taxpayer.ShouldPayTaxes())
        {
            // clear any outdated data just in case
            Data.Write(taxpayer, DataKeys.SeasonIncome, null);
            Data.Write(taxpayer, DataKeys.BusinessExpenses, null);
            Data.Write(taxpayer, DataKeys.PercentDeductions, null);
            Data.Write(taxpayer, DataKeys.DebtOutstanding, null);
            return;
        }

        if (Game1.dayOfMonth == 0 && Game1.currentSeason == "spring" && Game1.year == 1)
        {
            PostalService.Send(Mail.LewisIntro);
            PostalService.Send(Mail.FrsIntro);
        }

        var amountSold = Game1.game1.GetTotalSoldByPlayer(taxpayer);
        if (amountSold > 0 && !PostalService.HasSent(Mail.FrsIntro))
        {
            PostalService.Send(Mail.FrsIntro);
        }

        Log.D(
            amountSold > 0
                ? $"{Game1.player} sold items worth a total of {amountSold}g on day {Game1.dayOfMonth} of {Game1.currentSeason}."
                : $"No items were sold on day {Game1.dayOfMonth} of {Game1.currentSeason}.");

        var dayIncome = amountSold;
        if (Game1.currentSeason == "spring" && Game1.year == 1)
        {
            goto skipSwitch;
        }

        switch (Game1.dayOfMonth)
        {
            // handle Conservationist profession
            case 28:
            {
                CheckIncomeStatement(taxpayer);
                PostalService.Send(CheckDeductions(taxpayer) ? Mail.FrsDeduction : Mail.FrsNotice);
                if (Game1.currentSeason == "spring")
                {
                    CheckPropertyStatement(taxpayer);
                    PostalService.Send(Mail.LewisNotice);
                }

                goto default;
            }

            case 8:
            case 22:
            {
                if (taxpayer.IsMainPlayer)
                {
                    Game1.getFarm().Appraise();
                }

                goto default;
            }

            default:
            {
                if (Game1.dayOfMonth == Config.IncomeTaxDay)
                {
                    DebitIncomeStatement(taxpayer, ref dayIncome);
                }

                if (taxpayer.IsMainPlayer && Game1.currentSeason == "spring" && Game1.dayOfMonth == Config.PropertyTaxDay)
                {
                    DebitPropertyStatement(taxpayer, ref dayIncome);
                }

                DebitOutstanding(taxpayer, ref dayIncome);
                break;
            }
        }

        skipSwitch:
        if (dayIncome < amountSold)
        {
            Data.Write(taxpayer, DataKeys.Withheld, (amountSold - dayIncome).ToString());
            TaxesMod.EventManager.Enable<TaxDayStartedEvent>();
            Log.T(dayIncome > 0
                ? $"Actual income was decreased by {amountSold - dayIncome}g after debts and payments."
                : "Day's income was entirely consumed by debts and payments.");
        }

        Data.Increment(taxpayer, DataKeys.SeasonIncome, dayIncome);
    }

    private static bool CheckDeductions(Farmer taxpayer)
    {
        if (!ProfessionsIntegration.IsValueCreated || !ProfessionsIntegration.Instance.IsLoaded ||
            !taxpayer.professions.Contains(Farmer.mariner))
        {
            return false;
        }

        var professionsApi = ProfessionsIntegration.Instance.ModApi;
        var deductible = professionsApi.GetConservationistTaxDeduction();
        if (deductible <= 0f)
        {
            return false;
        }

        deductible = Math.Min(
            deductible,
            professionsApi.GetConfig().ConservationistTaxDeductionCeiling);

        Data.Write(taxpayer, DataKeys.PercentDeductions, deductible.ToString(CultureInfo.InvariantCulture));
        Log.I(
            FormattableString.CurrentCulture(
                $"Farmer {taxpayer.Name} is eligible for income tax deductions of {deductible:0.0%}.") +
            (deductible >= 1f
                ? $" No income taxes will be charged for {Game1.currentSeason}."
                : string.Empty));
        return true;
    }

    private static void CheckIncomeStatement(Farmer taxpayer)
    {
        var (amountDue, _, _, _, _) = RevenueService.CalculateTaxes(taxpayer);
        Data.Write(taxpayer, DataKeys.AccruedIncomeTax, amountDue.ToString());
    }

    private static void DebitIncomeStatement(Farmer taxpayer, ref int dayIncome)
    {
        var amountDue = Data.ReadAs<int>(taxpayer, DataKeys.AccruedIncomeTax);
        if (amountDue <= 0)
        {
            return;
        }

        int amountPaid;
        if (taxpayer.Money >= amountDue)
        {
            taxpayer.Money -= amountDue;
            amountPaid = amountDue;
            amountDue = 0;
            Log.I("Income tax due was paid in full.");
        }
        else
        {
            amountPaid = taxpayer.Money;
            amountDue -= amountPaid;
            taxpayer.Money = 0;
            if (amountDue >= dayIncome)
            {
                var outstanding = amountDue - dayIncome;
                dayIncome = 0;
                var penalties = Math.Max((int)(outstanding * Config.IncomeTaxLatenessFine), 100);
                Data.Increment(taxpayer, DataKeys.DebtOutstanding, outstanding + penalties);
                Data.Write(taxpayer, DataKeys.OutstandingIncomeTax, (outstanding + penalties).ToString());
                PostalService.Send(Mail.FrsOutstanding);
                Log.I(
                    $"{taxpayer.Name} did not carry enough funds to cover the income tax due." +
                    $"\n\t- Amount charged: {amountPaid}g" +
                    $"\n\t- Outstanding debt: {outstanding}g (+{penalties}g in penalties).");
            }
            else
            {
                dayIncome -= amountDue;
            }
        }

        TaxesMod.EventManager.Enable<TaxDayStartedEvent>();
        Data.Write(taxpayer, DataKeys.SeasonIncome, "0");
        Data.Write(taxpayer, DataKeys.BusinessExpenses, "0");
    }

    private static void CheckPropertyStatement(Farmer taxpayer)
    {
        var farm = Game1.getFarm();
        var agricultureValue = Data.ReadAs<int>(farm, DataKeys.AgricultureValue);
        var livestockValue = Data.ReadAs<int>(farm, DataKeys.LivestockValue);
        var buildingValue = Data.ReadAs<int>(farm, DataKeys.BuildingValue);
        var usableTiles = Data.ReadAs(farm, DataKeys.UsableTiles, -1);
        if (usableTiles < 0)
        {
            var origin = farm.GetMainFarmHouseEntry();
            usableTiles = origin.FloodFill(
                farm.Map.Layers[0].TileWidth,
                farm.Map.Layers[0].TileHeight,
                p => farm.doesTileHaveProperty(p.X, p.Y, "Diggable", "Back") is not null).Count;
            Data.Write(farm, DataKeys.UsableTiles, usableTiles.ToString());
        }

        var currentUsePct = Data.ReadAs<float>(farm, DataKeys.UsedTiles) / usableTiles;
        var amountDue = (int)(((agricultureValue + livestockValue) * currentUsePct * Config.UsedTileTaxRate) +
                              ((agricultureValue + livestockValue) * (1f - currentUsePct) * Config.UnusedTileTaxRate) +
                              (buildingValue * Config.BuildingTaxRate));
        Data.Write(taxpayer, DataKeys.AccruedPropertyTax, amountDue.ToString());
    }

    private static void DebitPropertyStatement(Farmer taxpayer, ref int dayIncome)
    {
        var amountDue = Data.ReadAs<int>(taxpayer, DataKeys.AccruedPropertyTax);
        if (amountDue <= 0)
        {
            return;
        }

        int amountPaid;
        if (taxpayer.Money >= amountDue)
        {
            taxpayer.Money -= amountDue;
            amountPaid = amountDue;
            amountDue = 0;
            Log.I("Property tax due was paid in full.");
        }
        else
        {
            amountPaid = taxpayer.Money;
            amountDue -= amountPaid;
            taxpayer.Money = 0;
            if (amountDue > dayIncome)
            {
                var outstanding = amountDue - dayIncome;
                dayIncome = 0;
                var penalties = Math.Max((int)(outstanding * Config.PropertyTaxLatenessFine), 500);
                Data.Increment(taxpayer, DataKeys.DebtOutstanding, outstanding + penalties);
                Data.Write(taxpayer, DataKeys.OutstandingPropertyTax, (outstanding + penalties).ToString());
                PostalService.Send(Mail.LewisOutstanding);
                Log.I(
                    $"{taxpayer.Name} did not carry enough funds to cover the property tax due." +
                    $"\n\t- Amount charged: {amountPaid}g" +
                    $"\n\t- Outstanding debt: {outstanding}g (+{penalties}g in penalties).");
            }
            else
            {
                dayIncome -= amountDue;
            }
        }

        TaxesMod.EventManager.Enable<TaxDayStartedEvent>();
        var farm = Game1.getFarm();
        Data.Write(farm, DataKeys.AgricultureValue, "0");
        Data.Write(farm, DataKeys.LivestockValue, "0");
        Data.Write(farm, DataKeys.BuildingValue, "0");
        Data.Write(farm, DataKeys.UsedTiles, "0");
    }

    private static void DebitOutstanding(Farmer taxpayer, ref int dayIncome)
    {
        var debtOutstanding = Data.ReadAs<int>(taxpayer, DataKeys.DebtOutstanding);
        if (debtOutstanding <= 0)
        {
            return;
        }

        // only seize shipping bin income
        int withheld;
        if (dayIncome >= debtOutstanding)
        {
            withheld = debtOutstanding;
            debtOutstanding = 0;
            Log.I(
                $"{taxpayer.Name} has successfully paid off their outstanding debt and will resume earning income from Shipping Bin sales.");
        }
        else
        {
            withheld = dayIncome;
            debtOutstanding -= withheld;
            var interest = (int)Math.Round(debtOutstanding * Config.AnnualInterest / 112f);
            debtOutstanding += interest;
            Log.I(
                $"{taxpayer.Name}'s outstanding debt has accrued {interest}g interest and is now worth {debtOutstanding}g.");
        }

        dayIncome -= withheld;
        Data.Write(taxpayer, DataKeys.DebtOutstanding, debtOutstanding.ToString());
    }
}
