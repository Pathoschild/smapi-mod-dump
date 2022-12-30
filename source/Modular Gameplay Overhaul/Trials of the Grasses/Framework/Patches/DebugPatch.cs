/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

#if DEBUG
using DaLion.Common;
using DaLion.Common.Attributes;
using HarmonyLib;
using JetBrains.Annotations;

namespace DaLion.Stardew.Alchemy.Framework.Patches;

/// <summary>Wildcard prefix patch for on-demand debugging.</summary>
[UsedImplicitly, DebugOnly]
internal class DebugPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal DebugPatch()
    {
        //Target = RequireMethod<>(nameof(.));
    }

    #region harmony patches

    [HarmonyPrefix]
    private static bool DebugPrefix(object __instance)
    {
        Log.D("DebugPatch called!");



        return false; // don't run original logic
    }

    #endregion harmony patches
}
#endif