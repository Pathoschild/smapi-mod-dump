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
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodEnchantmentCanApplyToPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FishingRodEnchantmentCanApplyToPatch()
    {
        Target = RequireMethod<FishingRodEnchantment>(nameof(FishingRodEnchantment.CanApplyTo));
    }

    #region harmony patches

    /// <summary>Allow apply Master enchantment to other tools.</summary>
    [HarmonyPostfix]
    private static void FishingRodEnchantmentCanApplyTo(FishingRodEnchantment __instance, ref bool __result, Item item)
    {
        if (__instance is MasterEnchantment && item is Axe or Hoe or Pickaxe or WateringCan) __result = true;
    }

    #endregion harmony patches
}