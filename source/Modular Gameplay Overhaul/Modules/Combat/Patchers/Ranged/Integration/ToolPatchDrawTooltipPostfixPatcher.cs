/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged.Integration;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModRequirement("PeacefulEnd.Archery", "Archery", "2.1.0")]
internal sealed class ToolPatchDrawTooltipPostfixPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolPatchDrawTooltipPostfixPatcher"/> class.</summary>
    internal ToolPatchDrawTooltipPostfixPatcher()
    {
        this.Target = "Archery.Framework.Patches.Objects.ToolPatch"
            .ToType()
            .RequireMethod("DrawTooltipPostfix");
    }

    #region harmony patches

    /// <summary>Override tooltip damage.</summary>
    [HarmonyPrefix]
    private static bool ToolPatchDrawTooltipPostfixPrefix()
    {
        return false; // don't run original logic
    }

    #endregion harmony patches
}
