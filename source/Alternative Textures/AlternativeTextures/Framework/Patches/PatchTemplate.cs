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
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches
{
    internal class PatchTemplate
    {
        internal static IMonitor _monitor;

        internal PatchTemplate(IMonitor modMonitor)
        {
            _monitor = modMonitor;
        }

        internal static GenericTool GetPaintBucketTool()
        {
            var paintBucket = new GenericTool("Paint Bucket", "Allows you to apply different textures to supported objects.", -1, 6, 6);
            paintBucket.modData[AlternativeTextures.PAINT_BUCKET_FLAG] = true.ToString();

            return paintBucket;
        }

        internal static GenericTool GetPaintBrushTool()
        {
            var paintBucket = new GenericTool("Paint Brush", "Allows you to copy a texture and apply it other objects of the same type.", -1, 6, 6);
            paintBucket.modData[AlternativeTextures.PAINT_BRUSH_FLAG] = null;

            return paintBucket;
        }

        internal static string GetObjectName(Object obj)
        {
            if (obj.bigCraftable)
            {
                if (!Game1.bigCraftablesInformation.ContainsKey(obj.parentSheetIndex))
                {
                    return obj.name;
                }

                return Game1.bigCraftablesInformation[obj.parentSheetIndex].Split('/')[0];
            }
            else if (obj is Furniture)
            {
                var dataSheet = Game1.content.LoadBase<Dictionary<int, string>>("Data\\Furniture");
                if (!dataSheet.ContainsKey(obj.parentSheetIndex))
                {
                    return obj.name;
                }

                return dataSheet[obj.parentSheetIndex].Split('/')[0];
            }
            else
            {
                if (obj is Fence fence && fence.isGate)
                {
                    return Game1.objectInformation[325].Split('/')[0];
                }
                if (!Game1.objectInformation.ContainsKey(obj.parentSheetIndex))
                {
                    return obj.name;
                }

                return Game1.objectInformation[obj.parentSheetIndex].Split('/')[0];
            }
        }

        internal static Object GetObjectAt(GameLocation location, int x, int y)
        {
            return location.getObjectAt(x, y);
        }

        internal static TerrainFeature GetTerrainFeatureAt(GameLocation location, int x, int y)
        {
            Vector2 tile = new Vector2(x / 64, y / 64);
            if (!location.terrainFeatures.ContainsKey(tile))
            {
                return null;
            }

            return location.terrainFeatures[tile];
        }

        internal static int GetFloorSheetId(Flooring floor)
        {
            var matchedFloor = Game1.objectInformation.Where(p => p.Value.Split('/')[0] == GetFlooringName(floor));
            return matchedFloor.Count() == 0 ? -1 : matchedFloor.First().Key;
        }

        internal static string GetFlooringName(Flooring floor)
        {
            switch (floor.whichFloor.Value)
            {
                case 0:
                    return "Wood Floor";
                case 1:
                    return "Stone Floor";
                case 2:
                    return "Weathered Floor";
                case 3:
                    return "Crystal Floor";
                case 4:
                    return "Straw Floor";
                case 5:
                    return "Gravel Path";
                case 6:
                    return "Wood Path";
                case 7:
                    return "Crystal Path";
                case 8:
                    return "Cobblestone Path";
                case 9:
                    return "Stepping Stone Path";
                case 10:
                    return "Straw Brick Floor";
                case 11:
                    return "Rustic Plank Floor";
                case 12:
                    return "Stone Walkway Floor";
                default:
                    return String.Empty;
            }
        }

        internal static string GetBushTypeString(Bush bush)
        {
            switch (bush.size)
            {
                case 3:
                    return "Tea";
                default:
                    return String.Empty;
            }
        }

        internal static bool AssignDefaultModData<T>(T type, string modelName, bool trackSeason = false, bool trackSheetId = false)
        {
            var textureModel = new AlternativeTextureModel() { Owner = AlternativeTextures.DEFAULT_OWNER, Season = trackSeason ? Game1.currentSeason : String.Empty };
            switch (type)
            {
                case Object obj:
                    AssignObjectModData(obj, modelName, textureModel, -1, trackSeason, trackSheetId);
                    return true;
                case TerrainFeature terrain:
                    AssignTerrainFeatureModData(terrain, modelName, textureModel, -1, trackSeason);
                    return true;
            }

            return false;
        }

        internal static bool AssignModData<T>(T type, string modelName, bool trackSeason = false, bool trackSheetId = false)
        {
            var textureModel = AlternativeTextures.textureManager.GetRandomTextureModel(modelName);

            var selectedVariation = Game1.random.Next(-1, textureModel.Variations);
            if (textureModel.ManualVariations.Count() > 0)
            {
                var weightedSelection = textureModel.ManualVariations.Where(v => v.ChanceWeight >= Game1.random.NextDouble()).ToList();
                if (weightedSelection.Count > 0)
                {
                    var randomWeightedSelection = Game1.random.Next(!textureModel.ManualVariations.Any(v => v.Id == -1) ? -1 : 0, weightedSelection.Count());
                    selectedVariation = randomWeightedSelection == -1 ? -1 : weightedSelection[randomWeightedSelection].Id;
                }
            }

            switch (type)
            {
                case Object obj:
                    AssignObjectModData(obj, modelName, textureModel, selectedVariation, trackSeason, trackSheetId);
                    return true;
                case TerrainFeature terrain:
                    AssignTerrainFeatureModData(terrain, modelName, textureModel, selectedVariation, trackSeason);
                    return true;
            }

            return false;
        }

        private static void AssignObjectModData(Object obj, string modelName, AlternativeTextureModel textureModel, int variation, bool trackSeason = false, bool trackSheetId = false)
        {
            obj.modData["AlternativeTextureOwner"] = textureModel.Owner;
            obj.modData["AlternativeTextureName"] = String.Concat(textureModel.Owner, ".", modelName);

            if (trackSeason && !String.IsNullOrEmpty(textureModel.Season))
            {
                obj.modData["AlternativeTextureSeason"] = Game1.currentSeason;
            }

            if (trackSheetId)
            {
                obj.modData["AlternativeTextureSheetId"] = obj.ParentSheetIndex.ToString();
            }

            obj.modData["AlternativeTextureVariation"] = variation.ToString();
        }

        private static void AssignTerrainFeatureModData(TerrainFeature terrain, string modelName, AlternativeTextureModel textureModel, int variation, bool trackSeason = false)
        {
            terrain.modData["AlternativeTextureOwner"] = textureModel.Owner;
            terrain.modData["AlternativeTextureName"] = String.Concat(textureModel.Owner, ".", modelName);

            if (trackSeason && !String.IsNullOrEmpty(textureModel.Season))
            {
                terrain.modData["AlternativeTextureSeason"] = Game1.GetSeasonForLocation(terrain.currentLocation);
            }

            terrain.modData["AlternativeTextureVariation"] = variation.ToString();
        }
    }
}
