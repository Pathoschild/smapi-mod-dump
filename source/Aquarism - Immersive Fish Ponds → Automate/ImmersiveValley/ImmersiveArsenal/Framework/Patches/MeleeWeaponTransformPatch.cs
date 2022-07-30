/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common.Data;
using Enchantments;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponTransformPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponTransformPatch()
    {
        Target = RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.transform));
    }

    #region harmony patches

    /// <summary>Add Dark Sword mod data</summary>
    [HarmonyPostfix]
    private static void MeleeWeaponTransformPostfix(MeleeWeapon __instance, int newIndex)
    {
        if (!ModEntry.Config.InfinityPlusOneWeapons) return;

        switch (newIndex)
        {
            // dark sword -> holy blade
            case Constants.HOLY_BLADE_INDEX_I:
                ModDataIO.WriteTo(__instance, "EnemiesSlain", null);
                __instance.enchantments.Remove(__instance.GetEnchantmentOfType<DemonicEnchantment>());
                __instance.enchantments.Add(new HolyEnchantment());
                break;
            // galaxy -> infinity
            case Constants.INFINITY_BLADE_INDEX_I:
            case Constants.INFINITY_DAGGER_INDEX_I:
            case Constants.INFINITY_CLUB_INDEX_I:
                __instance.enchantments.Remove(__instance.GetEnchantmentOfType<GalaxySoulEnchantment>());
                __instance.enchantments.Add(new InfinityEnchantment());
                __instance.specialItem = true;
                break;
        }
    }

    #endregion harmony patches
}