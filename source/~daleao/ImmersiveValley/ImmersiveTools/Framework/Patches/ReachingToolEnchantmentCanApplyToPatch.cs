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
internal sealed class ReachingToolEnchantmentCanApplyToPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ReachingToolEnchantmentCanApplyToPatch()
    {
        Target = RequireMethod<ReachingToolEnchantment>(nameof(ReachingToolEnchantment.CanApplyTo));
    }

    #region harmony patches

    /// <summary>Allow apply Reaching enchant to Axe and Pickaxe.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static bool ReachingToolEnchantmentCanApplyToPrefix(ref bool __result, Item item)
    {
        if (item is Tool tool && (tool is WateringCan or Hoe ||
                                  tool is Axe && ModEntry.Config.AxeConfig.AllowReachingEnchantment ||
                                  tool is Pickaxe && ModEntry.Config.PickaxeConfig.AllowReachingEnchantment))
            __result = tool.UpgradeLevel == 4;
        else
            __result = false;

        return false; // don't run original logic
    }

    #endregion harmony patches
}