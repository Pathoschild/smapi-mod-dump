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
using StardewValley.Monsters;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches.Entities
{
    internal class MonsterPatch : PatchTemplate
    {
        private readonly Type _entity = typeof(Monster);

        internal MonsterPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_entity, nameof(Monster.draw), new[] { typeof(SpriteBatch) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            harmony.Patch(AccessTools.Method(_entity, nameof(Monster.update), new[] { typeof(GameTime), typeof(GameLocation) }), postfix: new HarmonyMethod(GetType(), nameof(UpdatePostfix)));
            harmony.Patch(AccessTools.Method(_entity, nameof(Monster.reloadSprite), null), postfix: new HarmonyMethod(GetType(), nameof(ReloadSpritePostfix)));
            harmony.Patch(AccessTools.Constructor(_entity, new[] { typeof(string), typeof(Vector2), typeof(int) }), postfix: new HarmonyMethod(GetType(), nameof(MonsterPostfix)));
        }

        private static void SetTexture(Monster monster, AlternativeTextureModel textureModel)
        {
            if (textureModel is null)
            {
                monster.Sprite.loadedTexture = String.Empty;
                return;
            }

            var textureVariation = Int32.Parse(monster.modData["AlternativeTextureVariation"]);
            if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
            {
                monster.Sprite.loadedTexture = String.Empty;
                return;
            }
            var textureOffset = textureModel.GetTextureOffset(textureVariation);

            monster.Sprite.spriteTexture = textureModel.GetTexture(textureVariation);
            monster.Sprite.sourceRect.Y = textureOffset + (monster.Sprite.currentFrame * monster.Sprite.SpriteWidth / monster.Sprite.Texture.Width * monster.Sprite.SpriteHeight);
        }

        private static bool DrawPrefix(Monster __instance, SpriteBatch b)
        {
            if (__instance.modData.ContainsKey("AlternativeTextureName"))
            {
                SetTexture(__instance, AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]));
            }

            return true;
        }

        private static void UpdatePostfix(Monster __instance, GameTime time, GameLocation location)
        {
            if (!__instance.modData.ContainsKey("AlternativeTextureName"))
            {
                return;
            }

            if (__instance.Sprite.textureName.Value.IndexOf("_dangerous", StringComparison.OrdinalIgnoreCase) >= 0 && __instance.modData["AlternativeTextureName"].IndexOf("_dangerous", StringComparison.OrdinalIgnoreCase) == -1)
            {
                var instanceName = $"{AlternativeTextureModel.TextureType.Character}_{GetCharacterName(__instance)}_dangerous";
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

            var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]);
            if (textureModel is null)
            {
                __instance.Sprite.loadedTexture = String.Empty;
                return;
            }

            var textureVariation = Int32.Parse(__instance.modData["AlternativeTextureVariation"]);
            if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
            {
                __instance.Sprite.loadedTexture = String.Empty;
                return;
            }

            if (__instance.Sprite.textureName.Value != textureModel.GetTexture(textureVariation).Name)
            {
                SetTexture(__instance, textureModel);
            }
        }

        private static void ReloadSpritePostfix(Monster __instance)
        {
            if (__instance.modData.ContainsKey("AlternativeTextureName"))
            {
                SetTexture(__instance, AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]));
            }
        }

        private static void MonsterPostfix(Monster __instance, string name, Vector2 position, int facingDir)
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
