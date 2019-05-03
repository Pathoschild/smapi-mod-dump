using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SilentOak.Patching;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts.Patches.BetterMeadIcons
{
    /// <summary>
    /// Patch for custom mead textures.
    /// </summary>
    public static class SObjectDraw2Patch
    {
        public static readonly PatchData PatchData = new PatchData(
            type: typeof(SObject),
            originalMethodName: "draw",
            originalMethodParams: new Type[]
            {
                typeof(SpriteBatch),   // spriteBatch (sprite batch to draw onto)
                typeof(int),           // x           (x non-tile coordinate) 
                typeof(int),           // y           (y non-tile coordinate)
                typeof(float),         // layerDepth
                typeof(float)          // alpha       (transparency)
            }
        );

        /// <summary>
        /// Patch for drawing a custom sprite (if available) for the object.
        /// </summary>
        /// <returns>If original method should be executed.</returns>
        /// <param name="__instance">Object to be drawn.</param>
        /// <param name="spriteBatch">Sprite batch to draw onto.</param>
        /// <param name="xNonTile">X non-tile coordinate.</param>
        /// <param name="yNonTile">Y non-tile coordinate.</param>
        /// <param name="layerDepth">Layer depth.</param>
        /// <param name="alpha">Transparency.</param>
        public static bool Prefix(SObject __instance, SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1f)
        {
            if (!SpriteLoader.TryLoadSprite(__instance, out Texture2D texture, out Rectangle sourceRect))
            {
                return true;
            }

            if (Game1.eventUp && Game1.CurrentEvent.isTileWalkedOn(xNonTile / 64, yNonTile / 64))
            {
                return false;
            }

            if (__instance.fragility != 2)
            {
                Texture2D shadowTexture = Game1.shadowTexture;
                spriteBatch.Draw(
                    texture: shadowTexture,
                    position: Game1.GlobalToLocal(
                        viewport: Game1.viewport,
                        globalPosition: new Vector2(xNonTile + 32, yNonTile + 51 + 4)
                    ),
                    sourceRectangle: shadowTexture.Bounds,
                    color: Color.White * alpha,
                    rotation: 0f,
                    origin: new Vector2(shadowTexture.Bounds.Center.X, shadowTexture.Bounds.Center.Y),
                    scale: 4f,
                    effects: SpriteEffects.None,
                    layerDepth: layerDepth - 1E-06f
                );
            }

            spriteBatch.Draw(
                texture: texture,
                position: Game1.GlobalToLocal(
                    viewport: Game1.viewport,
                    globalPosition: new Vector2(
                        xNonTile + 32 + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0),
                        yNonTile + 32 + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0)
                    )
                ),
                sourceRectangle: sourceRect,
                color: Color.White * alpha, 
                rotation: 0f,
                origin: new Vector2(8f, 8f),
                scale: (__instance.Scale.Y > 1f) ? __instance.getScale().Y : 4f,
                effects: __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                layerDepth: layerDepth
            );

            return false;
        }
    }
}
