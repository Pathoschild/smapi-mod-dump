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
using StardewValley.Characters;
using System;

namespace AlternativeTextures.Framework.Patches.Entities
{
    internal class PetPatch : PatchTemplate
    {
        private readonly Type _entity = typeof(Pet);

        internal PetPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_entity, nameof(Pet.draw), new[] { typeof(SpriteBatch) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            harmony.Patch(AccessTools.Method(_entity, nameof(Pet.update), new[] { typeof(GameTime), typeof(GameLocation) }), postfix: new HarmonyMethod(GetType(), nameof(UpdatePostfix)));

            harmony.Patch(AccessTools.Constructor(typeof(Cat), new[] { typeof(int), typeof(int), typeof(string), typeof(string) }), postfix: new HarmonyMethod(GetType(), nameof(PetPostfix)));
            harmony.Patch(AccessTools.Constructor(typeof(Dog), new[] { typeof(int), typeof(int), typeof(string), typeof(string) }), postfix: new HarmonyMethod(GetType(), nameof(PetPostfix)));
        }

        private static void ReloadBreedSpritePostfix(Pet __instance)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME) && __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_OWNER] != AlternativeTextures.DEFAULT_OWNER)
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME]);
                if (textureModel is null)
                {
                    __instance.Sprite.LoadTexture(__instance.getPetTextureName());
                    return;
                }

                var textureVariation = Int32.Parse(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_VARIATION]);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    __instance.Sprite.LoadTexture(__instance.getPetTextureName());
                    return;
                }
                var textureOffset = textureModel.GetTextureOffset(textureVariation);

                __instance.Sprite.spriteTexture = textureModel.GetTexture(textureVariation);
                __instance.Sprite.sourceRect.Y = textureOffset + (__instance.Sprite.currentFrame * __instance.Sprite.SpriteWidth / __instance.Sprite.Texture.Width * __instance.Sprite.SpriteHeight);
            }

            return;
        }

        private static bool DrawPrefix(Pet __instance, int ___shakeTimer, SpriteBatch b)
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

                __instance.Sprite.spriteTexture = textureModel.GetTexture(textureVariation);
                __instance.Sprite.sourceRect.Y = textureOffset + (__instance.Sprite.currentFrame * __instance.Sprite.SpriteWidth / __instance.Sprite.Texture.Width * __instance.Sprite.SpriteHeight);

                b.Draw(__instance.Sprite.Texture, __instance.getLocalPosition(Game1.viewport) + new Vector2(__instance.Sprite.SpriteWidth * 4 / 2, __instance.GetBoundingBox().Height / 2) + ((___shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero), __instance.Sprite.SourceRect, Color.White, __instance.rotation, new Vector2(__instance.Sprite.SpriteWidth / 2, (float)__instance.Sprite.SpriteHeight * 3f / 4f), Math.Max(0.2f, __instance.Scale) * 4f, (__instance.flip || (__instance.Sprite.CurrentAnimation != null && __instance.Sprite.CurrentAnimation[__instance.Sprite.currentAnimationIndex].flip)) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, __instance.isSleepingOnFarmerBed.Value ? (((float)__instance.StandingPixel.Y + 112f) / 10000f) : ((float)__instance.StandingPixel.Y / 10000f)));
                if (__instance.IsEmoting)
                {
                    Vector2 emotePosition = __instance.getLocalPosition(Game1.viewport);
                    emotePosition.X += 32f;
                    emotePosition.Y -= 96 + ((__instance is Dog) ? 16 : 0);
                    b.Draw(Game1.emoteSpriteSheet, emotePosition, new Rectangle(__instance.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, __instance.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)__instance.StandingPixel.Y / 10000f + 0.0001f);
                }
            }

            return true;
        }

        private static void UpdatePostfix(Pet __instance, GameTime time, GameLocation location)
        {
            if (!__instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME))
            {
                return;
            }

            var instanceName = String.Concat(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_OWNER], ".", $"{AlternativeTextureModel.TextureType.Character}_{GetCharacterName(__instance)}").ToLower();
            var instanceSeasonName = $"{instanceName}_{Game1.GetSeasonForLocation(__instance.currentLocation)}".ToLower();
            if (__instance is Pet pet && !String.Equals(pet.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME], instanceName, StringComparison.OrdinalIgnoreCase) && !String.Equals(pet.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME], instanceSeasonName, StringComparison.OrdinalIgnoreCase))
            {
                pet.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME] = String.Concat(pet.modData[ModDataKeys.ALTERNATIVE_TEXTURE_OWNER], ".", $"{AlternativeTextureModel.TextureType.Character}_{GetCharacterName(pet)}");
                if (pet.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_SEASON) && !String.IsNullOrEmpty(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]))
                {
                    pet.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON] = Game1.GetSeasonForLocation(location).ToString();
                    pet.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME] = String.Concat(pet.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME], "_", pet.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]);
                }
            }

            ReloadBreedSpritePostfix(__instance);
        }

        private static void PetPostfix(Pet __instance, int xTile, int yTile, string petBreed, string petType)
        {
            var instanceName = $"{AlternativeTextureModel.TextureType.Character}_{GetCharacterName(__instance)}";
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
    }
}
