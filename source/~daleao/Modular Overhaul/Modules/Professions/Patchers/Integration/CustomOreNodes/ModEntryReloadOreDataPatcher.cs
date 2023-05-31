/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration;

#region using directives

using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[RequiresMod("aedenthorn.CustomOreNodes")]
internal sealed class ModEntryReloadOreDataPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ModEntryReloadOreDataPatcher"/> class.</summary>
    internal ModEntryReloadOreDataPatcher()
    {
        this.Target = "CustomOreNodes.ModEntry"
            .ToType()
            .RequireMethod("ReloadOreData");
    }

    #region harmony patches

    /// <summary>Register custom ores.</summary>
    [HarmonyPostfix]
    private static void ModEntryReloadOreDataPostfix()
    {
        CustomOreNodesIntegration.Instance!.RegisterCustomOreData();
    }

    #endregion harmony patches
}
