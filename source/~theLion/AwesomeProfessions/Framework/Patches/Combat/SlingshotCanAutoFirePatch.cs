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

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Tools;

using SuperMode;

#endregion using directives

[UsedImplicitly]
internal class SlingshotCanAutoFirePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotCanAutoFirePatch()
    {
        Original = RequireMethod<Slingshot>(nameof(Slingshot.CanAutoFire));
    }

    #region harmony patches

    /// <summary>Patch to allow auto-fire during Desperado Super Mode.</summary>
    [HarmonyPrefix]
    private static bool SlingshotCanAutoFirePrefix(Slingshot __instance, ref bool __result)
    {
        try
        {
            var who = __instance.getLastFarmerToUse();
            if (who.IsLocalPlayer && ModEntry.PlayerState.Value.SuperMode is DesperadoTemerity {IsActive: true})
                __result = true;
            else
                __result = false;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}