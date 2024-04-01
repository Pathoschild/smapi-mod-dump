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

namespace AlternativeTextures.Framework.Patches.StandardObjects
{
    internal class BushPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Bush);

        internal BushPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Bush.draw), new[] { typeof(SpriteBatch) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Bush.seasonUpdate), new[] { typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(SeasonUpdatePostfix)));
            harmony.Patch(AccessTools.Constructor(typeof(Bush), new[] { typeof(Vector2), typeof(int), typeof(GameLocation), typeof(int) }), postfix: new HarmonyMethod(GetType(), nameof(BushPostfix)));
        }

        private static bool DrawPrefix(Bush __instance, float ___yDrawOffset, float ___shakeRotation, NetRectangle ___sourceRect, SpriteBatch spriteBatch)
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
                Vector2 tileLocation = __instance.Tile;

                var effectiveSize = getEffectiveSize(__instance.size.Value);
                if (__instance.drawShadow.Value)
                {
                    if (effectiveSize > 0)
                    {
                        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((tileLocation.X + ((effectiveSize == 1) ? 0.5f : 1f)) * 64f - 51f, tileLocation.Y * 64f - 16f + ___yDrawOffset)), Bush.shadowSourceRect, Color.White, 0f, Vector2.Zero, 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1E-06f);
                    }
                    else
                    {
                        spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 32f, tileLocation.Y * 64f + 64f - 4f + ___yDrawOffset)), Game1.shadowTexture.Bounds, Color.White, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, 1E-06f);
                    }
                }

                var textureOffset = textureModel.GetTextureOffset(textureVariation);
                var sourceRect = new Rectangle(__instance.tileSheetOffset.Value == 1 && __instance.inBloom() ? 32 : 0, textureOffset, ___sourceRect.Value.Width, ___sourceRect.Value.Height);
                if (__instance.size.Value == Bush.greenTeaBush)
                {
                    sourceRect = new Rectangle(Math.Min(2, __instance.getAge() / 10) * 16 + __instance.tileSheetOffset.Value * 16, textureOffset, 16, 32);
                }
                spriteBatch.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + (float)((effectiveSize + 1) * 64 / 2), (tileLocation.Y + 1f) * 64f - (float)((effectiveSize > 0 && (!__instance.townBush || effectiveSize != 1) && (int)__instance.size != 4) ? 64 : 0) + ___yDrawOffset)), sourceRect, Color.White, ___shakeRotation, new Vector2((effectiveSize + 1) * 16 / 2, 32f), 4f, __instance.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float)(__instance.getBoundingBox().Center.Y + 48) / 10000f - tileLocation.X / 1000000f);

                return false;
            }
            return true;
        }

        private static int getEffectiveSize(int size)
        {
            if (size == 3)
            {
                return 0;
            }
            if (size == 4)
            {
                return 1;
            }

            return size;
        }


        private static void SeasonUpdatePostfix(Bush __instance, bool onLoad)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME) && __instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_SEASON) && !String.IsNullOrEmpty(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]))
            {
                __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON] = Game1.GetSeasonForLocation(__instance.Location).ToString();
                __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME] = String.Concat(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_OWNER], ".", $"{AlternativeTextureModel.TextureType.Bush}_{GetBushTypeString(__instance)}_{__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]}");
            }
        }

        private static void BushPostfix(Bush __instance)
        {
            var instanceName = $"{AlternativeTextureModel.TextureType.Bush}_{GetBushTypeString(__instance)}";
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
