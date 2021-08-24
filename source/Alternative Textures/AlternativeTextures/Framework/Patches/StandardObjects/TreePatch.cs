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
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches.StandardObjects
{
    internal class TreePatch : PatchTemplate
    {
        private readonly Type _object = typeof(Tree);

        internal TreePatch(IMonitor modMonitor) : base(modMonitor)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Tree.draw), new[] { typeof(SpriteBatch), typeof(Vector2) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Tree.seasonUpdate), new[] { typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(SeasonUpdatePostfix)));
            harmony.Patch(AccessTools.Constructor(typeof(Tree), new[] { typeof(int) }), postfix: new HarmonyMethod(GetType(), nameof(TreePostfix)));
            harmony.Patch(AccessTools.Constructor(typeof(Tree), new[] { typeof(int), typeof(int) }), postfix: new HarmonyMethod(GetType(), nameof(TreePostfix)));
        }

        private static bool DrawPrefix(Tree __instance, float ___shakeRotation, float ___shakeTimer, float ___alpha, List<Leaf> ___leaves, NetBool ___falling, SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            if (__instance.modData.ContainsKey("AlternativeTextureName"))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]);
                if (textureModel is null)
                {
                    return true;
                }

                var textureVariation = Int32.Parse(__instance.modData["AlternativeTextureVariation"]);
                if (textureVariation == -1)
                {
                    return true;
                }

                if (__instance.isTemporarilyInvisible)
                {
                    return true;
                }

                var textureOffset = textureVariation * textureModel.TextureHeight;
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

                    spriteBatch.Draw(textureModel.Texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 32f, tileLocation.Y * 64f - (float)(sourceRect.Height * 4 - 64) + (float)(((int)__instance.growthStage >= 3) ? 128 : 64))), sourceRect, __instance.fertilized ? Color.HotPink : Color.White, ___shakeRotation, new Vector2(8f, ((int)__instance.growthStage >= 3) ? 32 : 16), 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, ((int)__instance.growthStage == 0) ? 0.0001f : ((float)__instance.getBoundingBox(tileLocation).Bottom / 10000f));
                }
                else
                {
                    if (!__instance.stump || (bool)___falling)
                    {
                        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f - 51f, tileLocation.Y * 64f - 16f)), Tree.shadowSourceRect, Color.White * ((float)Math.PI / 2f - Math.Abs(___shakeRotation)), 0f, Vector2.Zero, 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1E-06f);
                        Rectangle source_rect = __instance.treeTopSourceRect;
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
                        source_rect.Y += textureOffset;
                        spriteBatch.Draw(textureModel.Texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 32f, tileLocation.Y * 64f + 64f)), source_rect, Color.White * ___alpha, ___shakeRotation, new Vector2(24f, 96f), 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float)(__instance.getBoundingBox(tileLocation).Bottom + 2) / 10000f - tileLocation.X / 1000000f);
                    }
                    if ((float)__instance.health >= 1f || (!___falling && (float)__instance.health > -99f))
                    {
                        spriteBatch.Draw(textureModel.Texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + ((___shakeTimer > 0f) ? ((float)Math.Sin(Math.PI * 2.0 / (double)___shakeTimer) * 3f) : 0f), tileLocation.Y * 64f - 64f)), new Rectangle(Tree.stumpSourceRect.X, Tree.stumpSourceRect.Y + textureOffset, Tree.stumpSourceRect.Width, Tree.stumpSourceRect.Height), Color.White * ___alpha, 0f, Vector2.Zero, 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float)__instance.getBoundingBox(tileLocation).Bottom / 10000f);
                    }
                    if ((bool)__instance.stump && (float)__instance.health < 4f && (float)__instance.health > -99f)
                    {
                        spriteBatch.Draw(textureModel.Texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + ((___shakeTimer > 0f) ? ((float)Math.Sin(Math.PI * 2.0 / (double)___shakeTimer) * 3f) : 0f), tileLocation.Y * 64f)), new Rectangle(Math.Min(2, (int)(3f - (float)__instance.health)) * 16, 144 + textureOffset, 16, 16), Color.White * ___alpha, 0f, Vector2.Zero, 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float)(__instance.getBoundingBox(tileLocation).Bottom + 1) / 10000f);
                    }
                }
                foreach (Leaf i in ___leaves)
                {
                    spriteBatch.Draw(textureModel.Texture, Game1.GlobalToLocal(Game1.viewport, i.position), new Rectangle(16 + i.type % 2 * 8, textureOffset + (112 + i.type / 2 * 8), 8, 8), Color.White, i.rotation, Vector2.Zero, 4f, SpriteEffects.None, (float)__instance.getBoundingBox(tileLocation).Bottom / 10000f + 0.01f);
                }
                return false;
            }
            return true;
        }

        private static void SeasonUpdatePostfix(Tree __instance, bool onLoad)
        {
            if (__instance.modData.ContainsKey("AlternativeTextureName") && __instance.modData.ContainsKey("AlternativeTextureSeason") && !String.IsNullOrEmpty(__instance.modData["AlternativeTextureSeason"]))
            {
                __instance.modData["AlternativeTextureSeason"] = Game1.GetSeasonForLocation(__instance.currentLocation);
                __instance.modData["AlternativeTextureName"] = String.Concat(__instance.modData["AlternativeTextureOwner"], ".", $"{AlternativeTextureModel.TextureType.Tree}_{GetTreeTypeString(__instance)}_{__instance.modData["AlternativeTextureSeason"]}");
            }
        }

        private static void TreePostfix(Tree __instance)
        {
            var instanceName = $"{AlternativeTextureModel.TextureType.Tree}_{GetTreeTypeString(__instance)}";
            var instanceSeasonName = $"{instanceName}_{Game1.GetSeasonForLocation(__instance.currentLocation)}";

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

        private static string GetTreeTypeString(Tree tree)
        {
            switch (tree.treeType.Value)
            {
                case Tree.bushyTree:
                    return "Oak";
                case Tree.leafyTree:
                    return "Maple";
                case Tree.pineTree:
                    return "Pine";
                case Tree.mahoganyTree:
                    return "Mahogany";
                case Tree.mushroomTree:
                    return "Mushroom";
                case Tree.palmTree:
                    return "Palm_1";
                case Tree.palmTree2:
                    return "Palm_2";
                default:
                    return String.Empty;
            }
        }
    }
}
