/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures.Framework.Models;
using AlternativeTextures.Framework.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.FloorsAndPaths;
using StardewValley.TerrainFeatures;
using System;

namespace AlternativeTextures.Framework.Patches.StandardObjects
{
    internal class FlooringPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Flooring);

        public FlooringPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Flooring.draw), new[] { typeof(SpriteBatch) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Flooring.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(Vector2), typeof(float), typeof(float) }), prefix: new HarmonyMethod(GetType(), nameof(DrawInMenuPrefix)));
            harmony.Patch(AccessTools.Method(typeof(TerrainFeature), nameof(TerrainFeature.seasonUpdate), new[] { typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(SeasonUpdatePostfix)));
        }

        private static bool DrawPrefix(Flooring __instance, byte ___neighborMask, SpriteBatch spriteBatch)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME]);
                if (textureModel is null)
                {
                    return true;
                }

                var textureVariation = Int32.Parse(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_VARIATION]);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    return true;
                }
                var textureOffset = textureModel.GetTextureOffset(textureVariation);

                Vector2 tileLocation = __instance.Tile;

                FloorPathData data = __instance.GetData();
                Texture2D texture = textureModel.GetTexture(textureVariation);
                Point corner = __instance.GetTextureCorner();
                float cornerSortOffset = 1f;
                switch (data.ConnectType)
                {
                    case FloorPathConnectType.CornerDecorated:
                        {
                            int border_size = data.CornerSize;
                            if ((___neighborMask & 9) == 9 && (___neighborMask & 0x20) == 0)
                            {
                                spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), new Rectangle(64 - border_size + corner.X, 48 - border_size + corner.Y, border_size, border_size), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (tileLocation.Y * 64f + 2f + tileLocation.X / 10000f) / 20000f);
                            }
                            if ((___neighborMask & 3) == 3 && (___neighborMask & 0x10) == 0)
                            {
                                spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 64f - (float)(border_size * 4), tileLocation.Y * 64f)), new Rectangle(16 + corner.X, 48 - border_size + corner.Y, border_size, border_size), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (tileLocation.Y * 64f + 2f + tileLocation.X / 10000f + cornerSortOffset) / 20000f);
                            }
                            if ((___neighborMask & 6) == 6 && (___neighborMask & 0x40) == 0)
                            {
                                spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 64f - (float)(border_size * 4), tileLocation.Y * 64f + 64f - (float)(border_size * 4))), new Rectangle(16 + corner.X, corner.Y, border_size, border_size), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (tileLocation.Y * 64f + 2f + tileLocation.X / 10000f) / 20000f);
                            }
                            if ((___neighborMask & 0xC) == 12 && (___neighborMask & 0x80) == 0)
                            {
                                spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f + 64f - (float)(border_size * 4))), new Rectangle(64 - border_size + corner.X, corner.Y, border_size, border_size), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (tileLocation.Y * 64f + 2f + tileLocation.X / 10000f) / 20000f);
                            }
                            break;
                        }
                    case FloorPathConnectType.Default:
                        {
                            int borderSize = data.CornerSize;
                            if ((___neighborMask & 9) == 9 && (___neighborMask & 0x20) == 0)
                            {
                                spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), new Rectangle(64 - borderSize + corner.X, 48 - borderSize + corner.Y, borderSize, borderSize), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (tileLocation.Y * 64f + 2f + tileLocation.X / 10000f) / 20000f);
                            }
                            if ((___neighborMask & 3) == 3 && (___neighborMask & 0x10) == 0)
                            {
                                spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 64f - (float)(borderSize * 4), tileLocation.Y * 64f)), new Rectangle(16 + corner.X, 48 - borderSize + corner.Y, borderSize, borderSize), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (tileLocation.Y * 64f + 2f + tileLocation.X / 10000f + cornerSortOffset) / 20000f);
                            }
                            if ((___neighborMask & 6) == 6 && (___neighborMask & 0x40) == 0)
                            {
                                spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 64f - (float)(borderSize * 4), tileLocation.Y * 64f + 48f)), new Rectangle(16 + corner.X, corner.Y, borderSize, borderSize), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (tileLocation.Y * 64f + 2f + tileLocation.X / 10000f) / 20000f);
                            }
                            if ((___neighborMask & 0xC) == 12 && (___neighborMask & 0x80) == 0)
                            {
                                spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f + 64f - (float)(borderSize * 4))), new Rectangle(64 - borderSize + corner.X, corner.Y, borderSize, borderSize), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (tileLocation.Y * 64f + 2f + tileLocation.X / 10000f) / 20000f);
                            }
                            break;
                        }
                }

                byte drawSum = (byte)(___neighborMask & 0xFu);
                int sourceRectPosition = Flooring.drawGuide[drawSum];
                if (data.ConnectType == FloorPathConnectType.Random)
                {
                    sourceRectPosition = Flooring.drawGuideList[__instance.whichView.Value];
                }
                switch (data.ShadowType)
                {
                    case FloorPathShadowType.Square:
                        spriteBatch.Draw(Game1.staminaRect, new Rectangle((int)(tileLocation.X * 64f) - 4 - Game1.viewport.X, (int)(tileLocation.Y * 64f) + 4 - Game1.viewport.Y, 64, 64), Color.Black * 0.33f);
                        break;
                    case FloorPathShadowType.Contoured:
                        {
                            Color shadowColor = Color.Black;
                            shadowColor.A = (byte)((float)(int)shadowColor.A * 0.33f);
                            spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)) + new Vector2(-4f, 4f), new Rectangle(corner.X + sourceRectPosition * 16 % 256, sourceRectPosition / 16 * 16 + corner.Y, 16, 16), shadowColor, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-10f);
                            break;
                        }
                }

                spriteBatch.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), new Rectangle(sourceRectPosition * 16 % 256, (sourceRectPosition / 16 * 16) + textureOffset, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-09f);

                return false;
            }
            return true;
        }

        private static bool DrawInMenuPrefix(Flooring __instance, SpriteBatch spriteBatch, Vector2 positionOnScreen, Vector2 tileLocation, float scale, float layerDepth)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME) && !PatchTemplate.IsDGAObject(__instance))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME]);
                if (textureModel is null)
                {
                    return true;
                }

                var textureVariation = Int32.Parse(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_VARIATION]);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    return true;
                }
                var textureOffset = textureModel.GetTextureOffset(textureVariation);

                int sourceRectPosition = 1;
                byte drawSum = 0;
                Vector2 surroundingLocations = tileLocation;
                surroundingLocations.X += 1f;
                GameLocation farm = Game1.getLocationFromName("Farm");
                if (farm.terrainFeatures.ContainsKey(surroundingLocations) && farm.terrainFeatures[surroundingLocations] is Flooring)
                {
                    drawSum = (byte)(drawSum + 2);
                }
                surroundingLocations.X -= 2f;
                if (farm.terrainFeatures.ContainsKey(surroundingLocations) && Game1.currentLocation.terrainFeatures[surroundingLocations] is Flooring)
                {
                    drawSum = (byte)(drawSum + 8);
                }
                surroundingLocations.X += 1f;
                surroundingLocations.Y += 1f;
                if (Game1.currentLocation.terrainFeatures.ContainsKey(surroundingLocations) && farm.terrainFeatures[surroundingLocations] is Flooring)
                {
                    drawSum = (byte)(drawSum + 4);
                }
                surroundingLocations.Y -= 2f;
                if (farm.terrainFeatures.ContainsKey(surroundingLocations) && farm.terrainFeatures[surroundingLocations] is Flooring)
                {
                    drawSum = (byte)(drawSum + 1);
                }
                sourceRectPosition = Flooring.drawGuide[drawSum];
                spriteBatch.Draw(textureModel.GetTexture(textureVariation), positionOnScreen, new Rectangle(sourceRectPosition % 16 * 16, sourceRectPosition / 16 * 16 + textureOffset, 16, 16), Color.White, 0f, Vector2.Zero, scale * 4f, SpriteEffects.None, layerDepth + positionOnScreen.Y / 20000f);

                return false;
            }
            return true;
        }

        private static void SeasonUpdatePostfix(TerrainFeature __instance, bool onLoad)
        {
            if (__instance is not Flooring flooring || __instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_OWNER) is false || __instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME) is false)
            {
                return;
            }

            var season = Game1.GetSeasonForLocation(__instance.Location).ToString();
            var seasonalName = String.Concat(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_OWNER], ".", $"{AlternativeTextureModel.TextureType.Flooring}_{GetFlooringName(flooring)}_{season}");
            if ((__instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME) && __instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_SEASON) && !String.IsNullOrEmpty(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]) && !String.Equals(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON], season, StringComparison.OrdinalIgnoreCase)) || AlternativeTextures.textureManager.DoesObjectHaveAlternativeTextureById(seasonalName))
            {
                __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON] = season;
                __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME] = seasonalName;
            }
        }
    }
}
