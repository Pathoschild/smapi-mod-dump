/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

//namespace DaLion.Stardew.Alchemy.Framework.Patches;

//#region using directives

//using HarmonyLib;
//using JetBrains.Annotations;

//using DaLion.Common;
//using DaLion.Common.Harmony;

//#endregion using directives

///// <summary>Wildcard prefix patch for on-demand debugging.</summary>
//[UsedImplicitly]
//internal class DebugPatch : BasePatch
//{
//    /// <summary>Construct an instance.</summary>
//    internal DebugPatch()
//    {
//#if DEBUG
//        Target = RequireMethod<>(nameof(.));
//#endif
//    }

//    #region harmony patches

//    [HarmonyPrefix]
//    private static bool DebugPrefix(object __instance)
//    {
//        Log.D("DebugPatch called!");

//        return true; // run original logic
//    }

//    #endregion harmony patches
//}