/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Runtime.CompilerServices;

using AtraBase.Toolkit;

using HarmonyLib;

using Microsoft.Xna.Framework;

using MoreFertilizers.Framework;

using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.BeverageDrawPatches;

/// <summary>
/// Patches fruit tree's draw method to add a TAS.
/// </summary>
[HarmonyPatch(typeof(FruitTree))]
internal static class FruitTreeUpdatePatches
{
    [MethodImpl(TKConstants.Hot)]
    [HarmonyPatch(nameof(FruitTree.draw))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static void Postfix(FruitTree __instance)
    {
        if (!ModEntry.Config.DrawParticleEffects)
        {
            return;
        }

        if (__instance.modData.ContainsKey(CanPlaceHandler.MiraculousBeverages) && Game1.random.Next(512) == 0)
        {
            __instance.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(
                Game1.mouseCursorsName,
                new Rectangle(372, 1956, 10, 10),
                new Vector2(
                    (__instance.currentTileLocation.X * 64f) + Game1.random.Next(-64, 96),
                    (__instance.currentTileLocation.Y * 64f) + Game1.random.Next(-256, -128)),
                flipped: false,
                0.002f,
                Color.LimeGreen)
            {
                alpha = 0.75f,
                motion = new Vector2(0f, -0.5f),
                interval = 99999f,
                layerDepth = 1f,
                scale = 2f,
                scaleChange = 0.01f,
            });
        }
        else if (__instance.growthStage.Value == FruitTree.treeStage && __instance.modData.ContainsKey(CanPlaceHandler.EverlastingFruitTreeFertilizer)
            && Game1.random.Next(512) == 0)
        {
            Utility.addSprinklesToLocation(
              l: __instance.currentLocation,
              sourceXTile: (int)__instance.currentTileLocation.X,
              sourceYTile: (int)__instance.currentTileLocation.Y - 2,
              tilesWide: 3,
              tilesHigh: 5,
              totalSprinkleDuration: 400,
              millisecondsBetweenSprinkles: 10,
              sprinkleColor: Color.LightGoldenrodYellow);
        }
    }
}
