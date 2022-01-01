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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AlternativeTextures.Framework.Models.AlternativeTextureModel;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches.Tools
{
    internal class ToolPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Tool);

        internal ToolPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, "get_DisplayName", null), postfix: new HarmonyMethod(GetType(), nameof(GetNamePostfix)));
            harmony.Patch(AccessTools.Method(_object, "get_description", null), postfix: new HarmonyMethod(GetType(), nameof(GetDescriptionPostfix)));


            harmony.Patch(AccessTools.Method(_object, nameof(Tool.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(DrawInMenuPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(Tool.beginUsing), new[] { typeof(GameLocation), typeof(int), typeof(int), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(BeginUsingPrefix)));
        }

        private static void GetNamePostfix(Tool __instance, ref string __result)
        {
            if (__instance.modData.ContainsKey(AlternativeTextures.PAINT_BUCKET_FLAG))
            {
                __result = _helper.Translation.Get("tools.name.paint_bucket");
                return;
            }

            if (__instance.modData.ContainsKey(AlternativeTextures.SCISSORS_FLAG))
            {
                __result = _helper.Translation.Get("tools.name.scissors");
                return;
            }

            if (__instance.modData.ContainsKey(AlternativeTextures.PAINT_BRUSH_FLAG))
            {
                __result = _helper.Translation.Get("tools.name.paint_brush");
                return;
            }
        }

        private static void GetDescriptionPostfix(Tool __instance, ref string __result)
        {
            if (__instance.modData.ContainsKey(AlternativeTextures.PAINT_BUCKET_FLAG))
            {
                __result = _helper.Translation.Get("tools.description.paint_bucket");
                return;
            }

            if (__instance.modData.ContainsKey(AlternativeTextures.SCISSORS_FLAG))
            {
                __result = _helper.Translation.Get("tools.description.scissors");
                return;
            }

            if (__instance.modData.ContainsKey(AlternativeTextures.PAINT_BRUSH_FLAG))
            {
                __result = _helper.Translation.Get("tools.description.paint_brush");
                return;
            }
        }

        private static bool DrawInMenuPrefix(Tool __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (__instance.modData.ContainsKey(AlternativeTextures.PAINT_BUCKET_FLAG))
            {
                spriteBatch.Draw(AlternativeTextures.assetManager.GetPaintBucketTexture(), location + new Vector2(32f, 32f), new Rectangle(0, 0, 16, 16), color * transparency, 0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);

                return false;
            }

            if (__instance.modData.ContainsKey(AlternativeTextures.SCISSORS_FLAG))
            {
                spriteBatch.Draw(AlternativeTextures.assetManager.GetScissorsTexture(), location + new Vector2(32f, 32f), new Rectangle(0, 0, 16, 16), color * transparency, 0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);

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

            if (__instance.modData.ContainsKey(AlternativeTextures.SCISSORS_FLAG))
            {
                __result = true;
                return UseScissors(location, x, y, who);
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
            if (location is Farm farm)
            {
                var targetedBuilding = farm.getBuildingAt(new Vector2(x / 64, y / 64));
                if (farm.GetHouseRect().Contains(new Vector2(x / 64, y / 64)))
                {
                    targetedBuilding = new Building();
                    targetedBuilding.buildingType.Value = $"Farmhouse_{Game1.MasterPlayer.HouseUpgradeLevel}";
                    targetedBuilding.tileX.Value = farm.GetHouseRect().X;
                    targetedBuilding.tileY.Value = farm.GetHouseRect().Y;
                    targetedBuilding.tilesWide.Value = farm.GetHouseRect().Width;
                    targetedBuilding.tilesHigh.Value = farm.GetHouseRect().Height;

                    var modelType = AlternativeTextureModel.TextureType.Building;
                    if (!farm.modData.ContainsKey("AlternativeTextureName") || !farm.modData["AlternativeTextureName"].Contains(targetedBuilding.buildingType.Value))
                    {
                        var instanceSeasonName = $"{modelType}_{targetedBuilding.buildingType}_{Game1.currentSeason}";
                        AssignDefaultModData(farm, instanceSeasonName, true);
                    }

                    foreach (string key in farm.modData.Keys)
                    {
                        targetedBuilding.modData[key] = farm.modData[key];
                    }
                }

                if (targetedBuilding != null)
                {
                    // Assign default data if none exists
                    if (!targetedBuilding.modData.ContainsKey("AlternativeTextureName"))
                    {
                        var modelType = AlternativeTextureModel.TextureType.Building;
                        var instanceSeasonName = $"{modelType}_{targetedBuilding.buildingType}_{Game1.currentSeason}";
                        AssignDefaultModData(targetedBuilding, instanceSeasonName, true);
                    }

                    var modelName = targetedBuilding.modData["AlternativeTextureName"].Replace($"{targetedBuilding.modData["AlternativeTextureOwner"]}.", String.Empty);
                    if (targetedBuilding.modData.ContainsKey("AlternativeTextureSeason") && !String.IsNullOrEmpty(targetedBuilding.modData["AlternativeTextureSeason"]))
                    {
                        modelName = modelName.Replace($"_{targetedBuilding.modData["AlternativeTextureSeason"]}", String.Empty);
                    }

                    if (AlternativeTextures.textureManager.GetAvailableTextureModels(modelName, Game1.GetSeasonForLocation(Game1.currentLocation)).Count == 0)
                    {
                        Game1.addHUDMessage(new HUDMessage(_helper.Translation.Get("messages.warning.no_textures_for_season", new { itemName = modelName }), 3));
                        return CancelUsing(who);
                    }

                    // Verify this building has a texture we can target
                    if (!farm.GetHouseRect().Contains(new Vector2(x / 64, y / 64)))
                    {
                        var texturePath = PathUtilities.NormalizePath(Path.Combine(targetedBuilding.textureName() + ".png"));
                        try
                        {
                            _ = _helper.Content.Load<Texture2D>(Path.Combine(targetedBuilding.textureName()), ContentSource.GameContent);
                            _monitor.Log($"{modelName} has a targetable texture within Buildings: {texturePath}", LogLevel.Trace);
                        }
                        catch (ContentLoadException ex)
                        {
                            Game1.addHUDMessage(new HUDMessage(AlternativeTextures.modHelper.Translation.Get("messages.warning.custom_building_not_supported", new { itemName = modelName }), 3));
                            return CancelUsing(who);
                        }
                    }
                    // Display texture menu
                    var buildingObj = new Object(100, 1, isRecipe: false, -1)
                    {
                        TileLocation = new Vector2(targetedBuilding.tileX, targetedBuilding.tileY),
                        modData = targetedBuilding.modData
                    };
                    Game1.activeClickableMenu = new PaintBucketMenu(buildingObj, buildingObj.TileLocation * 64f, GetTextureType(targetedBuilding), modelName, _helper.Translation.Get("tools.name.paint_bucket"), textureTileWidth: targetedBuilding.tilesWide);

                    return CancelUsing(who);
                }
            }

            var targetedObject = GetObjectAt(location, x, y);
            if (targetedObject != null)
            {
                // Assign default data if none exists
                if (!targetedObject.modData.ContainsKey("AlternativeTextureName"))
                {
                    var instanceSeasonName = $"{GetTextureType(targetedObject)}_{GetObjectName(targetedObject)}_{Game1.currentSeason}";
                    AssignDefaultModData(targetedObject, instanceSeasonName, true);
                }

                var modelName = targetedObject.modData["AlternativeTextureName"].Replace($"{targetedObject.modData["AlternativeTextureOwner"]}.", String.Empty);
                if (targetedObject.modData.ContainsKey("AlternativeTextureSeason") && !String.IsNullOrEmpty(targetedObject.modData["AlternativeTextureSeason"]))
                {
                    modelName = modelName.Replace($"_{targetedObject.modData["AlternativeTextureSeason"]}", String.Empty);
                }

                if (AlternativeTextures.textureManager.GetAvailableTextureModels(modelName, Game1.GetSeasonForLocation(Game1.currentLocation)).Count == 0)
                {
                    Game1.addHUDMessage(new HUDMessage(_helper.Translation.Get("messages.warning.no_textures_for_season", new { itemName = modelName }), 3));
                    return CancelUsing(who);
                }

                // Display texture menu
                Game1.activeClickableMenu = new PaintBucketMenu(targetedObject, new Vector2(x, y), GetTextureType(targetedObject), modelName, _helper.Translation.Get("tools.name.paint_bucket"));

                return CancelUsing(who);
            }

            var targetedTerrain = GetTerrainFeatureAt(location, x, y);
            if (targetedTerrain != null)
            {
                if (targetedTerrain is HoeDirt || targetedTerrain is GiantCrop || targetedTerrain is Grass)
                {
                    Game1.addHUDMessage(new HUDMessage(_helper.Translation.Get("messages.warning.paint_not_placeable"), 3));
                    return CancelUsing(who);
                }

                if (!targetedTerrain.modData.ContainsKey("AlternativeTextureName"))
                {
                    if (targetedTerrain is Flooring flooring)
                    {
                        var instanceSeasonName = $"{AlternativeTextureModel.TextureType.Flooring}_{GetFlooringName(flooring)}_{Game1.GetSeasonForLocation(Game1.currentLocation)}";
                        AssignDefaultModData(targetedTerrain, instanceSeasonName, true);
                    }
                    else if (targetedTerrain is Tree tree)
                    {
                        var instanceSeasonName = $"{AlternativeTextureModel.TextureType.Tree}_{GetTreeTypeString(tree)}_{Game1.GetSeasonForLocation(Game1.currentLocation)}";
                        AssignDefaultModData(targetedTerrain, instanceSeasonName, true);
                    }
                    else if (targetedTerrain is FruitTree fruitTree)
                    {
                        Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>("Data\\fruitTrees");
                        var saplingIndex = data.FirstOrDefault(d => int.Parse(d.Value.Split('/')[0]) == fruitTree.treeType).Key;
                        var saplingName = Game1.objectInformation.ContainsKey(saplingIndex) ? Game1.objectInformation[saplingIndex].Split('/')[0] : String.Empty;

                        var instanceSeasonName = $"{AlternativeTextureModel.TextureType.FruitTree}_{saplingName}_{Game1.GetSeasonForLocation(Game1.currentLocation)}";
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

                if (AlternativeTextures.textureManager.GetAvailableTextureModels(modelName, Game1.GetSeasonForLocation(Game1.currentLocation)).Count == 0)
                {
                    Game1.addHUDMessage(new HUDMessage(_helper.Translation.Get("messages.warning.no_textures_for_season", new { itemName = modelName }), 3));
                    return CancelUsing(who);
                }

                // Display texture menu
                var terrainObj = new Object(100, 1, isRecipe: false, -1)
                {
                    TileLocation = targetedTerrain.currentTileLocation,
                    modData = targetedTerrain.modData
                };
                Game1.activeClickableMenu = new PaintBucketMenu(terrainObj, terrainObj.TileLocation * 64f, GetTextureType(targetedTerrain), modelName, _helper.Translation.Get("tools.name.paint_bucket"));

                return CancelUsing(who);
            }

            if (location is DecoratableLocation decoratableLocation)
            {
                Point tile = new Point(x / 64, y / 64);

                var wallId = decoratableLocation.getWallForRoomAt(tile);
                if (wallId != -1)
                {
                    if (!decoratableLocation.modData.ContainsKey("AlternativeTextureName") || !decoratableLocation.modData["AlternativeTextureName"].Contains("Wallpaper"))
                    {
                        var instanceSeasonName = $"{AlternativeTextureModel.TextureType.Decoration}_Wallpaper_{Game1.GetSeasonForLocation(decoratableLocation)}";
                        AssignDefaultModData(decoratableLocation, instanceSeasonName, true);
                    }

                    var modelName = decoratableLocation.modData["AlternativeTextureName"].Replace($"{decoratableLocation.modData["AlternativeTextureOwner"]}.", String.Empty);
                    if (decoratableLocation.modData.ContainsKey("AlternativeTextureSeason") && !String.IsNullOrEmpty(decoratableLocation.modData["AlternativeTextureSeason"]))
                    {
                        modelName = modelName.Replace($"_{decoratableLocation.modData["AlternativeTextureSeason"]}", String.Empty);
                    }

                    if (AlternativeTextures.textureManager.GetAvailableTextureModels(modelName, Game1.GetSeasonForLocation(Game1.currentLocation)).Count == 0)
                    {
                        Game1.addHUDMessage(new HUDMessage(_helper.Translation.Get("messages.warning.no_textures_for_season", new { itemName = modelName }), 3));
                        return CancelUsing(who);
                    }

                    // Display texture menu
                    var locationObj = new Object(100, 1, isRecipe: false, -1)
                    {
                        TileLocation = Utility.PointToVector2(tile),
                        modData = decoratableLocation.modData
                    };
                    Game1.activeClickableMenu = new PaintBucketMenu(locationObj, locationObj.TileLocation, GetTextureType(decoratableLocation), modelName, _helper.Translation.Get("tools.name.paint_bucket"));

                    return CancelUsing(who);
                }

                var floorId = decoratableLocation.getFloorAt(tile);
                if (floorId != -1)
                {
                    if (!decoratableLocation.modData.ContainsKey("AlternativeTextureName") || !decoratableLocation.modData["AlternativeTextureName"].Contains("Floor"))
                    {
                        var instanceSeasonName = $"{AlternativeTextureModel.TextureType.Decoration}_Floor_{Game1.GetSeasonForLocation(decoratableLocation)}";
                        AssignDefaultModData(decoratableLocation, instanceSeasonName, true);
                    }

                    var modelName = decoratableLocation.modData["AlternativeTextureName"].Replace($"{decoratableLocation.modData["AlternativeTextureOwner"]}.", String.Empty);
                    if (decoratableLocation.modData.ContainsKey("AlternativeTextureSeason") && !String.IsNullOrEmpty(decoratableLocation.modData["AlternativeTextureSeason"]))
                    {
                        modelName = modelName.Replace($"_{decoratableLocation.modData["AlternativeTextureSeason"]}", String.Empty);
                    }

                    if (AlternativeTextures.textureManager.GetAvailableTextureModels(modelName, Game1.GetSeasonForLocation(Game1.currentLocation)).Count == 0)
                    {
                        Game1.addHUDMessage(new HUDMessage(_helper.Translation.Get("messages.warning.no_textures_for_season", new { itemName = modelName }), 3));
                        return CancelUsing(who);
                    }

                    // Display texture menu
                    var locationObj = new Object(100, 1, isRecipe: false, -1)
                    {
                        TileLocation = Utility.PointToVector2(tile),
                        modData = decoratableLocation.modData
                    };
                    Game1.activeClickableMenu = new PaintBucketMenu(locationObj, locationObj.TileLocation, GetTextureType(decoratableLocation), modelName, _helper.Translation.Get("tools.name.paint_bucket"));

                    return CancelUsing(who);
                }
            }

            return CancelUsing(who);
        }

        private static bool UseScissors(GameLocation location, int x, int y, Farmer who)
        {
            var character = GetCharacterAt(location, x, y);
            if (character != null)
            {
                // Assign default data if none exists
                if (!character.modData.ContainsKey("AlternativeTextureName"))
                {
                    var modelType = AlternativeTextureModel.TextureType.Character;
                    var instanceSeasonName = $"{modelType}_{GetCharacterName(character)}_{Game1.currentSeason}";
                    AssignDefaultModData(character, instanceSeasonName, true);
                }

                var modelName = character.modData["AlternativeTextureName"].Replace($"{character.modData["AlternativeTextureOwner"]}.", String.Empty);
                if (character.modData.ContainsKey("AlternativeTextureSeason") && !String.IsNullOrEmpty(character.modData["AlternativeTextureSeason"]))
                {
                    modelName = modelName.Replace($"_{character.modData["AlternativeTextureSeason"]}", String.Empty);
                }

                if (AlternativeTextures.textureManager.GetAvailableTextureModels(modelName, Game1.GetSeasonForLocation(Game1.currentLocation)).Count == 0)
                {
                    Game1.addHUDMessage(new HUDMessage(_helper.Translation.Get("messages.warning.no_textures_for_season", new { itemName = modelName }), 3));
                    return CancelUsing(who);
                }

                // Display texture menu
                var obj = new Object(100, 1, isRecipe: false, -1)
                {
                    Name = character.Name,
                    displayName = character.displayName,
                    TileLocation = character.getTileLocation(),
                    modData = character.modData
                };
                Game1.activeClickableMenu = new PaintBucketMenu(obj, obj.TileLocation * 64f, GetTextureType(character), modelName, uiTitle: _helper.Translation.Get("tools.name.scissors"));

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
