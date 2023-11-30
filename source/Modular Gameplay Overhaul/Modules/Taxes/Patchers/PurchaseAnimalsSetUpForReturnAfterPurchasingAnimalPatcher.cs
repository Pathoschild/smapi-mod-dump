/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Taxes.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Taxes.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class PurchaseAnimalsSetUpForReturnAfterPurchasingAnimalPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="PurchaseAnimalsSetUpForReturnAfterPurchasingAnimalPatcher"/> class.</summary>
    internal PurchaseAnimalsSetUpForReturnAfterPurchasingAnimalPatcher()
    {
        this.Target = this.RequireMethod<PurchaseAnimalsMenu>(nameof(PurchaseAnimalsMenu.setUpForReturnAfterPurchasingAnimal));
    }

    #region harmony patches

    /// <summary>Patch to deduct animal expenses.</summary>
    [HarmonyPostfix]
    private static void PurchaseAnimalsMenuReceiveLeftClickPostfix(FarmAnimal ___animalBeingPurchased, int ___priceOfAnimal)
    {
        if (TaxesModule.Config.DeductibleAnimalExpenses <= 0f)
        {
            return;
        }

        var deductible = (int)(___priceOfAnimal * TaxesModule.Config.DeductibleAnimalExpenses);
        if (Game1.player.ShouldPayTaxes())
        {
            Game1.player.Increment(DataKeys.BusinessExpenses, deductible);
        }
        else
        {
            Broadcaster.MessageHost(
                deductible.ToString(),
                OverhaulModule.Taxes.Namespace + '/' + DataKeys.BusinessExpenses);
        }
    }

    #endregion harmony patches
}
