/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Enchantments.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using DaLion.Overhaul.Modules.Weapons.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class AmethystEnchantmentUnapplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AmethystEnchantmentUnapplyToPatcher"/> class.</summary>
    internal AmethystEnchantmentUnapplyToPatcher()
    {
        this.Target = this.RequireMethod<AmethystEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Amethyst enchant.</summary>
    [HarmonyPrefix]
    private static bool AmethystEnchantmentUnapplyToPrefix(AmethystEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon || !EnchantmentsModule.Config.RebalancedForges)
        {
            return true; // run original logic
        }

        weapon.knockback.Value -= 0.1f * __instance.GetLevel();
        return false; // don't run original logic
    }

    /// <summary>Reset cached stats.</summary>
    [HarmonyPostfix]
    private static void AmethystEnchantmentUnapplyPostfix(Item item)
    {
        switch (item)
        {
            case MeleeWeapon weapon when WeaponsModule.ShouldEnable:
                weapon.Invalidate();
                break;
            case Slingshot slingshot when SlingshotsModule.ShouldEnable:
                slingshot.Invalidate();
                break;
        }
    }

    #endregion harmony patches
}
