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
using AlternativeTextures.Framework.UI;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

namespace AlternativeTextures.Framework.Patches.Tools
{
    internal class ToolPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Tool);

        internal ToolPatch(IMonitor modMonitor) : base(modMonitor)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Tool.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(DrawInMenuPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Tool.beginUsing), new[] { typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(BeginUsingPrefix)));
        }

        private static bool DrawInMenuPrefix(Tool __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (__instance.modData.ContainsKey(AlternativeTextures.PAINT_BUCKET_FLAG))
            {
                spriteBatch.Draw(AlternativeTextures.assetManager.GetPaintBucketTexture(), location + new Vector2(32f, 32f), new Rectangle(0, 0, 16, 16), color * transparency, 0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);

                return false;
            }

            if (__instance.modData.ContainsKey(AlternativeTextures.PAINT_BRUSH_FLAG))
            {
                var scale = __instance.modData.ContainsKey(AlternativeTextures.PAINT_BRUSH_SCALE) ? float.Parse(__instance.modData[AlternativeTextures.PAINT_BRUSH_SCALE]) : 0f;
                var texture = AlternativeTextures.assetManager.GetPaintBrushEmptyTexture();
                if (!String.IsNullOrEmpty(__instance.modData[AlternativeTextures.PAINT_BRUSH_FLAG]))
                {
                    texture = AlternativeTextures.assetManager.GetPaintBrushFilledTexture();
                }
                spriteBatch.Draw(texture, location + new Vector2(32f, 32f), new Rectangle(0, 0, 16, 16), color * transparency, 0f, new Vector2(8f, 8f), 4f * (scaleSize + scale), SpriteEffects.None, layerDepth);

                if (scale > 0f)
                {
                    __instance.modData[AlternativeTextures.PAINT_BRUSH_SCALE] = (scale -= 0.01f).ToString();
                }
                return false;
            }

            return true;
        }

        private static bool BeginUsingPrefix(Tool __instance, ref bool __result, GameLocation location, int x, int y, Farmer who)
        {
            if (__instance.modData.ContainsKey(AlternativeTextures.PAINT_BUCKET_FLAG))
            {
                __result = true;
                return UsePaintBucket(location, x, y, who);
            }

            if (__instance.modData.ContainsKey(AlternativeTextures.PAINT_BRUSH_FLAG))
            {
                __result = true;
                return CancelUsing(who);
            }

            return true;
        }

        private static bool UsePaintBucket(GameLocation location, int x, int y, Farmer who)
        {
            var targetedObject = location.getObjectAt(x, y);
            if (targetedObject != null)
            {
                // Assign default data if none exists
                if (!targetedObject.modData.ContainsKey("AlternativeTextureName"))
                {
                    var modelType = targetedObject is Furniture ? AlternativeTextureModel.TextureType.Furniture : AlternativeTextureModel.TextureType.Craftable;
                    var instanceSeasonName = $"{modelType}_{GetObjectName(targetedObject)}_{Game1.currentSeason}";
                    AssignDefaultModData(targetedObject, instanceSeasonName, true);
                }

                var modelName = targetedObject.modData["AlternativeTextureName"].Replace($"{targetedObject.modData["AlternativeTextureOwner"]}.", String.Empty);
                if (targetedObject.modData.ContainsKey("AlternativeTextureSeason") && !String.IsNullOrEmpty(targetedObject.modData["AlternativeTextureSeason"]))
                {
                    modelName = modelName.Replace($"_{targetedObject.modData["AlternativeTextureSeason"]}", String.Empty);
                }

                if (AlternativeTextures.textureManager.GetAvailableTextureModels(modelName, Game1.GetSeasonForLocation(Game1.currentLocation)).Count == 0)
                {
                    Game1.addHUDMessage(new HUDMessage($"{modelName} has no alternative textures for this season!", 3));
                    return CancelUsing(who);
                }

                // Display texture menu
                Game1.activeClickableMenu = new PaintBucketMenu(targetedObject, modelName);

                return CancelUsing(who);
            }

            var targetedTerrain = GetTerrainFeatureAt(location, x, y);
            if (targetedTerrain != null)
            {
                if (targetedTerrain is HoeDirt || targetedTerrain is GiantCrop || targetedTerrain is Tree || targetedTerrain is FruitTree || targetedTerrain is Grass)
                {
                    Game1.addHUDMessage(new HUDMessage($"You can't put paint on that!", 3));
                    return CancelUsing(who);
                }

                if (!targetedTerrain.modData.ContainsKey("AlternativeTextureName"))
                {
                    if (targetedTerrain is Flooring flooring)
                    {
                        if (GetFloorSheetId(flooring) == -1)
                        {
                            return CancelUsing(who);
                        }

                        var instanceSeasonName = $"{AlternativeTextureModel.TextureType.Flooring}_{GetFlooringName(flooring)}_{Game1.GetSeasonForLocation(Game1.currentLocation)}";
                        targetedTerrain.modData["AlternativeTextureSheetId"] = GetFloorSheetId(flooring).ToString();
                        AssignDefaultModData(targetedTerrain, instanceSeasonName, true);
                    }
                    else
                    {
                        return CancelUsing(who);
                    }
                }

                var modelName = targetedTerrain.modData["AlternativeTextureName"].Replace($"{targetedTerrain.modData["AlternativeTextureOwner"]}.", String.Empty);
                if (targetedTerrain.modData.ContainsKey("AlternativeTextureSeason") && !String.IsNullOrEmpty(targetedTerrain.modData["AlternativeTextureSeason"]))
                {
                    modelName = modelName.Replace($"_{targetedTerrain.modData["AlternativeTextureSeason"]}", String.Empty);
                }

                if (AlternativeTextures.textureManager.GetAvailableTextureModels(modelName, Game1.GetSeasonForLocation(Game1.currentLocation)).Count == 0 || !targetedTerrain.modData.ContainsKey("AlternativeTextureSheetId"))
                {
                    Game1.addHUDMessage(new HUDMessage($"{modelName} has no alternative textures for this season!", 3));
                    return CancelUsing(who);
                }

                // Display texture menu
                var terrainObj = new Object(targetedTerrain.currentTileLocation, Int32.Parse(targetedTerrain.modData["AlternativeTextureSheetId"]), 1);
                if (terrainObj != null)
                {
                    foreach (string key in targetedTerrain.modData.Keys)
                    {
                        terrainObj.modData[key] = targetedTerrain.modData[key];
                    }

                    Game1.activeClickableMenu = new PaintBucketMenu(terrainObj, modelName, true);
                }

                return CancelUsing(who);
            }

            return CancelUsing(who);
        }

        private static bool CancelUsing(Farmer who)
        {
            who.CanMove = true;
            who.UsingTool = false;
            return false;
        }
    }
}
