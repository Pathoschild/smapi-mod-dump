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
using StardewValley.TerrainFeatures;
using System;

namespace AlternativeTextures.Framework.Patches.StandardObjects
{
    internal class ResourceClumpPatch : PatchTemplate
    {
        private readonly Type _object = typeof(ResourceClump);

        internal ResourceClumpPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(ResourceClump.draw), new[] { typeof(SpriteBatch) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            harmony.Patch(AccessTools.Method(typeof(TerrainFeature), nameof(TerrainFeature.seasonUpdate), new[] { typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(SeasonUpdatePostfix)));
            harmony.Patch(AccessTools.Constructor(typeof(ResourceClump), new[] { typeof(int), typeof(int), typeof(int), typeof(Vector2), typeof(int), typeof(string) }), postfix: new HarmonyMethod(GetType(), nameof(ResourceClumpPostfix)));
        }

        private static bool DrawPrefix(ResourceClump __instance, float ___shakeTimer, SpriteBatch spriteBatch)
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

                Vector2 position = __instance.Tile * 64f;
                if (___shakeTimer > 0f)
                {
                    position.X += (float)Math.Sin(Math.PI * 2.0 / (double)___shakeTimer) * 4f;
                }

                var textureOffset = textureModel.GetTextureOffset(textureVariation);
                Rectangle sourceRect = new Rectangle(0, textureOffset, 32, 32);

                spriteBatch.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, position), sourceRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (__instance.Tile.Y + 1f) * 64f / 10000f + __instance.Tile.X / 100000f);

                return false;
            }

            return true;
        }

        private static void SeasonUpdatePostfix(TerrainFeature __instance, bool onLoad)
        {
            if (__instance is ResourceClump resourceClump)
            {
                if (__instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME) && __instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_SEASON) && !String.IsNullOrEmpty(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]))
                {
                    __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON] = Game1.GetSeasonForLocation(__instance.Location).ToString();
                    __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME] = String.Concat(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_OWNER], ".", $"{AlternativeTextureModel.TextureType.ResourceClump}_{GetResourceClumpName(resourceClump)}_{__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]}");
                }
            }
        }

        private static void ResourceClumpPostfix(ResourceClump __instance)
        {
            var instanceName = $"{AlternativeTextureModel.TextureType.ResourceClump}_{GetResourceClumpName(__instance)}";
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

        private static string GetResourceClumpName(ResourceClump clump)
        {
            switch (clump.parentSheetIndex)
            {
                case 600:
                    return "Stump";
                case 602:
                    return "Log";
                case 622:
                    return "Meteor";
                case 672:
                    return "Boulder";
                default:
                    return String.Empty;
            }
        }
    }
}
