/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class PowerfulEnchantmentApplyToPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal PowerfulEnchantmentApplyToPatch()
    {
        Target = RequireMethod<PowerfulEnchantment>("_ApplyTo");
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