/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotGetRequiredChargeTimePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotGetRequiredChargeTimePatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.GetRequiredChargeTime));
        Postfix!.after = new[] { "DaLion.ImmersiveArsenal" };
        Postfix!.before = new[] { "DaLion.ImmersiveProfessions" };
    }

    #region harmony patches

    /// <summary>Apply Emerald Enchantment to Slingshot.</summary>
    [HarmonyPostfix]
    [HarmonyAfter("DaLion.ImmersiveArsenal")]
    [HarmonyBefore("DaLion.ImmersiveProfessions")]
    private static void SlingshotGetRequiredChargeTimePostfix(Slingshot __instance, ref float __result)
    {
        var firer = __instance.getLastFarmerToUse();
        if (!firer.IsLocalPlayer) return;
        
        __result *= 1f - firer.weaponSpeedModifier;
    }

    #endregion harmony patches
}