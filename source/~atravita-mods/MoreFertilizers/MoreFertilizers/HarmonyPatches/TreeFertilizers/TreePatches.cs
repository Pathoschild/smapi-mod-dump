/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Utils.Extensions;
using HarmonyLib;

using Microsoft.Xna.Framework;

using MoreFertilizers.Framework;
using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.TreeFertilizers;

/// <summary>
/// Patches against Tree.
/// </summary>
[HarmonyPatch(typeof(Tree))]
internal static class TreePatches
{
    [HarmonyPatch(nameof(Tree.fertilize))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static void Postfix(Tree __instance, bool __result)
    {
        if (__result)
        {
            __instance.modData?.SetBool(CanPlaceHandler.TreeFertilizer, true);
        }
    }

    [HarmonyPostfix]
    [HarmonyPriority(Priority.LowerThanNormal)]
    [HarmonyPatch(nameof(Tree.UpdateTapperProduct))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static void PostfixUpdate(Tree __instance, SObject tapper_instance)
    {
        if (__instance.modData?.GetBool(CanPlaceHandler.TreeTapperFertilizer) == true
            && tapper_instance.heldObject?.Value is not null && Game1.random.Next(8) == 0)
        {
            ModEntry.ModMonitor.DebugOnlyLog($"Boosting tapper yield of tree at {__instance.currentTileLocation}.");
            tapper_instance.heldObject.Value.Stack++;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Tree.draw))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention")]
    private static void PostfixDraw(Tree __instance)
    {
        if (__instance.modData.ContainsKey(CanPlaceHandler.TreeTapperFertilizer) && Game1.random.Next(256) == 0)
        {
            __instance.currentLocation.temporarySprites.Add(
                new TemporaryAnimatedSprite(
                    rowInAnimationTexture: 4,
                    position: (__instance.currentTileLocation * Game1.tileSize) + new Vector2(Game1.random.Next(-32, 32), Game1.random.Next(-128, -14)),
                    color: Color.Yellow,
                    animationLength: 8,
                    flipped: Game1.random.Next(2) == 0,
                    animationInterval: 150,
                    layerDepth: ((__instance.currentTileLocation.Y * Game1.tileSize) + Game1.random.Next(100)) / 10000f
                )
                {
                    scaleChange = 0.01f,
                });
        }
    }
}