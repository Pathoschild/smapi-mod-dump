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
    internal class GiantCropPatch : PatchTemplate
    {
        private readonly Type _object = typeof(GiantCrop);

        internal GiantCropPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(GiantCrop.draw), new[] { typeof(SpriteBatch) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
            harmony.Patch(AccessTools.Constructor(typeof(GiantCrop), new[] { typeof(string), typeof(Vector2) }), postfix: new HarmonyMethod(GetType(), nameof(GiantCropPostfix)));

            if (PatchTemplate.IsDGAUsed())
            {
                try
                {
                    if (Type.GetType("DynamicGameAssets.Game.CustomGiantCrop, DynamicGameAssets") is Type dgaGiantCropType && dgaGiantCropType != null)
                    {
                        harmony.Patch(AccessTools.Method(dgaGiantCropType, nameof(GiantCrop.draw), new[] { typeof(SpriteBatch), typeof(Vector2) }), prefix: new HarmonyMethod(GetType(), nameof(DrawPrefix)));
                    }
                }
                catch (Exception ex)
                {
                    _monitor.Log($"Failed to patch Dynamic Game Assets in {this.GetType().Name}: AT may not be able to override certain DGA object types!", LogLevel.Warn);
                    _monitor.Log($"Patch for DGA failed in {this.GetType().Name}: {ex}", LogLevel.Trace);
                }
            }
        }

        [HarmonyBefore(new string[] { "spacechase0.JsonAssets", "spacechase0.MoreGiantCrops" })]
        private static bool DrawPrefix(GiantCrop __instance, float ___shakeTimer, SpriteBatch spriteBatch)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME))
            {
                Vector2 tileLocation = __instance.Tile;

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
                spriteBatch.Draw(textureModel.GetTexture(textureVariation), Game1.GlobalToLocal(Game1.viewport, tileLocation * 64f - new Vector2((___shakeTimer > 0f) ? ((float)Math.Sin(Math.PI * 2.0 / (double)___shakeTimer) * 2f) : 0f, 64f)), new Rectangle(0, textureOffset, 48, 63), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (tileLocation.Y + 2f) * 64f / 10000f);

                return false;
            }

            return true;
        }

        private static void GiantCropPostfix(GiantCrop __instance)
        {
            var instanceName = Game1.objectData.ContainsKey(__instance.Id) ? Game1.objectData[__instance.Id].Name : String.Empty;
            instanceName = $"{AlternativeTextureModel.TextureType.GiantCrop}_{instanceName}";
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
