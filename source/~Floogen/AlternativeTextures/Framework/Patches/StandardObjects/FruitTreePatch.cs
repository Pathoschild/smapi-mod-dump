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
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace AlternativeTextures.Framework.Patches.StandardObjects
{
    internal class FruitTreePatch : PatchTemplate
    {
        private readonly Type _object = typeof(FruitTree);

        internal FruitTreePatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(FruitTree.draw), new[] { typeof(SpriteBatch), typeof(Vector2) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(FruitTree.seasonUpdate), new[] { typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(SeasonUpdatePostfix)));
            harmony.Patch(AccessTools.Constructor(typeof(FruitTree), new[] { typeof(int) }), postfix: new HarmonyMethod(GetType(), nameof(FruitTreePostfix)));
            harmony.Patch(AccessTools.Constructor(typeof(FruitTree), new[] { typeof(int), typeof(int) }), postfix: new HarmonyMethod(GetType(), nameof(FruitTreePostfix)));

            if (PatchTemplate.IsDGAUsed())
            {
                try
                {
                    if (Type.GetType("DynamicGameAssets.Game.CustomFruitTree, DynamicGameAssets") is Type dgaCropType && dgaCropType != null)
                    {
                        harmony.Patch(AccessTools.Method(dgaCropType, nameof(FruitTree.draw), new[] { typeof(SpriteBatch), typeof(Vector2) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
                        harmony.Patch(AccessTools.Method(dgaCropType, nameof(FruitTree.seasonUpdate), new[] { typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(SeasonUpdatePostfix)));
                    }
                }
                catch (Exception ex)
                {
                    _monitor.Log($"Failed to patch Dynamic Game Assets in {this.GetType().Name}: AT may not be able to override certain DGA object types!", LogLevel.Warn);
                    _monitor.Log($"Patch for DGA failed in {this.GetType().Name}: {ex}", LogLevel.Trace);
                }
            }
        }

        private static bool DrawPrefix(FruitTree __instance, float ___shakeRotation, float ___shakeTimer, float ___alpha, List<Leaf> ___leaves, NetBool ___falling, SpriteBatch spriteBatch, Vector2 tileLocation)
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

                string season = Game1.GetSeasonForLocation(__instance.currentLocation);
                if ((bool)__instance.greenHouseTileTree)
                {
                    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), new Rectangle(669, 1957, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-08f);
                }

                var textureOffset = textureModel.GetTextureOffset(textureVariation);
                if ((int)__instance.growthStage < 4)
                {
                    Vector2 positionOffset = new Vector2((float)Math.Max(-8.0, Math.Min(64.0, Math.Sin((double)(tileLocation.X * 200f) / (Math.PI * 2.0)) * -16.0)), (float)Math.Max(-8.0, Math.Min(64.0, Math.Sin((double)(tileLocation.X * 200f) / (Math.PI * 2.0)) * -16.0))) / 2f;
                    Rectangle sourceRect = Rectangle.Empty;
                    switch ((int)__instance.growthStage)
                    {
                        case 0:
                            sourceRect = new Rectangle(0, textureOffset * 5 * 16, 48, 80);
                            break;
                        case 1:
                            sourceRect = new Rectangle(48, textureOffset * 5 * 16, 48, 80);
                            break;
                        case 2:
                            sourceRect = new Rectangle(96, textureOffset * 5 * 16, 48, 80);
                            break;
                        default:
                            sourceRect = new Rectangle(144, textureOffset * 5 * 16, 48, 80);
                            break;
                    }

                    spriteBatch.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 32f + positionOffset.X, tileLocation.Y * 64f - (float)sourceRect.Height + 128f + positionOffset.Y)), sourceRect, Color.White, ___shakeRotation, new Vector2(24f, 80f), 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float)__instance.getBoundingBox(tileLocation).Bottom / 10000f - tileLocation.X / 1000000f);
                }
                else
                {
                    if (!__instance.stump || (bool)___falling)
                    {
                        if (!___falling)
                        {
                            spriteBatch.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 32f, tileLocation.Y * 64f + 64f)), new Rectangle((12 + (__instance.greenHouseTree ? 1 : Utility.getSeasonNumber(season)) * 3) * 16, textureOffset * 5 * 16 + 64, 48, 16), ((int)__instance.struckByLightningCountdown > 0) ? (Color.Gray * ___alpha) : (Color.White * ___alpha), 0f, new Vector2(24f, 16f), 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1E-07f);
                        }
                        spriteBatch.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 32f, tileLocation.Y * 64f + 64f)), new Rectangle((12 + (__instance.greenHouseTree ? 1 : Utility.getSeasonNumber(season)) * 3) * 16, textureOffset * 5 * 16, 48, 64), ((int)__instance.struckByLightningCountdown > 0) ? (Color.Gray * ___alpha) : (Color.White * ___alpha), ___shakeRotation, new Vector2(24f, 80f), 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float)__instance.getBoundingBox(tileLocation).Bottom / 10000f + 0.001f - tileLocation.X / 1000000f);
                    }
                    if ((float)__instance.health >= 1f || (!___falling && (float)__instance.health > -99f))
                    {
                        spriteBatch.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 32f + ((___shakeTimer > 0f) ? ((float)Math.Sin(Math.PI * 2.0 / (double)___shakeTimer) * 2f) : 0f), tileLocation.Y * 64f + 64f)), new Rectangle(384, textureOffset * 5 * 16 + 48, 48, 32), ((int)__instance.struckByLightningCountdown > 0) ? (Color.Gray * ___alpha) : (Color.White * ___alpha), 0f, new Vector2(24f, 32f), 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, ((bool)__instance.stump && !___falling) ? ((float)__instance.getBoundingBox(tileLocation).Bottom / 10000f) : ((float)__instance.getBoundingBox(tileLocation).Bottom / 10000f - 0.001f - tileLocation.X / 1000000f));
                    }
                    for (int i = 0; i < (int)__instance.fruitsOnTree; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f - 64f + tileLocation.X * 200f % 64f / 2f, tileLocation.Y * 64f - 192f - tileLocation.X % 64f / 3f)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, ((int)__instance.struckByLightningCountdown > 0) ? 382 : ((int)__instance.indexOfFruit), 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)__instance.getBoundingBox(tileLocation).Bottom / 10000f + 0.002f - tileLocation.X / 1000000f);
                                break;
                            case 1:
                                spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 32f, tileLocation.Y * 64f - 256f + tileLocation.X * 232f % 64f / 3f)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, ((int)__instance.struckByLightningCountdown > 0) ? 382 : ((int)__instance.indexOfFruit), 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)__instance.getBoundingBox(tileLocation).Bottom / 10000f + 0.002f - tileLocation.X / 1000000f);
                                break;
                            case 2:
                                spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + tileLocation.X * 200f % 64f / 3f, tileLocation.Y * 64f - 160f + tileLocation.X * 200f % 64f / 3f)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, ((int)__instance.struckByLightningCountdown > 0) ? 382 : ((int)__instance.indexOfFruit), 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, (float)__instance.getBoundingBox(tileLocation).Bottom / 10000f + 0.002f - tileLocation.X / 1000000f);
                                break;
                        }
                    }
                }
                foreach (Leaf j in ___leaves)
                {
                    spriteBatch.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, j.position), new Rectangle((24 + Utility.getSeasonNumber(season)) * 16, textureOffset * 5 * 16, 8, 8), Color.White, j.rotation, Vector2.Zero, 4f, SpriteEffects.None, (float)__instance.getBoundingBox(tileLocation).Bottom / 10000f + 0.01f);
                }

                return false;
            }
            return true;
        }

        private static void SeasonUpdatePostfix(FruitTree __instance, bool onLoad)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME) && __instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_SEASON) && !String.IsNullOrEmpty(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]) && __instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_SAPLING_NAME))
            {
                __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON] = Game1.GetSeasonForLocation(__instance.currentLocation);
                __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME] = String.Concat(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_OWNER], ".", $"{AlternativeTextureModel.TextureType.FruitTree}_{__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SAPLING_NAME]}_{__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]}");
            }
        }

        private static void FruitTreePostfix(FruitTree __instance, int saplingIndex)
        {
            var saplingName = Game1.objectInformation.ContainsKey(saplingIndex) ? Game1.objectInformation[saplingIndex].Split('/')[0] : String.Empty;
            var instanceName = $"{AlternativeTextureModel.TextureType.FruitTree}_{saplingName}";
            var instanceSeasonName = $"{instanceName}_{Game1.GetSeasonForLocation(__instance.currentLocation)}";

            if (AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceName) && AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceSeasonName))
            {
                __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SAPLING_NAME] = saplingName;
                var result = Game1.random.Next(2) > 0 ? AssignModData(__instance, instanceSeasonName, true) : AssignModData(__instance, instanceName, false);
                return;
            }
            else
            {
                if (AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceName))
                {
                    __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SAPLING_NAME] = saplingName;
                    AssignModData(__instance, instanceName, false);
                    return;
                }

                if (AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceSeasonName))
                {
                    __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SAPLING_NAME] = saplingName;
                    AssignModData(__instance, instanceSeasonName, true);
                    return;
                }
            }

            __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SAPLING_NAME] = saplingName;
            AssignDefaultModData(__instance, instanceSeasonName, true);
        }
    }
}
