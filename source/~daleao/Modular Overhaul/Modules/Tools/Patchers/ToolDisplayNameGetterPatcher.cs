/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModConflict("spacechase0.MoonMisadventures")]
internal sealed class ToolDisplayNameGetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolDisplayNameGetterPatcher"/> class.</summary>
    internal ToolDisplayNameGetterPatcher()
    {
        this.Target = this.RequirePropertyGetter<Tool>(nameof(Tool.DisplayName));
    }

    #region harmony patches

    /// <summary>Don't change tile index. We will simply patch over it.</summary>
    [HarmonyPrefix]
    private static bool ToolSetNewTileIndexForUpgradeLevelPrefix(Tool __instance, ref string __result)
    {
        if (__instance.UpgradeLevel < 5)
        {
            return true;
        }

        __result = __instance.Name;
        return false;
    }

    #endregion harmony patches
}
