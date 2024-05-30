/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chiccenFL/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WaterYourCrops
{
    public partial class ModEntry
    {
        // HoeDirt.dry = 0;
        // HoeDirt.watered = 1;
        // both apply to HoeDirt.state
        // so if HoeDirt.state == 1, then that dirt is watered

        [HarmonyPatch(typeof(HoeDirt), nameof(HoeDirt.DrawOptimized))]
        public class HoeDirt_DrawOptimized_Patch
        {
            public static void Postfix(HoeDirt __instance, SpriteBatch dirt_batch)
            {
                if (!Config.EnableMod || __instance.state.Value != 0 || __instance.crop is null || (!HasCan() && Config.OnlyWaterCan))
                    return;

                Vector2 tile = __instance.Tile;
                Vector2 drawPos = Game1.GlobalToLocal(Game1.viewport, tile * 64f);
                byte drawSum = 0;
                Vector2 tileLocation = tile;
                tileLocation.X += 1f;
                GameLocation location = Game1.player.currentLocation;

                if (location.terrainFeatures.TryGetValue(tileLocation, out var rightFeature) && rightFeature is HoeDirt)
                    drawSum += 2;
                tileLocation.X -= 2f;
                if (location.terrainFeatures.TryGetValue(tileLocation, out var leftFeature) && leftFeature is HoeDirt)
                    drawSum += 8;
                tileLocation.X += 1f;
                tileLocation.Y += 1f;
                if (location.terrainFeatures.TryGetValue(tileLocation, out var downFeature) && downFeature is HoeDirt)
                    drawSum += 4;
                tileLocation.Y -= 2f;
                if (location.terrainFeatures.TryGetValue(tileLocation, out var upFeature) && upFeature is HoeDirt)
                    drawSum++;

                int sourceRectPosition = HoeDirt.drawGuide[drawSum];

                dirt_batch?.Draw(
                    waterTexture, 
                    drawPos, 
                    //new Rectangle(sourceRectPosition % 4 * 16, sourceRectPosition / 4 * 16, 16, 16),
                    null,
                    Config.IndicatorColor * Config.IndicatorOpacity,
                    0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    (tile.Y * 64f + 32f + (tile.Y * 11f + tile.X * 7f) % 10f - 5f) / 100f
                );
            }
        }

    }
}
