/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/silentoak/StardewMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SilentOak.Patching;
using StardewValley;
using SObject = StardewValley.Object;

/***
 * Inspired by https://github.com/danvolchek/StardewMods/blob/32046f848ea1a1aade495b9adad612f8b94928d1/BetterArtisanGoodIcons/Patches/SObjectPatches/DrawPatch.cs
 ***/
namespace SilentOak.QualityProducts.Patches.BetterMeadIcons
{
    /// <summary>
    /// Patch for custom mead textures.
    /// </summary>
    public static class SObjectDrawPatch
    {
        public static readonly PatchData PatchData = new PatchData(
            type: typeof(SObject),
            originalMethodName: "draw",
            originalMethodParams: new Type[]
            {
                typeof(SpriteBatch),   // spriteBatch (sprite batch to draw onto)
                typeof(int),           // x           (x tile coordinate) 
                typeof(int),           // y           (y tile coordinate)
                typeof(float),         // alpha       (transparency)
            }
        );

        /// <summary>
        /// Patch for drawing a custom sprite (if available) for the object held by the instance.
        /// </summary>
        /// <param name="__instance">Object to be drawn.</param>
        /// <param name="spriteBatch">Sprite batch to draw onto.</param>
        /// <param name="x">The x tile coordinate.</param>
        /// <param name="y">The y tile coordinate.</param>
        /// <param name="alpha">Transparency.</param>
        public static bool Prefix(SObject __instance, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (
                !(bool)__instance.bigCraftable
                || !(bool)__instance.readyForHarvest
                || __instance.heldObject == null
                || !SpriteLoader.TryLoadSprite(__instance.heldObject.Value, out Texture2D texture, out Rectangle sourceRect)
                )
            {
                return true;
            }

            Vector2 value = 4 * __instance.getScale();

            Vector2 vector = Game1.GlobalToLocal(
                viewport: Game1.viewport,
                globalPosition: new Vector2(x * 64, y * 64 - 64)
            );

            Rectangle destinationRectangle = new Rectangle(
                (int)(vector.X - value.X / 2f) + ((__instance.shakeTimer > 0)
                    ? Game1.random.Next(-1, 2) : 0),
                (int)(vector.Y - value.Y / 2f) + ((__instance.shakeTimer > 0)
                    ? Game1.random.Next(-1, 2) : 0),
                (int)(64f + value.X),
                (int)(128f + value.Y / 2f)
            );

            spriteBatch.Draw(
                texture: Game1.bigCraftableSpriteSheet,
                destinationRectangle: destinationRectangle,
                sourceRectangle: SObject.getSourceRectForBigCraftable(((bool)__instance.showNextIndex)
                    ? (__instance.ParentSheetIndex + 1) : __instance.ParentSheetIndex),
                color: Color.White * alpha,
                rotation: 0f,
                origin: Vector2.Zero,
                effects: SpriteEffects.None,
                layerDepth: Math.Max(0f, ((y + 1) * 64 - 24) / 10000f) + (((int)__instance.parentSheetIndex == 105) ? 0.0035f : 0f) + x * 1E-05f
                );

            if (__instance.Name.Equals("Loom") && (int)__instance.minutesUntilReady > 0)
            {
                spriteBatch.Draw(
                    texture: Game1.objectSpriteSheet,
                    position: __instance.getLocalPosition(Game1.viewport) + new Vector2(32f, 0f),
                    sourceRectangle: Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 435, 16, 16),
                    color: Color.White * alpha,
                    rotation: __instance.Scale.X,
                    origin: new Vector2(8f, 8f),
                    scale: 4f,
                    effects: SpriteEffects.None,
                    layerDepth: Math.Max(0f, (y + 1) * 64 / 10000f + 0.0001f + x * 1E-05f)
                );
            }

            if ((bool)__instance.isLamp && Game1.isDarkOut())
            {
                spriteBatch.Draw(
                    texture: Game1.mouseCursors,
                    position: vector + new Vector2(-32f, -32f),
                    sourceRectangle: new Rectangle(88, 1779, 32, 32),
                    color: Color.White * 0.75f,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: 4f,
                    effects: SpriteEffects.None,
                    layerDepth: Math.Max(0f, ((y + 1) * 64 - 20) / 10000f)
                );
            }

            if (__instance.ParentSheetIndex == 126 && __instance.Quality != 0)
            {
                spriteBatch.Draw(
                    texture: FarmerRenderer.hatsTexture,
                    position: vector + new Vector2(-3f, -6f) * 4f,
                    sourceRectangle: new Rectangle(
                        (__instance.Quality - 1) * 20 % FarmerRenderer.hatsTexture.Width,
                        (__instance.Quality - 1) * 20 / FarmerRenderer.hatsTexture.Width * 20 * 4,
                        20,
                        20
                    ),
                    color: Color.White * alpha,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: 4f,
                    effects: SpriteEffects.None,
                    layerDepth: Math.Max(0f, ((y + 1) * 64 - 20) / 10000f) + x * 1E-05f
                );
            }

            TimeSpan timeSpan = DateTime.UtcNow.TimeOfDay;
            float num6 = 4f * (float)Math.Round(Math.Sin(timeSpan.TotalMilliseconds / 250.0), 2);
            spriteBatch.Draw(
                texture: Game1.mouseCursors,
                position: Game1.GlobalToLocal(
                    viewport: Game1.viewport,
                    globalPosition: new Vector2(
                        x * 64 - 8,
                        y * 64 - 96 - 16 + num6
                    )
                ),
                sourceRectangle: new Rectangle(141, 465, 20, 24),
                color: Color.White * 0.75f,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: (y + 1) * 64 / 10000f + 1E-06f + __instance.tileLocation.X / 10000f
                    + ((__instance.parentSheetIndex == 105) ? 0.0015f : 0f)
            );

            spriteBatch.Draw(
                texture: texture,
                position: Game1.GlobalToLocal(
                    viewport: Game1.viewport,
                    globalPosition: new Vector2(
                        x * 64 + 32,
                        y * 64 - 64 - 8 + num6
                    )
                ),
                sourceRectangle: sourceRect,
                color: Color.White * 0.75f,
                rotation: 0f,
                origin: new Vector2(8f, 8f),
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: (y + 1) * 64 / 10000f + 1E-05f + __instance.tileLocation.X / 10000f
                    + ((__instance.parentSheetIndex == 105) ? 0.0015f : 0f)
            );

            return false;
        }
    }
}
