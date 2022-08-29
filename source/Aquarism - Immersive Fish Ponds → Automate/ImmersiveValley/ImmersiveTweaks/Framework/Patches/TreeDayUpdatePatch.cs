/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Common.Extensions.Stardew;
using Extensions;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class TreeDayUpdatePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal TreeDayUpdatePatch()
    {
        Target = RequireMethod<Tree>(nameof(Tree.dayUpdate));
    }

    #region harmony patches

    /// <summary>Ages tapper trees.</summary>
    [HarmonyPostfix]
    private static void TreeDayUpdatePostfix(Tree __instance)
    {
        if (__instance.growthStage.Value >= Tree.treeStage && __instance.CanBeTapped() &&
            ModEntry.Config.AgeImprovesTreeSap) __instance.Increment("Age");
    }

    #endregion harmony patches
}