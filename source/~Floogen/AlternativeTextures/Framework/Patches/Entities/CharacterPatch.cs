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
    internal class CharacterPatch : PatchTemplate
    {
        private readonly Type _entity = typeof(Character);

        internal const string BABY_NAME_PREFIX = "Baby";
        internal const string TODDLER_NAME_PREFIX = "Toddler";

        internal CharacterPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_entity, nameof(Character.update), new[] { typeof(GameTime), typeof(GameLocation) }), postfix: new HarmonyMethod(GetType(), nameof(UpdatePostfix)));
            harmony.Patch(AccessTools.Method(_entity, nameof(Character.draw), new[] { typeof(SpriteBatch) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));

            harmony.CreateReversePatcher(AccessTools.Method(_entity, nameof(Character.draw), new[] { typeof(SpriteBatch) }), new HarmonyMethod(GetType(), nameof(DrawReversePatch))).Patch();
        }

        private static void UpdatePostfix(Character __instance, GameTime time, GameLocation location)
        {
            if (!__instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME) || AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME]) is null)
            {
                return;
            }

            var instanceName = String.Concat(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_OWNER], ".", $"{AlternativeTextureModel.TextureType.Character}_{GetCharacterName(__instance)}").ToLower();
            var instanceSeasonName = $"{instanceName}_{Game1.GetSeasonForLocation(__instance.currentLocation)}".ToLower();
            if (__instance is Child child && !String.Equals(child.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME], instanceName, StringComparison.OrdinalIgnoreCase) && !String.Equals(child.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME], instanceSeasonName, StringComparison.OrdinalIgnoreCase))
            {
                child.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME] = String.Concat(child.modData[ModDataKeys.ALTERNATIVE_TEXTURE_OWNER], ".", $"{AlternativeTextureModel.TextureType.Character}_{GetCharacterName(child)}");
                if (child.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_SEASON) && !String.IsNullOrEmpty(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]))
                {
                    child.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON] = Game1.GetSeasonForLocation(location);
                    child.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME] = String.Concat(child.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME], "_", child.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]);
                }

                __instance.Sprite.loadedTexture = String.Empty;
            }
            if (__instance is Horse horse && !String.Equals(horse.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME], instanceName, StringComparison.OrdinalIgnoreCase) && !String.Equals(horse.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME], instanceSeasonName, StringComparison.OrdinalIgnoreCase))
            {
                horse.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME] = String.Concat(horse.modData[ModDataKeys.ALTERNATIVE_TEXTURE_OWNER], ".", $"{AlternativeTextureModel.TextureType.Character}_{GetCharacterName(horse)}");
                if (horse.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_SEASON) && !String.IsNullOrEmpty(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]))
                {
                    horse.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON] = Game1.GetSeasonForLocation(location);
                    horse.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME] = String.Concat(horse.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME], "_", horse.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]);
                }

                __instance.Sprite.loadedTexture = String.Empty;
            }
        }

        private static bool DrawPrefix(Character __instance, SpriteBatch b)
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
            }

            return true;
        }

        public static void DrawReversePatch(Character __instance, SpriteBatch b)
        {
            new NotImplementedException("It's a stub!");
        }
    }
}
