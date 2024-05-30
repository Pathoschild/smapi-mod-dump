/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Chargeable.Framework.Patchers;

#region using directives

using HarmonyLib;
using StardewValley.Enchantments;
using StardewValley.Tools;

#endregion using directives

[HarmonyPatch(typeof(ReachingToolEnchantment), nameof(ReachingToolEnchantment.CanApplyTo))]
internal sealed class ReachingToolEnchantmentCanApplyToPatcher
{
    /// <summary>Allow apply Reaching enchant to Axe and Pick.</summary>
    // ReSharper disable once RedundantAssignment
    private static void Postfix(ref bool __result, Item item)
    {
        if (__result)
        {
            return;
        }

        if (item is Tool tool && ((tool is Axe && Config.Axe.AllowReachingEnchantment) ||
                                  (tool is Pickaxe && Config.Pick.AllowReachingEnchantment)))
        {
            __result = tool.UpgradeLevel == 4;
        }
    }
}
