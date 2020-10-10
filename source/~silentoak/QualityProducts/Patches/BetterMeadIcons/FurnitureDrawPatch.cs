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
using Netcode;
using SilentOak.Patching;
using SilentOak.QualityProducts.Utils;
using StardewValley;
using StardewValley.Objects;

/***
 * Inspired by https://github.com/danvolchek/StardewMods/blob/32046f848ea1a1aade495b9adad612f8b94928d1/BetterArtisanGoodIcons/Patches/FurniturePatches/DrawPatch.cs
 ***/
namespace SilentOak.QualityProducts.Patches.BetterMeadIcons
{
    /// <summary>
    /// Patch for custom mead textures.
    /// </summary>
    public static class FurnitureDrawPatch
    {
        public static readonly PatchData PatchData = new PatchData(
            type: typeof(Furniture),
            originalMethodName: "draw",
            originalMethodParams: new Type[]
            {
                typeof(SpriteBatch),
                typeof(int),
                typeof(int),
                typeof(float)
            }
        );

        /// <summary>
        /// Patch for drawing a custom sprite (if available) for the furniture's held object.
        /// </summary>
        /// <returns>If original method should be executed.</returns>
        /// <param name="__instance">The furniture instance.</param>
        /// <param name="spriteBatch">Sprite batch to draw onto.</param>
        /// <param name="x">The x tile coordinate.</param>
        /// <param name="y">The y tile coordinate.</param>
        /// <param name="alpha">Transparency.</param>
        public static bool Prefix(
            Furniture __instance,
            SpriteBatch spriteBatch,
            int x,
            int y,
            float alpha = 1f
        )
        {
            if (
                !SpriteLoader.TryLoadSprite(__instance.heldObject.Value, out Texture2D texture, out Rectangle sourceRect)
            )
            {
                return true;
            }


            Vector2 drawPosition = Util.Helper.Reflection.GetField<NetVector2>(__instance, "drawPosition").GetValue().Value;

            Rectangle rectangle;
            if (x == -1)
            {
                float layerDepth;

                if ((int)__instance.furniture_type != Furniture.rug)
                {
                    rectangle = __instance.boundingBox.Value;
                    layerDepth = (rectangle.Bottom - 8) / 10000f;
                }
                else
                {
                    layerDepth = 0f;
                }

                spriteBatch.Draw(
                    texture: Furniture.furnitureTexture,
                    position: Game1.GlobalToLocal(Game1.viewport, drawPosition),
                    sourceRectangle: __instance.sourceRect,
                    color: Color.White * alpha,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: 4f,
                    effects: __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    layerDepth: layerDepth
                );
            }
            else
            {
                float layerDepth2;

                if ((int)__instance.furniture_type != Furniture.rug)
                {
                    rectangle = __instance.boundingBox.Value;
                    layerDepth2 = (rectangle.Bottom - 8) / 10000f;
                }
                else
                {
                    layerDepth2 = 0f;
                }

                spriteBatch.Draw(
                    texture: Furniture.furnitureTexture,
                    position: Game1.GlobalToLocal(
                        viewport: Game1.viewport,
                        globalPosition: new Vector2(
                            x * 64,
                            y * 64 - (__instance.sourceRect.Height * 4 - __instance.boundingBox.Height)
                        )
                    ),
                    sourceRectangle: __instance.sourceRect,
                    color: Color.White * alpha,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: 4f,
                    effects: __instance.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    layerDepth: layerDepth2
                );
            }

            Rectangle bounds = Game1.shadowTexture.Bounds;
            spriteBatch.Draw(
                texture: Game1.shadowTexture,
                position: Game1.GlobalToLocal(
                    viewport: Game1.viewport,
                    globalPosition: new Vector2(
                        __instance.boundingBox.Center.X - 32,
                        __instance.boundingBox.Center.Y - (((bool)__instance.drawHeldObjectLow) ? 32 : 85)
                    )
                ) + new Vector2(32f, 53f),
                sourceRectangle: bounds,
                color: Color.White * alpha,
                rotation: 0f,
                origin: new Vector2(bounds.Center.X, bounds.Center.Y),
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: __instance.boundingBox.Bottom / 10000f
            );

            spriteBatch.Draw(
                texture: texture,
                position: Game1.GlobalToLocal(
                    viewport: Game1.viewport,
                    globalPosition: new Vector2(
                        __instance.boundingBox.Center.X - 32,
                        __instance.boundingBox.Center.Y - (((bool)__instance.drawHeldObjectLow) ? 32 : 85)
                    )
                ),
                sourceRectangle: sourceRect,
                color: Color.White * alpha,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 4f,
                effects: SpriteEffects.None,
                layerDepth: (__instance.boundingBox.Bottom + 1) / 10000f
            );

            if ((bool)__instance.isOn && (int)__instance.furniture_type == Furniture.fireplace)
            {
                Vector2 position4 = Game1.GlobalToLocal(
                    viewport: Game1.viewport,
                    globalPosition: new Vector2(
                        __instance.boundingBox.Center.X - 12,
                        __instance.boundingBox.Center.Y - 64
                    )
                );

                TimeSpan totalGameTime = Game1.currentGameTime.TotalGameTime;

                rectangle = __instance.getBoundingBox(new Vector2(x, y));

                spriteBatch.Draw(
                    texture: Game1.mouseCursors,
                    position: position4,
                    sourceRectangle: new Rectangle(
                        276 + (int)((totalGameTime.TotalMilliseconds + x * 3047 + y * 88) % 400.0 / 100.0) * 12,
                        1985,
                        12,
                        11
                    ), 
                    color: Color.White,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: 4f,
                    effects: SpriteEffects.None,
                    layerDepth: (rectangle.Bottom - 2) / 10000f
                );

                Vector2 position5 = Game1.GlobalToLocal(
                    viewport: Game1.viewport,
                    globalPosition: new Vector2(
                        __instance.boundingBox.Center.X - 32 - 4,
                        __instance.boundingBox.Center.Y - 64
                    )
                );

                totalGameTime = Game1.currentGameTime.TotalGameTime;
                rectangle = __instance.getBoundingBox(new Vector2(x, y));
                spriteBatch.Draw(
                    texture: Game1.mouseCursors,
                    position: position5,
                    sourceRectangle: new Rectangle(
                        276 + (int)((totalGameTime.TotalMilliseconds + x * 2047 + y * 98) % 400.0 / 100.0) * 12,
                        1985,
                        12,
                        11
                    ),
                    color: Color.White,
                    rotation: 0f,
                    origin: Vector2.Zero,
                    scale: 4f,
                    effects: SpriteEffects.None,
                    layerDepth: (rectangle.Bottom - 1) / 10000f
                );
            }

            return false;
        }
    }
}
