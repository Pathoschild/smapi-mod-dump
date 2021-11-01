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
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

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
            if (!__instance.modData.ContainsKey("AlternativeTextureName") || AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]) is null)
            {
                return;
            }

            var instanceName = String.Concat(__instance.modData["AlternativeTextureOwner"], ".", $"{AlternativeTextureModel.TextureType.Character}_{GetCharacterName(__instance)}").ToLower();
            var instanceSeasonName = $"{instanceName}_{Game1.GetSeasonForLocation(__instance.currentLocation)}".ToLower();
            if (__instance is Child child && !String.Equals(child.modData["AlternativeTextureName"], instanceName, StringComparison.OrdinalIgnoreCase) && !String.Equals(child.modData["AlternativeTextureName"], instanceSeasonName, StringComparison.OrdinalIgnoreCase))
            {
                child.modData["AlternativeTextureName"] = String.Concat(child.modData["AlternativeTextureOwner"], ".", $"{AlternativeTextureModel.TextureType.Character}_{GetCharacterName(child)}");
                if (child.modData.ContainsKey("AlternativeTextureSeason") && !String.IsNullOrEmpty(__instance.modData["AlternativeTextureSeason"]))
                {
                    child.modData["AlternativeTextureSeason"] = Game1.GetSeasonForLocation(location);
                    child.modData["AlternativeTextureName"] = String.Concat(child.modData["AlternativeTextureName"], "_", child.modData["AlternativeTextureSeason"]);
                }

                __instance.Sprite.loadedTexture = String.Empty;
            }
            if (__instance is Horse horse && !String.Equals(horse.modData["AlternativeTextureName"], instanceName, StringComparison.OrdinalIgnoreCase) && !String.Equals(horse.modData["AlternativeTextureName"], instanceSeasonName, StringComparison.OrdinalIgnoreCase))
            {
                horse.modData["AlternativeTextureName"] = String.Concat(horse.modData["AlternativeTextureOwner"], ".", $"{AlternativeTextureModel.TextureType.Character}_{GetCharacterName(horse)}");
                if (horse.modData.ContainsKey("AlternativeTextureSeason") && !String.IsNullOrEmpty(__instance.modData["AlternativeTextureSeason"]))
                {
                    horse.modData["AlternativeTextureSeason"] = Game1.GetSeasonForLocation(location);
                    horse.modData["AlternativeTextureName"] = String.Concat(horse.modData["AlternativeTextureName"], "_", horse.modData["AlternativeTextureSeason"]);
                }

                __instance.Sprite.loadedTexture = String.Empty;
            }
        }

        private static bool DrawPrefix(Character __instance, SpriteBatch b)
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
