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
    internal class TreePatch : PatchTemplate
    {
        private readonly Type _object = typeof(Tree);

        internal TreePatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Tree.draw), new[] { typeof(SpriteBatch) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Tree.seasonUpdate), new[] { typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(SeasonUpdatePostfix)));
            harmony.Patch(AccessTools.Constructor(typeof(Tree), new[] { typeof(string) }), postfix: new HarmonyMethod(GetType(), nameof(TreePostfix)));
            harmony.Patch(AccessTools.Constructor(typeof(Tree), new[] { typeof(string), typeof(int), typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(TreePostfix)));
        }

        private static bool DrawPrefix(Tree __instance, float ___shakeRotation, float ___shakeTimer, float ___alpha, List<Leaf> ___leaves, NetBool ___falling, SpriteBatch spriteBatch)
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

                if (__instance.isTemporarilyInvisible)
                {
                    return true;
                }
                Vector2 tileLocation = __instance.Tile;

                var textureOffset = textureModel.GetTextureOffset(textureVariation);
                if ((int)__instance.growthStage < 5)
                {
                    Rectangle sourceRect = Rectangle.Empty;
                    switch ((int)__instance.growthStage)
                    {
                        case 0:
                            sourceRect = new Rectangle(32, 128, 16, 16);
                            break;
                        case 1:
                            sourceRect = new Rectangle(0, 128, 16, 16);
                            break;
                        case 2:
                            sourceRect = new Rectangle(16, 128, 16, 16);
                            break;
                        default:
                            sourceRect = new Rectangle(0, 96, 16, 32);
                            break;
                    }
                    sourceRect.Y += textureOffset;

                    spriteBatch.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 32f, tileLocation.Y * 64f - (float)(sourceRect.Height * 4 - 64) + (float)(((int)__instance.growthStage >= 3) ? 128 : 64))), sourceRect, __instance.fertilized ? Color.HotPink : Color.White, ___shakeRotation, new Vector2(8f, ((int)__instance.growthStage >= 3) ? 32 : 16), 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, ((int)__instance.growthStage == 0) ? 0.0001f : (__instance.getBoundingBox().Bottom / 10000f));
                }
                else
                {
                    if (!__instance.stump || (bool)___falling)
                    {
                        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f - 51f, tileLocation.Y * 64f - 16f)), Tree.shadowSourceRect, Color.White * ((float)Math.PI / 2f - Math.Abs(___shakeRotation)), 0f, Vector2.Zero, 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1E-06f);
                        Rectangle source_rect = Tree.treeTopSourceRect;

                        // TODO: Review if this code block is actually used
                        /*
                        if (__instance.treeType.Value == 9)
                        {
                            if (__instance.hasSeed.Value)
                            {
                                source_rect.X = 48;
                            }
                            else
                            {
                                source_rect.X = 0;
                            }
                        }
                        */
                        source_rect.Y += textureOffset;
                        spriteBatch.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 32f, tileLocation.Y * 64f + 64f)), source_rect, Color.White * ___alpha, ___shakeRotation, new Vector2(24f, 96f), 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float)(__instance.getBoundingBox().Bottom + 2) / 10000f - tileLocation.X / 1000000f);
                    }
                    if (__instance.health.Value >= 1f || (!___falling && __instance.health.Value > -99f))
                    {
                        spriteBatch.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + ((___shakeTimer > 0f) ? ((float)Math.Sin(Math.PI * 2.0 / (double)___shakeTimer) * 3f) : 0f), tileLocation.Y * 64f - 64f)), new Rectangle(Tree.stumpSourceRect.X, Tree.stumpSourceRect.Y + textureOffset, Tree.stumpSourceRect.Width, Tree.stumpSourceRect.Height), Color.White * ___alpha, 0f, Vector2.Zero, 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float)__instance.getBoundingBox().Bottom / 10000f);
                    }
                    if ((bool)__instance.stump && __instance.health.Value < 4f && __instance.health.Value > -99f)
                    {
                        spriteBatch.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + ((___shakeTimer > 0f) ? ((float)Math.Sin(Math.PI * 2.0 / (double)___shakeTimer) * 3f) : 0f), tileLocation.Y * 64f)), new Rectangle(Math.Min(2, (int)(3f - __instance.health.Value)) * 16, 144 + textureOffset, 16, 16), Color.White * ___alpha, 0f, Vector2.Zero, 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float)(__instance.getBoundingBox().Bottom + 1) / 10000f);
                    }
                }
                foreach (Leaf i in ___leaves)
                {
                    spriteBatch.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, i.position), new Rectangle(16 + i.type % 2 * 8, textureOffset + (112 + i.type / 2 * 8), 8, 8), Color.White, i.rotation, Vector2.Zero, 4f, SpriteEffects.None, (float)__instance.getBoundingBox().Bottom / 10000f + 0.01f);
                }
                return false;
            }
            return true;
        }

        private static void SeasonUpdatePostfix(Tree __instance, bool onLoad)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME) && __instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_SEASON) && !String.IsNullOrEmpty(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]))
            {
                __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON] = Game1.GetSeasonForLocation(__instance.Location).ToString();
                __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME] = String.Concat(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_OWNER], ".", $"{AlternativeTextureModel.TextureType.Tree}_{GetTreeTypeString(__instance)}_{__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]}");
            }
        }

        private static void TreePostfix(Tree __instance)
        {
            var instanceName = $"{AlternativeTextureModel.TextureType.Tree}_{GetTreeTypeString(__instance)}";
            var instanceSeasonName = $"{instanceName}_{Game1.GetSeasonForLocation(__instance.Location)}";

            if (AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceName) && AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceSeasonName))
            {
                var result = Game1.random.Next(2) > 0 ? AssignModData(__instance, instanceSeasonName, true) : AssignModData(__instance, instanceName, false);
                return;
            }
            else
            {
                if (AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceName))
                {
                    AssignModData(__instance, instanceName, false);
                    return;
                }

                if (AlternativeTextures.textureManager.DoesObjectHaveAlternativeTexture(instanceSeasonName))
                {
                    AssignModData(__instance, instanceSeasonName, true);
                    return;
                }
            }

            AssignDefaultModData(__instance, instanceSeasonName, true);
        }
    }
}
