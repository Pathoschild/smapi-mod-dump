/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

// ReSharper disable PossibleLossOfFraction
namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponSalePricePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponSalePricePatcher"/> class.</summary>
    internal MeleeWeaponSalePricePatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.salePrice));
    }

    #region harmony patches

    /// <summary>Adjust weapon level.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponGetItemLevelPrefix(MeleeWeapon __instance, ref int __result)
    {
        if (!ArsenalModule.Config.Weapons.EnableRebalance)
        {
            return true; // run original logic
        }

        try
        {
            var tier = WeaponTier.GetFor(__instance);
            if (tier == WeaponTier.Untiered)
            {
                return true; // run original logic
            }

            if (tier == WeaponTier.Masterwork)
            {
                __result = __instance.Name.StartsWith("Dwarven") ? tier.Price : (int)(tier.Price * 1.5);
                __result *= 2;
                return false; // don't run original logic
            }

            __result = tier.Price * 2;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
