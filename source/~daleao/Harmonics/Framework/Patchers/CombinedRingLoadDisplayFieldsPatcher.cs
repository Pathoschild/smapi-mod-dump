/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers.Rings;

using DaLion.Overhaul.Modules.Combat.Integrations;

#region using directives

using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingLoadDisplayFieldsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingLoadDisplayFieldsPatcher"/> class.</summary>
    internal CombinedRingLoadDisplayFieldsPatcher()
    {
        this.Target = this.RequireMethod<CombinedRing>("loadDisplayFields");
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Iridium description is always first, and gemstone descriptions are grouped together.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool CombinedRingsLoadDisplayFieldsPrefix(CombinedRing __instance, ref bool __result)
    {
        if (!JsonAssetsIntegration.InfinityBandIndex.HasValue ||
            __instance.ParentSheetIndex != JsonAssetsIntegration.InfinityBandIndex.Value)
        {
            return true; // don't run original logic
        }

        if (Game1.objectInformation is null || __instance.indexInTileSheet is null)
        {
            __result = false;
            return false; // don't run original logic
        }

        var data = Game1.objectInformation[__instance.indexInTileSheet.Value].SplitWithoutAllocation('/');
        __instance.displayName = data[4].ToString();
        __instance.description = data[5].ToString();
        __result = true;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
