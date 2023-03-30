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
[RequiresMod("aedenthorn.CustomResourceClumps")]
internal sealed class ModEntryGameLoopGameLaunchedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ModEntryGameLoopGameLaunchedPatcher"/> class.</summary>
    internal ModEntryGameLoopGameLaunchedPatcher()
    {
        this.Target = "CustomResourceClumps.ModEntry"
            .ToType()
            .RequireMethod("GameLoop_GameLaunched");
    }

    #region harmony patches

    /// <summary>Register custom clumps.</summary>
    [HarmonyPostfix]
    private static void ModEntryGameLoopGameLaunchedPostfix()
    {
        CustomResourceClumpsIntegration.Instance!.RegisterCustomClumpData();
    }

    #endregion harmony patches
}
