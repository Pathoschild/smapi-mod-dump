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

using StardewValley.TerrainFeatures;

namespace MoreFertilizers.HarmonyPatches.BeverageDrawPatches;

/// <summary>
/// A patch on hoedirt to draw in the particle effects for the beverage fertilizer.
/// </summary>
[HarmonyPatch(typeof(HoeDirt))]
internal class HoeDirtDrawPatch
{
    [HarmonyPatch(nameof(HoeDirt.draw))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    private static void Postfix(HoeDirt __instance)
    {
        if (!ModEntry.Config.DrawParticleEffects)
        {
            return;
        }

        if (__instance.fertilizer.Value != -1 && __instance.fertilizer.Value == ModEntry.MiraculousBeveragesID && Game1.random.Next(512) == 0)
        {
            __instance.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(
                Game1.mouseCursorsName,
                new Rectangle(372, 1956, 10, 10),
                new Vector2(
                    (__instance.currentTileLocation.X * 64f) + Game1.random.Next(32),
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
