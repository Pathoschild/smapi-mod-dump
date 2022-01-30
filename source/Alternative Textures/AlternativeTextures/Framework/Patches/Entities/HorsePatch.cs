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
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches.Entities
{
    internal class HorsePatch : PatchTemplate
    {
        private readonly Type _entity = typeof(Horse);

        internal HorsePatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_entity, nameof(Horse.draw), new[] { typeof(SpriteBatch) }), postfix: new HarmonyMethod(GetType(), nameof(DrawPostfix)));

            harmony.Patch(AccessTools.Method(_entity, nameof(Horse.draw), new[] { typeof(SpriteBatch) }), transpiler: new HarmonyMethod(typeof(HorsePatch), nameof(AdjustForVariationTranspiler)));
            harmony.Patch(AccessTools.Constructor(_entity, new[] { typeof(Guid), typeof(int), typeof(int) }), postfix: new HarmonyMethod(GetType(), nameof(HorsePostfix)));
        }


        private static IEnumerable<CodeInstruction> AdjustForVariationTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var list = instructions.ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].opcode == OpCodes.Callvirt && list[i].operand is not null && list[i].operand.ToString().Contains("updatesourcerect", StringComparison.OrdinalIgnoreCase))
                    {
                        list.Insert(i + 1, new CodeInstruction(OpCodes.Ldarg_0));
                        list.Insert(i + 2, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HorsePatch), nameof(HandleVariations), new[] { typeof(Horse) })));
                    }

                    if (list[i].opcode == OpCodes.Ldc_I4_S && (sbyte)list[i].operand == 96)
                    {
                        list.Insert(i, new CodeInstruction(OpCodes.Ldarg_0));
                        list[i + 1] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HorsePatch), nameof(GetHeadTextureYOffset), new[] { typeof(Horse) }));
                    }
                }

                return list;
            }
            catch (Exception e)
            {
                _monitor.Log($"There was an issue modifying the instructions for Horse.draw: {e}", LogLevel.Error);
                return instructions;
            }
        }

        private static int GetHeadTextureYOffset(Horse horse)
        {
            int yOffset = 96;
            if (!horse.modData.ContainsKey("AlternativeTextureName"))
            {
                return yOffset;
            }

            var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(horse.modData["AlternativeTextureName"]);
            if (textureModel is null)
            {
                return yOffset;
            }

            var textureVariation = Int32.Parse(horse.modData["AlternativeTextureVariation"]);
            if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
            {
                return yOffset;
            }

            return yOffset + textureModel.GetTextureOffset(textureVariation);
        }

        private static void HandleVariations(Horse horse)
        {
            if (!horse.modData.ContainsKey("AlternativeTextureName"))
            {
                return;
            }

            var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(horse.modData["AlternativeTextureName"]);
            if (textureModel is null)
            {
                return;
            }

            var textureVariation = Int32.Parse(horse.modData["AlternativeTextureVariation"]);
            if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
            {
                return;
            }

            var textureOffset = textureModel.GetTextureOffset(textureVariation);
            horse.Sprite.spriteTexture = textureModel.GetTexture(textureVariation);
            horse.Sprite.sourceRect.Y = textureOffset + (horse.Sprite.currentFrame * horse.Sprite.SpriteWidth / horse.Sprite.Texture.Width * horse.Sprite.SpriteHeight);
        }

        [HarmonyBefore(new string[] { "Goldenrevolver.HorseOverhaul" })]
        private static void DrawPostfix(Horse __instance, SpriteBatch b)
        {
            if (__instance.modData.ContainsKey("AlternativeTextureName"))
            {
                var textureModel = AlternativeTextures.textureManager.GetSpecificTextureModel(__instance.modData["AlternativeTextureName"]);
                if (textureModel is null)
                {
                    return;
                }

                var textureVariation = Int32.Parse(__instance.modData["AlternativeTextureVariation"]);
                if (textureVariation == -1 || AlternativeTextures.modConfig.IsTextureVariationDisabled(textureModel.GetId(), textureVariation))
                {
                    return;
                }

                __instance.Sprite.UpdateSourceRect();
            }
        }

        private static void HorsePostfix(Horse __instance, Guid horseId, int xTile, int yTile)
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
