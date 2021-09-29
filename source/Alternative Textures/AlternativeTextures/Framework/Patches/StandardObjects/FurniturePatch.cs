/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures;
using AlternativeTextures.Framework.Models;
using AlternativeTextures.Framework.UI;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches.StandardObjects
{
    internal class FurniturePatch : PatchTemplate
    {
        private readonly Type _object = typeof(Furniture);

        internal FurniturePatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Furniture.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Furniture.drawAtNonTileSpot), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawAtNonTileSpotPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Furniture.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(DrawInMenuPrefix)));

            if (PatchTemplate.IsDGAUsed())
            {
                try
                {
                    if (Type.GetType("DynamicGameAssets.Game.CustomBasicFurniture, DynamicGameAssets") is Type dgaFurnitureType && dgaFurnitureType != null)
                    {
                        harmony.Patch(AccessTools.Method(dgaFurnitureType, nameof(Furniture.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
                        harmony.Patch(AccessTools.Method(dgaFurnitureType, nameof(Furniture.drawAtNonTileSpot), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawAtNonTileSpotPrefix)));
                        harmony.Patch(AccessTools.Method(dgaFurnitureType, nameof(Furniture.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(DrawInMenuPrefix)));
                    }

                    if (Type.GetType("DynamicGameAssets.Game.CustomStorageFurniture, DynamicGameAssets") is Type dgaStorageFurnitureType && dgaStorageFurnitureType != null)
                    {
                        harmony.Patch(AccessTools.Method(dgaStorageFurnitureType, nameof(Furniture.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
                        harmony.Patch(AccessTools.Method(dgaStorageFurnitureType, nameof(Furniture.drawAtNonTileSpot), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawAtNonTileSpotPrefix)));
                        harmony.Patch(AccessTools.Method(dgaStorageFurnitureType, nameof(Furniture.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(DrawInMenuPrefix)));
                    }

                    if (Type.GetType("DynamicGameAssets.Game.CustomTVFurniture, DynamicGameAssets") is Type dgaTVFurnitureType && dgaTVFurnitureType != null)
                    {
                        harmony.Patch(AccessTools.Method(dgaTVFurnitureType, nameof(Furniture.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
                        harmony.Patch(AccessTools.Method(dgaTVFurnitureType, nameof(Furniture.drawAtNonTileSpot), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawAtNonTileSpotPrefix)));
                        harmony.Patch(AccessTools.Method(dgaTVFurnitureType, nameof(Furniture.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(DrawInMenuPrefix)));
                    }
                }
                catch (Exception ex)
                {
                    _monitor.Log($"Failed to patch Dynamic Game Assets in {this.GetType().Name}: AT may not be able to override certain DGA object types!", LogLevel.Warn);
                    _monitor.Log($"Patch for DGA failed in {this.GetType().Name}: {ex}", LogLevel.Trace);
                }
            }
        }

        internal static bool DrawPrefix(Furniture __instance, NetInt ___sourceIndexOffset, NetVector2 ___drawPosition, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (__instance.modData.ContainsKey("AlternativeTextureName"))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]);
                if (textureModel is null)
                {
                    return true;
                }

                var textureVariation = Int32.Parse(__instance.modData["AlternativeTextureVariation"]);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    return true;
                }
                var textureOffset = textureVariation * textureModel.TextureHeight;

                // Replicate the base draw
                if (__instance.isTemporarilyInvisible)
                {
                    return true;
                }


                // Set xTileOffset if AlternativeTextureModel has an animation
                var xTileOffset = 0;
                if (textureModel.HasAnimation(textureVariation))
                {
                    if (!__instance.modData.ContainsKey("AlternativeTextureCurrentFrame") || !__instance.modData.ContainsKey("AlternativeTextureFrameDuration") || !__instance.modData.ContainsKey("AlternativeTextureElapsedDuration"))
                    {
                        __instance.modData["AlternativeTextureCurrentFrame"] = "0";
                        __instance.modData["AlternativeTextureFrameDuration"] = textureModel.GetAnimationDataAtIndex(textureVariation, 0).Duration.ToString();// Animation.ElementAt(0).Duration.ToString();
                        __instance.modData["AlternativeTextureElapsedDuration"] = "0";
                    }

                    var currentFrame = Int32.Parse(__instance.modData["AlternativeTextureCurrentFrame"]);
                    var frameDuration = Int32.Parse(__instance.modData["AlternativeTextureFrameDuration"]);
                    var elapsedDuration = Int32.Parse(__instance.modData["AlternativeTextureElapsedDuration"]);

                    if (elapsedDuration >= frameDuration)
                    {
                        currentFrame = currentFrame + 1 >= textureModel.GetAnimationData(textureVariation).Count() ? 0 : currentFrame + 1;

                        __instance.modData["AlternativeTextureCurrentFrame"] = currentFrame.ToString();
                        __instance.modData["AlternativeTextureFrameDuration"] = textureModel.GetAnimationDataAtIndex(textureVariation, currentFrame).Duration.ToString();
                        __instance.modData["AlternativeTextureElapsedDuration"] = "0";
                    }
                    else
                    {
                        __instance.modData["AlternativeTextureElapsedDuration"] = (elapsedDuration + Game1.currentGameTime.ElapsedGameTime.Milliseconds).ToString();
                    }

                    xTileOffset = currentFrame;
                }

                Rectangle sourceRect = __instance.sourceRect.Value;
                sourceRect.X -= __instance.defaultSourceRect.X;
                sourceRect.X += (___sourceIndexOffset * sourceRect.Width) + (xTileOffset * sourceRect.Width);
                sourceRect.Y = textureOffset;
                if (Furniture.isDrawingLocationFurniture)
                {
                    if (__instance.HasSittingFarmers() && __instance.sourceRect.Right <= Furniture.furnitureFrontTexture.Width && __instance.sourceRect.Bottom <= Furniture.furnitureFrontTexture.Height)
                    {
                        spriteBatch.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, ___drawPosition + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero)), sourceRect, Color.White * alpha, 0f, Vector2.Zero, 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float)(__instance.boundingBox.Value.Top + 16) / 10000f);

                        var rotationSourceRect = sourceRect;
                        rotationSourceRect.Y += textureModel.TextureHeight / 2;
                        spriteBatch.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, ___drawPosition + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero)), rotationSourceRect, Color.White * alpha, 0f, Vector2.Zero, 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float)(__instance.boundingBox.Value.Bottom - 8) / 10000f);
                    }
                    else
                    {
                        spriteBatch.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, ___drawPosition + ((__instance.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero)), sourceRect, Color.White * alpha, 0f, Vector2.Zero, 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, ((int)__instance.furniture_type == 12) ? (2E-09f + __instance.tileLocation.Y / 100000f) : ((float)(__instance.boundingBox.Value.Bottom - (((int)__instance.furniture_type == 6 || (int)__instance.furniture_type == 17 || (int)__instance.furniture_type == 13) ? 48 : 8)) / 10000f));
                    }
                }
                else
                {
                    spriteBatch.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), y * 64 - (__instance.sourceRect.Height * 4 - __instance.boundingBox.Height) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0))), __instance.sourceRect, Color.White * alpha, 0f, Vector2.Zero, 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, ((int)__instance.furniture_type == 12) ? (2E-09f + __instance.tileLocation.Y / 100000f) : ((float)(__instance.boundingBox.Value.Bottom - (((int)__instance.furniture_type == 6 || (int)__instance.furniture_type == 17 || (int)__instance.furniture_type == 13) ? 48 : 8)) / 10000f));
                }
                if (__instance.heldObject.Value != null)
                {
                    if (__instance.heldObject.Value is Furniture)
                    {
                        (__instance.heldObject.Value as Furniture).drawAtNonTileSpot(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 32, __instance.boundingBox.Center.Y - (__instance.heldObject.Value as Furniture).sourceRect.Height * 4 - (__instance.drawHeldObjectLow ? (-16) : 16))), (float)(__instance.boundingBox.Bottom - 7) / 10000f, alpha);
                    }
                    else if (HeldObjectDraw(__instance.heldObject, spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 32, __instance.boundingBox.Center.Y - (__instance.drawHeldObjectLow ? 32 : 85))), (float)(__instance.boundingBox.Bottom + 1) / 10000f, alpha))
                    {
                        spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 32, __instance.boundingBox.Center.Y - (__instance.drawHeldObjectLow ? 32 : 85))) + new Vector2(32f, 53f), Game1.shadowTexture.Bounds, Color.White * alpha, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (float)__instance.boundingBox.Bottom / 10000f);
                        spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 32, __instance.boundingBox.Center.Y - (__instance.drawHeldObjectLow ? 32 : 85))), GameLocation.getSourceRectForObject(__instance.heldObject.Value.ParentSheetIndex), Color.White * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(__instance.boundingBox.Bottom + 1) / 10000f);
                    }
                }
                if ((bool)__instance.isOn && (int)__instance.furniture_type == 14)
                {
                    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 12, __instance.boundingBox.Center.Y - 64)), new Rectangle(276 + (int)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double)(x * 3047) + (double)(y * 88)) % 400.0 / 100.0) * 12, 1985, 12, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(__instance.getBoundingBox(new Vector2(x, y)).Bottom - 2) / 10000f);
                    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 32 - 4, __instance.boundingBox.Center.Y - 64)), new Rectangle(276 + (int)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double)(x * 2047) + (double)(y * 98)) % 400.0 / 100.0) * 12, 1985, 12, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(__instance.getBoundingBox(new Vector2(x, y)).Bottom - 1) / 10000f);
                }
                else if ((bool)__instance.isOn && (int)__instance.furniture_type == 16)
                {
                    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.boundingBox.Center.X - 20, (float)__instance.boundingBox.Center.Y - 105.6f)), new Rectangle(276 + (int)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + (double)(x * 3047) + (double)(y * 88)) % 400.0 / 100.0) * 12, 1985, 12, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(__instance.getBoundingBox(new Vector2(x, y)).Bottom - 2) / 10000f);
                }

                return false;
            }
            return true;
        }

        private static bool HeldObjectDraw(Object __instance, SpriteBatch spriteBatch, Vector2 location, float layerDepth, float alpha = 1f)
        {
            if (__instance.modData.ContainsKey("AlternativeTextureName"))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]);
                if (textureModel is null)
                {
                    return true;
                }

                var textureVariation = Int32.Parse(__instance.modData["AlternativeTextureVariation"]);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    return true;
                }
                var textureOffset = textureVariation * textureModel.TextureHeight;

                // Get the current X index for the source tile
                var xTileOffset = __instance.modData.ContainsKey("AlternativeTextureSheetId") ? __instance.ParentSheetIndex - Int32.Parse(__instance.modData["AlternativeTextureSheetId"]) : 0;
                if (__instance.showNextIndex)
                {
                    xTileOffset += 1;
                }
                xTileOffset *= textureModel.TextureWidth;

                // Replicate the base draw
                Rectangle sourceRect = new Rectangle(xTileOffset, textureOffset, textureModel.TextureWidth, textureModel.TextureHeight);
                spriteBatch.Draw(textureModel.GetTexture(textureVariation), location, sourceRect, Color.White * alpha, 0f, Vector2.Zero, 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);

                return false;
            }
            return true;
        }

        private static bool DrawAtNonTileSpotPrefix(Furniture __instance, NetInt ___sourceIndexOffset, SpriteBatch spriteBatch, Vector2 location, float layerDepth, float alpha = 1f)
        {
            if (__instance.modData.ContainsKey("AlternativeTextureName"))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]);
                if (textureModel is null)
                {
                    return true;
                }

                var textureVariation = Int32.Parse(__instance.modData["AlternativeTextureVariation"]);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    return true;
                }
                var textureOffset = textureVariation * textureModel.TextureHeight;

                // Replicate the base draw
                Rectangle sourceRect = __instance.sourceRect.Value;
                sourceRect.X -= __instance.defaultSourceRect.X;
                sourceRect.Y = textureOffset;
                spriteBatch.Draw(textureModel.GetTexture(textureVariation), location, sourceRect, Color.White * alpha, 0f, Vector2.Zero, 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);

                return false;
            }
            return true;
        }

        private static bool DrawInMenuPrefix(Furniture __instance, NetInt ___sourceIndexOffset, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (Game1.activeClickableMenu is PaintBucketMenu && !PatchTemplate.IsDGAObject(__instance))
            {
                var texture = Furniture.furnitureTexture;
                var sourceRect = __instance.rotations > 1 ? __instance.sourceRect.Value : __instance.defaultSourceRect.Value;

                if (__instance.modData.ContainsKey("AlternativeTextureName") && AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]) is AlternativeTextureModel textureModel && Int32.TryParse(__instance.modData["AlternativeTextureVariation"], out int textureVariation) && textureVariation != -1)
                {
                    texture = textureModel.GetTexture(textureVariation);
                    sourceRect.X = Math.Max(0, __instance.sourceRect.X - __instance.defaultSourceRect.X);
                    sourceRect.Y = textureVariation * textureModel.TextureHeight;
                }

                sourceRect.X += sourceRect.Width * ___sourceIndexOffset.Value;
                spriteBatch.Draw(texture, location + new Vector2(32f, 32f), sourceRect, color * transparency, 0f, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 1f * GetScaleSize(sourceRect) * scaleSize, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);

                return false;
            }
            return true;
        }

        private static float GetScaleSize(Rectangle sourceRect)
        {
            int tilesWide = sourceRect.Width / 16;
            int tilesHigh = sourceRect.Height / 16;
            if (tilesWide >= 7)
            {
                return 0.5f;
            }
            if (tilesWide >= 6)
            {
                return 0.66f;
            }
            if (tilesWide >= 5)
            {
                return 0.75f;
            }
            if (tilesHigh >= 5)
            {
                return 0.8f;
            }
            if (tilesHigh >= 3)
            {
                return 1f;
            }
            if (tilesWide <= 2)
            {
                return 1.5f;
            }
            if (tilesWide <= 4)
            {
                return 1f;
            }
            return 0.1f;
        }
    }
}
