/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using HarmonyLib;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MoreFertilizers.Framework;

using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.BeverageDrawPatches;

/// <summary>
/// Holds a patch to draw the Beverages effect for bushes.
/// </summary>
[HarmonyPatch(typeof(Bush))]
internal static class BushDrawPatch
{
    [HarmonyPatch(nameof(Bush.draw), new[] { typeof(SpriteBatch), typeof(Vector2) } )]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static void Postfix(Bush __instance)
    {
        if (__instance.modData.ContainsKey(CanPlaceHandler.MiraculousBeverages) && Game1.random.Next(512) == 0)
        {
            __instance.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(
                Game1.mouseCursorsName,
                new Rectangle(372, 1956, 10, 10),
                new Vector2(
                    (__instance.currentTileLocation.X * 64f) + Game1.random.Next(64),
                    (__instance.currentTileLocation.Y * 64f) + Game1.random.Next(-128, 0)),
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
    }
}
