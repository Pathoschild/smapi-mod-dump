/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolDrawPatcher"/> class.</summary>
    internal ToolDrawPatcher()
    {
        this.Target = this.RequireMethod<Tool>("draw");
    }

    #region harmony patches

    /// <summary>Hide affected tiles overlay.</summary>
    [HarmonyPrefix]
    private static bool ToolDrawPrefix()
    {
        return !ToolsModule.Config.HideAffectedTiles;
    }

    #endregion harmony patches
}
