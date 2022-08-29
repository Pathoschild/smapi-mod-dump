/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

using DaLion.Stardew.Arsenal.Framework.Enchantments;

namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseWeaponEnchantmentCanApplyToPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal BaseWeaponEnchantmentCanApplyToPatch()
    {
        Target = RequireMethod<BaseWeaponEnchantment>("CanApplyTo");
    }

    #region harmony patches

    /// <summary>Allow Slingshot forges.</summary>
    [HarmonyPostfix]
    private static void BaseWeaponEnchantmentCanApplyToPostfix(BaseWeaponEnchantment __instance, ref bool __result,
        Item item)
    {
        if (__instance is CarvingEnchantment or CleavingEnchantment or EnergizedEnchantment or TributeEnchantment)
        {
            __result = ModEntry.Config.NewWeaponEnchants;
            return;
        }

        if (item is not Slingshot || __instance.IsSecondaryEnchantment()) return;

        __result = __instance.IsForge() && ModEntry.Config.EnableSlingshotForges ||
                   __instance is BugKillerEnchantment or CrusaderEnchantment &&
                   ModEntry.Config.EnableSlingshotEnchants;
    }

    #endregion harmony patches
}