/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Runtime.CompilerServices;

using AtraBase.Toolkit;

using GrowableGiantCrops.Framework;

using HarmonyLib;

namespace GrowableGiantCrops.HarmonyPatches.ToolPatches;

/// <summary>
/// Patches for enchantments for the shovel.
/// </summary>
[HarmonyPatch]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Named for Harmony.")]
internal static class EnchantmentPatches
{
    [HarmonyPostfix]
    [MethodImpl(TKConstants.Hot)]
    [HarmonyPatch(typeof(HoeEnchantment), nameof(HoeEnchantment.CanApplyTo))]
    private static void OverrideHoeCanApplyTo(Item item, ref bool __result)
    {
        if (!__result && ModEntry.Config.AllowHoeEnchantments && item is ShovelTool)
        {
            __result = true;
        }
    }
}
