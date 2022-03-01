/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Tools;

using SuperMode;

#endregion using directives

[UsedImplicitly]
internal class SlingshotGetRequiredChargeTimePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotGetRequiredChargeTimePatch()
    {
        Original = RequireMethod<Slingshot>(nameof(Slingshot.GetRequiredChargeTime));
    }

    #region harmony patches

    /// <summary>Patch to reduce slingshot charge time for Desperado.</summary>
    [HarmonyPostfix]
    private static void SlingshotGetRequiredChargeTimePostfix(Slingshot __instance, ref float __result)
    {
        var firer = __instance.getLastFarmerToUse();
        if (!firer.IsLocalPlayer || ModEntry.PlayerState.Value.SuperMode is not DesperadoTemerity) return;
        
        __result *= 1f - ModEntry.PlayerState.Value.SuperMode.PercentCharge;
    }

    #endregion harmony patches
}