/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Slingshots.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolForgePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolForgePatcher"/> class.</summary>
    internal ToolForgePatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.Forge));
    }

    #region harmony patches

    /// <summary>Invalidate stats on forge.</summary>
    [HarmonyPostfix]
    private static void ToolForgePostfix(Tool __instance, bool __result)
    {
        if (__instance is Slingshot slingshot && __result)
        {
            slingshot.Invalidate();
        }
    }

    #endregion harmony patches
}
