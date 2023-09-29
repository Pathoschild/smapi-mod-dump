/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Professions;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectGetPriceAfterMultipliersPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectGetPriceAfterMultipliersPatcher"/> class.</summary>
    internal ObjectGetPriceAfterMultipliersPatcher()
    {
        this.Target = this.RequireMethod<SObject>("getPriceAfterMultipliers");
    }

    #region harmony patches

    /// <summary>Patch to modify price multipliers for various modded professions.</summary>
    // ReSharper disable once RedundantAssignment
    [HarmonyPrefix]
    private static bool ObjectGetPriceAfterMultipliersPrefix(
        SObject __instance, ref float __result, float startPrice, long specificPlayerID)
    {
        var saleMultiplier = 1f;
        try
        {
            foreach (var farmer in Game1.getAllFarmers())
            {
                if (Game1.player.useSeparateWallets)
                {
                    if (specificPlayerID == -1)
                    {
                        if (farmer.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID || !farmer.isActive())
                        {
                            continue;
                        }
                    }
                    else if (farmer.UniqueMultiplayerID != specificPlayerID)
                    {
                        continue;
                    }
                }
                else if (!farmer.isActive())
                {
                    continue;
                }

                var multiplier = 1f;

                // professions
                if (farmer.HasProfession(Profession.Producer) &&
                    (IsAnimalOrDerivedProduct(__instance) || (__instance.ParentSheetIndex == ObjectIds.Honey &&
                                                              ProfessionsModule.Config.BeesAreAnimals)))
                {
                    multiplier += farmer.GetProducerPriceBonus();
                }

                if (farmer.HasProfession(Profession.Angler) && __instance.IsFish())
                {
                    multiplier += farmer.GetAnglerPriceBonus();
                }

                // events
                else if (farmer.eventsSeen.Contains(2120303) && __instance.IsWildBerry())
                {
                    multiplier *= 3f;
                }
                else if (farmer.eventsSeen.Contains(3910979) && __instance.IsSpringOnion())
                {
                    multiplier *= 5f;
                }

                saleMultiplier = Math.Max(saleMultiplier, multiplier);
            }
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }

        __result = startPrice * saleMultiplier;
        return false; // don't run original logic
    }

    #endregion harmony patches

    private static bool IsAnimalOrDerivedProduct(SObject @object)
    {
        return @object.Category.IsIn(
                   SObject.EggCategory,
                   SObject.MilkCategory,
                   SObject.meatCategory,
                   SObject.sellAtPierresAndMarnies) ||
               Sets.AnimalDerivedProductIds.Contains(@object.ParentSheetIndex);
    }
}
