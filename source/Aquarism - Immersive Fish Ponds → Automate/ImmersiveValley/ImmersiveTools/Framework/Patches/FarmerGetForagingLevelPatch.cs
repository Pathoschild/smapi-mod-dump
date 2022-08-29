/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGetForagingLevelPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmerGetForagingLevelPatch()
    {
        Target = RequireMethod<Farmer>("get_ForagingLevel");
    }

    #region harmony patches

    /// <summary>Master Axe enchantment effect.</summary>
    [HarmonyPostfix]
    private static void FarmerGetForagingLevelPostfix(Farmer __instance, ref int __result)
    {
        if (__instance.CurrentTool is Axe axe && axe.hasEnchantmentOfType<MasterEnchantment>())
            ++__result;
    }

    #endregion harmony patches
}