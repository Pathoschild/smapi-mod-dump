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
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches.Entities
{
    internal class FarmAnimalPatch : PatchTemplate
    {
        private readonly Type _entity = typeof(FarmAnimal);

        internal FarmAnimalPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_entity, nameof(FarmAnimal.draw), new[] { typeof(SpriteBatch) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            harmony.Patch(AccessTools.Method(_entity, nameof(FarmAnimal.updateWhenCurrentLocation), new[] { typeof(GameTime), typeof(GameLocation) }), postfix: new HarmonyMethod(GetType(), nameof(UpdateWhenCurrentLocationPostfix)));
            harmony.Patch(AccessTools.Constructor(_entity, new[] { typeof(string), typeof(long), typeof(long) }), postfix: new HarmonyMethod(GetType(), nameof(FarmAnimalPostfix)));
        }

        private static bool DrawPrefix(Character __instance, SpriteBatch b)
        {
            if (__instance.modData.ContainsKey("AlternativeTextureName"))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]);
                if (textureModel is null)
                {
                    __instance.Sprite.loadedTexture = String.Empty;
                    return true;
                }

                var textureVariation = Int32.Parse(__instance.modData["AlternativeTextureVariation"]);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    __instance.Sprite.loadedTexture = String.Empty;
                    return true;
                }
                var textureOffset = textureModel.GetTextureOffset(textureVariation);

                __instance.Sprite.spriteTexture = textureModel.GetTexture(textureVariation);
                __instance.Sprite.sourceRect.Y = textureOffset + (__instance.Sprite.currentFrame * __instance.Sprite.SpriteWidth / __instance.Sprite.Texture.Width * __instance.Sprite.SpriteHeight);
            }

            return true;
        }

        private static void UpdateWhenCurrentLocationPostfix(FarmAnimal __instance, GameTime time, GameLocation location)
        {
            if (!__instance.modData.ContainsKey("AlternativeTextureName"))
            {
                return;
            }

            var instanceName = String.Concat(__instance.modData["AlternativeTextureOwner"], ".", $"{AlternativeTextureModel.TextureType.Character}_{GetCharacterName(__instance)}").ToLower();
            var instanceSeasonName = $"{instanceName}_{Game1.GetSeasonForLocation(__instance.currentLocation)}".ToLower();
            if (!String.Equals(__instance.modData["AlternativeTextureName"], instanceName, StringComparison.OrdinalIgnoreCase) && !String.Equals(__instance.modData["AlternativeTextureName"], instanceSeasonName, StringComparison.OrdinalIgnoreCase))
            {
                __instance.modData["AlternativeTextureName"] = String.Concat(__instance.modData["AlternativeTextureOwner"], ".", $"{AlternativeTextureModel.TextureType.Character}_{GetCharacterName(__instance)}");
                if (__instance.modData.ContainsKey("AlternativeTextureSeason") && !String.IsNullOrEmpty(__instance.modData["AlternativeTextureSeason"]))
                {
                    __instance.modData["AlternativeTextureSeason"] = Game1.GetSeasonForLocation(location);
                    __instance.modData["AlternativeTextureName"] = String.Concat(__instance.modData["AlternativeTextureName"], "_", __instance.modData["AlternativeTextureSeason"]);
                }
            }
        }

        private static void FarmAnimalPostfix(FarmAnimal __instance, string type, long id, long ownerID)
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
