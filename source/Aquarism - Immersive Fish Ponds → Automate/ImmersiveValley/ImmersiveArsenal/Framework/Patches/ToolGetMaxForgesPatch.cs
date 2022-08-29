/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolGetMaxForgesPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ToolGetMaxForgesPatch()
    {
        Target = RequireMethod<Tool>(nameof(Tool.GetMaxForges));
    }

    #region harmony patches

    /// <summary>Allow Slingshot forges.</summary>
    [HarmonyPrefix]
    private static bool ToolGetMaxForgesPrefix(Tool __instance, ref int __result)
    {
        if (__instance is not Slingshot) return true; // run original logic

        __result = 3;
        return false; // don't run original logic
    }

    #endregion harmony patches
}