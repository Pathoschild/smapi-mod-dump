/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotGetRequiredChargeTimePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotGetRequiredChargeTimePatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.GetRequiredChargeTime));
        Postfix!.after = new[] {"DaLion.ImmersiveArsenal", "DaLion.ImmersiveRings"};
    }

    #region harmony patches

    /// <summary>Patch to reduce Slingshot charge time for Desperado.</summary>
    [HarmonyPostfix]
    [HarmonyBefore("DaLion.ImmersiveArsenal", "DaLion.ImmersiveRings")]
    private static void SlingshotGetRequiredChargeTimePostfix(Slingshot __instance, ref float __result)
    {
        var firer = __instance.getLastFarmerToUse();
        if (!firer.IsLocalPlayer || !firer.HasProfession(Profession.Desperado)) return;

        __result *= 1f - MathHelper.Lerp(0f, 0.5f, (float) firer.health / firer.maxHealth);
    }

    #endregion harmony patches
}