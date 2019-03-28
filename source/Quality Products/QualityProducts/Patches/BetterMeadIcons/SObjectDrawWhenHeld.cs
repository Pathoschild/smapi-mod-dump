using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SilentOak.Patching;
using StardewValley;
using SObject = StardewValley.Object;

/***
 * Inspired by https://github.com/danvolchek/StardewMods/blob/32046f848ea1a1aade495b9adad612f8b94928d1/BetterArtisanGoodIcons/Patches/SObjectPatches/DrawWhenHeldPatch.cs
 ***/
namespace SilentOak.QualityProducts.Patches.BetterMeadIcons
{
    /// <summary>
    /// Patch for custom mead textures.
    /// </summary>
    public static class SObjectDrawWhenHeld
    {
        public static readonly PatchData PatchData = new PatchData(
            type: typeof(SObject),
            originalMethodName: "drawWhenHeld",
            originalMethodParams: new Type[]
            {
                typeof(SpriteBatch),   // spriteBatch    (sprite batch to draw onto)
                typeof(Vector2),       // objectPosition (where to draw on the sprite batch)
                typeof(Farmer)         // f              (farmer that is holding the object)
            }
        );

        /// <summary>
        /// Patch for drawing a custom sprite (if available) for the given farmer's active object.
        /// </summary> 
        /// <returns>If original method should be executed.</returns>
        /// <param name="spriteBatch">Sprite batch to draw onto.</param>
        /// <param name="objectPosition">Where to draw on the sprite batch.</param>
        /// <param name="f">Farmer that is holding the object.</param>
        public static bool Prefix(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            if (!SpriteLoader.TryLoadSprite(f.ActiveObject, out Texture2D texture, out Rectangle sourceRect))
            {
                return true;
            }

            spriteBatch.Draw(
                texture: texture,
                position: objectPosition,
                sourceRectangle: sourceRect,
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: Math.Max(0f, (f.getStandingY() + 2) / 10000f)
            );

            if (f.ActiveObject != null && f.ActiveObject.Name.Contains("="))
            {
                spriteBatch.Draw(
                    texture: texture,
                    position: objectPosition + new Vector2(32f, 32f),
                    sourceRectangle: sourceRect,
                    color: Color.White,
                    rotation: 0f,
                    origin: new Vector2(32f, 32f),
                    scale: 4f + Math.Abs(Game1.starCropShimmerPause) / 8f,
                    effects: SpriteEffects.None,
                    layerDepth: Math.Max(0f, (f.getStandingY() + 2) / 10000f)
                );
                if (Math.Abs(Game1.starCropShimmerPause) <= 0.05f && Game1.random.NextDouble() < 0.97)
                {
                    return false;
                }
                Game1.starCropShimmerPause += 0.04f;
                if (Game1.starCropShimmerPause >= 0.8f)
                {
                    Game1.starCropShimmerPause = -0.8f;
                }
            }

            return false;
        }
    }
}
