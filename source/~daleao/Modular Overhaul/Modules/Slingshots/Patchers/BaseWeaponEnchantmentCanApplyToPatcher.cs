/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Slingshots.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseWeaponEnchantmentCanApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BaseWeaponEnchantmentCanApplyToPatcher"/> class.</summary>
    internal BaseWeaponEnchantmentCanApplyToPatcher()
    {
        this.Target = this.RequireMethod<BaseWeaponEnchantment>("CanApplyTo");
    }

    #region harmony patches

    /// <summary>Allow Slingshot gemstone enchantments.</summary>
    [HarmonyPrefix]
    private static bool BaseWeaponEnchantmentCanApplyToPostfix(
        BaseWeaponEnchantment __instance, ref bool __result, Item item)
    {
        if (item is not Slingshot || __instance.IsSecondaryEnchantment() || !__instance.IsForge() ||
            !SlingshotsModule.Config.EnableEnchantments)
        {
            return true; // run original logic
        }

        __result = true;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
