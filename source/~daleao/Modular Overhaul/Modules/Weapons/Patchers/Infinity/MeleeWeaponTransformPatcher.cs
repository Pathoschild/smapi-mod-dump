/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Patchers.Infinity;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Weapons.Enchantments;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponTransformPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponTransformPatcher"/> class.</summary>
    internal MeleeWeaponTransformPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.transform));
    }

    #region harmony patches

    /// <summary>Convert cursed -> blessed enchantment + galaxysoul -> infinity enchatnment.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponTransformPrefix(MeleeWeapon __instance, int newIndex)
    {
        if (!WeaponsModule.Config.InfinityPlusOne)
        {
            return true; // run original logic
        }

        try
        {
            __instance.CurrentParentTileIndex = newIndex;
            __instance.InitialParentTileIndex = newIndex;
            __instance.IndexOfMenuItemView = newIndex;
            __instance.appearance.Value = -1;
            switch (newIndex)
            {
                // dark sword -> holy blade
                case ItemIDs.HolyBlade:
                    __instance.RemoveEnchantment(__instance.GetEnchantmentOfType<CursedEnchantment>());
                    __instance.AddEnchantment(new BlessedEnchantment());
                    break;
                // galaxy -> infinity
                case ItemIDs.InfinityBlade:
                case ItemIDs.InfinityDagger:
                case ItemIDs.InfinityGavel:
                    __instance.RemoveEnchantment(__instance.GetEnchantmentOfType<GalaxySoulEnchantment>());
                    __instance.AddEnchantment(new InfinityEnchantment());
                    break;
            }

            __instance.RecalculateAppliedForges();
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
