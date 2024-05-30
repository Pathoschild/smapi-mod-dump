/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotCanAutoFirePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotCanAutoFirePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal SlingshotCanAutoFirePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.CanAutoFire));
    }

    #region harmony patches

    /// <summary>Patch to add Desperado auto-fire during LimitBreak.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyBefore("DaLion.Combat")]
    private static bool SlingshotCanAutoFirePrefix(Slingshot __instance, ref bool __result)
    {
        if (!__instance.lastUser.IsLocalPlayer)
        {
            return true; // run original logic
        }

        __result = State.LimitBreak is DesperadoBlossom { IsActive: true };
        return !__result;
    }

    #endregion harmony patches
}
