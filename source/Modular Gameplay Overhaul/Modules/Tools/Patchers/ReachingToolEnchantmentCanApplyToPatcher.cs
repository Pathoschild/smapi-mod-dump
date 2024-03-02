/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ReachingToolEnchantmentCanApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ReachingToolEnchantmentCanApplyToPatcher"/> class.</summary>
    internal ReachingToolEnchantmentCanApplyToPatcher()
    {
        this.Target = this.RequireMethod<ReachingToolEnchantment>(nameof(ReachingToolEnchantment.CanApplyTo));
    }

    #region harmony patches

    /// <summary>Allow apply Reaching enchant to Axe and Pick.</summary>
    [HarmonyPrefix]
    private static bool ReachingToolEnchantmentCanApplyToPrefix(ref bool __result, Item item)
    {
        if (item is Tool tool && (tool is WateringCan or Hoe ||
                                  (tool is Axe && ToolsModule.Config.Axe.AllowReachingEnchantment) ||
                                  (tool is Pickaxe && ToolsModule.Config.Pick.AllowReachingEnchantment)))
        {
            __result = tool.UpgradeLevel >= 4;
        }
        else
        {
            __result = false;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
