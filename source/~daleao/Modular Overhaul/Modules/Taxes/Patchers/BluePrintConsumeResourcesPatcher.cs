/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Taxes.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Taxes.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class BluePrintConsumeResourcesPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BluePrintConsumeResourcesPatcher"/> class.</summary>
    internal BluePrintConsumeResourcesPatcher()
    {
        this.Target = this.RequireMethod<BluePrint>(nameof(BluePrint.consumeResources));
    }

    #region harmony patches

    /// <summary>Patch to deduct building expenses.</summary>
    [HarmonyPostfix]
    private static void BluePrintConsumeResourcesPostfix(BluePrint __instance)
    {
        if ((__instance.magical && TaxesModule.Config.ExemptMagicalBuilding) ||
            TaxesModule.Config.DeductibleBuildingExpenses <= 0f)
        {
            return;
        }

        var deductible = (int)(__instance.moneyRequired * TaxesModule.Config.DeductibleBuildingExpenses);
        if (Game1.player.ShouldPayTaxes())
        {
            Game1.player.Increment(DataKeys.BusinessExpenses, deductible);
        }
        else
        {
            Broadcaster.MessageHost(
                deductible.ToString(),
                OverhaulModule.Taxes.Namespace + DataKeys.BusinessExpenses);
        }
    }

    #endregion harmony patches
}
