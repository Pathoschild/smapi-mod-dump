/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Taxes.Framework.Events;

#region using directives

using Common;
using Common.Data;
using Common.Events;
using Extensions;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Globalization;
using System.Linq;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class TaxDayEndingEvent : DayEndingEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TaxDayEndingEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        if (Game1.dayOfMonth == 0 && Game1.currentSeason == "spring" && Game1.year == 1)
            Game1.player.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/TaxIntro");

        var dayIncome = Game1.getFarm().getShippingBin(Game1.player).Sum(item =>
            item is SObject @object ? @object.sellToStorePrice() * @object.Stack : item.salePrice() / 2);
        if (dayIncome > 0 && !Game1.player.hasOrWillReceiveMail($"{ModEntry.Manifest.UniqueID}/TaxIntro"))
            Game1.player.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/TaxIntro");

        switch (Game1.dayOfMonth)
        {
            case 28 when ModEntry.ProfessionsAPI is not null && Game1.player.professions.Contains(Farmer.mariner):
                {
                    var deductible = ModEntry.ProfessionsAPI.GetConservationistEffectiveTaxBonus(Game1.player);
                    if (deductible <= 0f) break;

                    ModDataIO.WriteTo(Game1.player, "DeductionPct",
                        deductible.ToString(CultureInfo.InvariantCulture));
                    ModEntry.ModHelper.GameContent.InvalidateCache("Data/mail");
                    Game1.player.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/TaxDeduction");
                    Log.I(
                        FormattableString.CurrentCulture(
                            $"Farmer {Game1.player.Name} is eligible for tax deductions of {deductible:p0}.") +
                        (deductible >= 1f
                            ? $" No taxes will be charged for {Game1.currentSeason}."
                            : string.Empty) +
                        " An FRS deduction notice has been posted for tomorrow.");
                    break;
                }
            case 1 when ModDataIO.ReadFrom<float>(Game1.player, "DeductionPct") < 1f:
                {
                    if (Game1.currentSeason == "spring" && Game1.year == 1) break;

                    var amountDue = Game1.player.DoTaxes();
                    ModEntry.LatestAmountDue.Value = amountDue;
                    if (amountDue <= 0) break;

                    int amountPaid;
                    if (Game1.player.Money + dayIncome >= amountDue)
                    {
                        Game1.player.Money -= amountDue;
                        amountPaid = amountDue;
                        amountDue = 0;
                        ModEntry.ModHelper.GameContent.InvalidateCache("Data/mail");
                        Game1.player.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/TaxNotice");
                        Log.I("Amount due has been paid in full." +
                              " An FRS taxation notice has been posted for tomorrow.");
                    }
                    else
                    {
                        Game1.player.Money = 0;
                        amountPaid = Game1.player.Money + dayIncome;
                        amountDue -= amountPaid;
                        ModDataIO.Increment(Game1.player, "DebtOutstanding", amountDue);
                        ModEntry.ModHelper.GameContent.InvalidateCache("Data/mail");
                        Game1.player.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/TaxOutstanding");
                        Log.I(
                            $"{Game1.player.Name} did not carry enough funds to cover the amount due." +
                            $"\n\t- Amount charged: {amountPaid}g" +
                            $"\n\t- Outstanding debt: {amountDue}g." +
                            " An FRS collection notice has been posted for tomorrow.");
                    }

                    ModDataIO.WriteTo(Game1.player, "SeasonIncome", "0");

                    break;
                }
            default:
                {
                    var debtOutstanding = ModDataIO.ReadFrom<int>(Game1.player, "DebtOutstanding");
                    if (debtOutstanding <= 0) break;

                    if (dayIncome >= debtOutstanding)
                    {
                        dayIncome -= debtOutstanding;
                        debtOutstanding = 0;
                        Log.I(
                            $"{Game1.player.Name} has successfully paid off their outstanding debt and will resume earning income from Shipping Bin sales.");
                    }
                    else
                    {
                        debtOutstanding -= dayIncome;
                        var interest = (int)Math.Round(debtOutstanding * ModEntry.Config.AnnualInterest / 112f);
                        debtOutstanding += interest;
                        Log.I(
                            $"{Game1.player.Name}'s outstanding debt has accrued {interest}g interest and is now worth {debtOutstanding}g.");
                        dayIncome = 0;
                    }

                    Game1.player.Money = dayIncome;
                    ModDataIO.WriteTo(Game1.player, "DebtOutstanding", debtOutstanding.ToString());
                    break;
                }
        }

        ModDataIO.Increment(Game1.player, "SeasonIncome", dayIncome);
    }
}