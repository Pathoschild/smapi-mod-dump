/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class PowerfulEnchantmentApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="PowerfulEnchantmentApplyToPatcher"/> class.</summary>
    internal PowerfulEnchantmentApplyToPatcher()
    {
        this.Target = this.RequireMethod<PowerfulEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Rebalance powerful enchantment so it is not redundant.</summary>
    [HarmonyPrefix]
    private static bool PowerfulEnchantmentApplyToPrefix(Item item)
    {
        switch (item)
        {
            case Axe axe:
                axe.additionalPower.Value += 99;
                break;
            case Pickaxe pickaxe:
                pickaxe.additionalPower.Value += 99;
                break;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
