/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Enchantments;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class JadeEnchantmentUnapplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="JadeEnchantmentUnapplyToPatcher"/> class.</summary>
    internal JadeEnchantmentUnapplyToPatcher()
    {
        this.Target = this.RequireMethod<JadeEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Rebalances Jade enchant.</summary>
    [HarmonyPrefix]
    private static bool JadeEnchantmentUnpplyToPrefix(JadeEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon || !ArsenalModule.Config.RebalancedForges)
        {
            return true; // run original logic
        }

        weapon.critMultiplier.Value -= 0.5f * __instance.GetLevel();
        return false; // don't run original logic
    }

    /// <summary>Reset cached stats.</summary>
    [HarmonyPostfix]
    private static void JadeEnchantmentUnapplyPostfix(Item item)
    {
        if (item is Tool tool and (MeleeWeapon or Slingshot))
        {
            tool.Invalidate();
        }
    }

    #endregion harmony patches
}
