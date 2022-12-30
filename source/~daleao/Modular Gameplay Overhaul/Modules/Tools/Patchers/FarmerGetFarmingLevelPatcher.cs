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
internal sealed class FarmerGetFarmingLevelPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerGetFarmingLevelPatcher"/> class.</summary>
    internal FarmerGetFarmingLevelPatcher()
    {
        this.Target = this.RequireMethod<Farmer>("get_FarmingLevel");
    }

    #region harmony patches

    /// <summary>Master Hoe and Watering Can enchantment effect.</summary>
    [HarmonyPostfix]
    private static void FarmerGetFarmingLevelPostfix(Farmer __instance, ref int __result)
    {
        if (__instance.CurrentTool is { } tool and (Hoe or WateringCan) &&
            tool.hasEnchantmentOfType<MasterEnchantment>())
        {
            __result++;
        }
    }

    #endregion harmony patches
}
