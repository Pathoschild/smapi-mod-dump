using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SilentOak.Patching;
using StardewValley;
using SObject = StardewValley.Object;

/***
 * Inspired by https://github.com/danvolchek/StardewMods/blob/32046f848ea1a1aade495b9adad612f8b94928d1/BetterArtisanGoodIcons/Patches/SObjectPatches/DrawInMenuPatch.cs
 ***/
namespace SilentOak.QualityProducts.Patches.BetterMeadIcons
{
    /// <summary>
    /// Patch for custom mead textures.
    /// </summary>
    public static class SObjectDrawInMenuPatch
    {
        public static readonly PatchData PatchData = new PatchData(
            type: typeof(SObject),
            originalMethodName: "drawInMenu",
            originalMethodParams: new Type[]
            {
                typeof(SpriteBatch),  // spriteBatch     (sprite batch to draw onto)
                typeof(Vector2),      // location        (where to draw on the sprite batch)
                typeof(float),        // scaleSize       (drawing scale)   
                typeof(float),        // transparency    
                typeof(float),        // layerDepth      
                typeof(bool),         // drawStackNumber (if true, draw stack number)
                typeof(Color),        // color           
                typeof(bool)          // drawShadow      (if true, draw shadow)
            }
        );

        /// <summary>
        /// Patch for drawing a custom sprite (if available) for the object in the menu.
        /// </summary>
        /// <returns>If original method should be executed.</returns>
        /// <param name="__instance">The object to be drawn.</param>
        /// <param name="spriteBatch">Sprite batch to draw onto.</param>
        /// <param name="location">Where to draw on the sprite batch.</param>
        /// <param name="scaleSize">Drawing scale.</param>
        /// <param name="transparency">Transparency.</param>
        /// <param name="layerDepth">Layer depth.</param>
        /// <param name="drawStackNumber">If set to <c>true</c> draw stack number.</param>
        /// <param name="color">Color.</param>
        /// <param name="drawShadow">If set to <c>true</c> draw shadow.</param>
        public static bool Prefix(
            SObject __instance,
            SpriteBatch spriteBatch,
            Vector2 location,
            float scaleSize,
            float transparency,
            float layerDepth,
            bool drawStackNumber,
            Color color,
            bool drawShadow
        )
        {
            if (!SpriteLoader.TryLoadSprite(__instance, out Texture2D texture, out Rectangle sourceRect))
            {
                return true;
            }

            if (drawShadow)
            {
                Texture2D shadowTexture = Game1.shadowTexture;
                spriteBatch.Draw(
                    texture: shadowTexture,
                    position: location + new Vector2(32f, 48f),
                    sourceRectangle: shadowTexture.Bounds,
                    color: color * 0.5f,
                    rotation: 0f,
                    origin: new Vector2(shadowTexture.Bounds.Center.X, shadowTexture.Bounds.Center.Y),
                    scale: 3f,
                    effects: SpriteEffects.None,
                    layerDepth: layerDepth - 0.0001f
                );
            }

            spriteBatch.Draw(
                texture: texture,
                position: location + new Vector2(
                    (int)(32f * scaleSize),
                    (int)(32f * scaleSize)
                ),
                sourceRectangle: sourceRect,
                color: color * transparency,
                rotation: 0f,
                origin: new Vector2(8f, 8f) * scaleSize,
                scale: 4f * scaleSize,
                effects: SpriteEffects.None,
                layerDepth: layerDepth
            );

            if (drawStackNumber && __instance.maximumStackSize() > 1 && scaleSize > 0.3 && __instance.Stack != 2147483647 && __instance.Stack > 1)
            {
                StardewValley.Utility.drawTinyDigits(
                    toDraw: __instance.Stack,
                    spriteBatch,
                    position: location + new Vector2(
                        64 - StardewValley.Utility.getWidthOfTinyDigitString(__instance.Stack, 3f * scaleSize) + 3f * scaleSize,
                        64f - 18f * scaleSize + 2f
                    ),
                    scale: 3f * scaleSize,
                    layerDepth: 1f,
                    color
                );
            }

            if (drawStackNumber && __instance.Quality > 0)
            {
                float num = (__instance.Quality < 4) ? 0f : (((float)Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1f) * 0.05f);
                spriteBatch.Draw(
                    texture: Game1.mouseCursors,
                    position: location + new Vector2(12f, 52f + num),
                    sourceRectangle: (__instance.Quality < 4)
                        ? new Rectangle(338 + (__instance.Quality - 1) * 8, 400, 8, 8)
                        : new Rectangle(346, 392, 8, 8),
                    color: color * transparency,
                    rotation: 0f,
                    origin: new Vector2(4f, 4f),
                    scale: 3f * scaleSize * (1f + num),
                    effects: SpriteEffects.None,
                    layerDepth: layerDepth
                );
            }

            return false;
        }
    }
}
