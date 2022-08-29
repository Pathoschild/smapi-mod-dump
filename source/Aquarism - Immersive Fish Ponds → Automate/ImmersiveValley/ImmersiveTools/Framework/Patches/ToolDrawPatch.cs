/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolDrawPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ToolDrawPatch()
    {
        Target = RequireMethod<Tool>("draw");
    }

    #region harmony patches

    /// <summary>Hide affected tiles overlay.</summary>
    [HarmonyPrefix]
    private static bool ToolDrawPrefix() => !ModEntry.Config.HideAffectedTiles;

    #endregion harmony patches
}