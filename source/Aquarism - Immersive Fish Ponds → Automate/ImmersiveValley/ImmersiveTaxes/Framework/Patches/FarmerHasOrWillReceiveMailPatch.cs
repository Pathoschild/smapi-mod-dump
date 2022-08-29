/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Taxes.Framework.Patches;

#region using directives

using Common;
using HarmonyLib;
using System;
using System.Reflection;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerHasOrWillReceiveMailPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmerHasOrWillReceiveMailPatch()
    {
        Target = RequireMethod<Farmer>(nameof(Farmer.hasOrWillReceiveMail));
    }

    #region harmony patches

    /// <summary>Patch to allow receiving multiple letters from the FRS.</summary>
    [HarmonyPrefix]
    private static bool FarmerHasOrWillReceiveMailPrefix(ref bool __result, string id)
    {
        try
        {
            if (!id.Contains(ModEntry.Manifest.UniqueID))
                return true; // run original logic

            __result = id.Contains("TaxIntro");
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