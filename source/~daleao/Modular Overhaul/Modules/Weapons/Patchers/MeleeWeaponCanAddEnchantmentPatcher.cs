/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Weapons.Enchantments;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponCanAddEnchantmentPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponCanAddEnchantmentPatcher"/> class.</summary>
    internal MeleeWeaponCanAddEnchantmentPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.CanAddEnchantment));
    }

    #region harmony patches

    /// <summary>Prevent Mythic weapons from receiving additional enchantments.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponCanAddEnchantmentPrefix(
        MeleeWeapon __instance, ref bool __result, BaseEnchantment enchantment)
    {
        if (enchantment.IsForge() || enchantment.IsSecondaryEnchantment())
        {
            return true; // don't run original logic
        }

        __result = !__instance.HasAnyEnchantmentOf(
            typeof(KillerBugEnchantment),
            typeof(LavaEnchantment),
            typeof(NeedleEnchantment),
            typeof(NeptuneEnchantment),
            typeof(ObsidianEnchantment),
            typeof(YetiEnchantment));
        return false; // run original logic
    }

    #endregion harmony patches
}
