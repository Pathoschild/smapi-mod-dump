/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures.Framework.Interfaces;
using AlternativeTextures.Framework.Models;
using AlternativeTextures.Framework.Patches.Entities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using static AlternativeTextures.Framework.Models.AlternativeTextureModel;
using Object = StardewValley.Object;

namespace AlternativeTextures.Framework.Patches
{
    internal class PatchTemplate
    {
        internal static IMonitor _monitor;
        internal static IModHelper _helper;

        internal PatchTemplate(IMonitor modMonitor, IModHelper modHelper)
        {
            _monitor = modMonitor;
            _helper = modHelper;
        }

        internal static GenericTool GetPaintBucketTool()
        {
            var paintBucket = new GenericTool(_helper.Translation.Get("tools.name.paint_bucket"), _helper.Translation.Get("tools.description.paint_bucket"), -1, 6, 6);
            paintBucket.modData[AlternativeTextures.PAINT_BUCKET_FLAG] = true.ToString();

            return paintBucket;
        }

        internal static GenericTool GetScissorsTool()
        {
            var scissors = new GenericTool(_helper.Translation.Get("tools.name.scissors"), _helper.Translation.Get("tools.description.scissors"), -1, 6, 6);
            scissors.modData[AlternativeTextures.SCISSORS_FLAG] = true.ToString();

            return scissors;
        }

        internal static GenericTool GetPaintBrushTool()
        {
            var paintBrush = new GenericTool(_helper.Translation.Get("tools.name.paint_brush"), _helper.Translation.Get("tools.description.paint_brush"), -1, 6, 6);
            paintBrush.modData[AlternativeTextures.PAINT_BRUSH_FLAG] = null;

            return paintBrush;
        }

        internal static string GetObjectName(Object obj)
        {
            // Perform separate check for DGA objects, before using check for vanilla objects
            if (IsDGAUsed() && AlternativeTextures.apiManager.GetDynamicGameAssetsApi() is IDynamicGameAssetsApi api && api != null)
            {
                var dgaId = api.GetDGAItemId(obj);
                if (dgaId != null)
                {
                    return dgaId;
                }
            }

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

        internal static string GetCharacterName(Character character)
        {
            if (character is Child child)
            {
                if (child.Age >= 3)
                {
                    return $"{CharacterPatch.TODDLER_NAME_PREFIX}_{(child.Gender == 0 ? "Male" : "Female")}_{(child.darkSkinned ? "Dark" : "Light")}";
                }
                return $"{CharacterPatch.BABY_NAME_PREFIX}_{(child.darkSkinned ? "Dark" : "Light")}";
            }

            if (character is FarmAnimal animal)
            {
                var animalName = animal.type.Value;
                if (animal.age < animal.ageWhenMature)
                {
                    animalName = "Baby" + (animal.type.Value.Equals("Duck") ? "White Chicken" : animal.type.Value);
                }
                else if (animal.showDifferentTextureWhenReadyForHarvest && animal.currentProduce <= 0)
                {
                    animalName = "Sheared" + animalName;
                }
                return animalName;
            }

            if (character is Horse horse)
            {
                // Tractor mod compatibility: -794739 is the ID used by Tractor Mod for determining if a Stable is really a garage
                if (horse.modData.ContainsKey("Pathoschild.TractorMod"))
                {
                    return "Tractor";
                }

                return "Horse";
            }

            if (character is Pet pet)
            {
                return pet is Cat ? "Cat" : "Dog";
            }

            return character.name;
        }

        internal static string GetBuildingName(Building building)
        {
            // Tractor mod compatibility: -794739 is the ID used by Tractor Mod for determining if a Stable is really a garage
            if (building.maxOccupants.Value == -794739)
            {
                return "Tractor Garage";
            }

            return building.buildingType;
        }

        internal static Object GetObjectAt(GameLocation location, int x, int y)
        {
            // If object is furniture and currently has something on top of it, check that instead
            foreach (var furniture in location.furniture.Where(c => c.heldObject.Value != null))
            {
                if (furniture.boundingBox.Value.Contains(x, y))
                {
                    return furniture.heldObject.Value;
                }
            }

            // Prioritize checking non-rug furniture first
            foreach (var furniture in location.furniture.Where(c => c.furniture_type != Furniture.rug))
            {
                if (furniture.boundingBox.Value.Contains(x, y))
                {
                    return furniture;
                }
            }

            // Replicating GameLocation.getObjectAt, but doing objects before rugs
            // Doing this so the object on top of rugs are given instead of the latter
            var tile = new Vector2(x / 64, y / 64);
            if (location.objects.ContainsKey(tile))
            {
                return location.objects[tile];
            }

            return location.getObjectAt(x, y);
        }

        internal static Building GetBuildingAt(GameLocation location, int x, int y)
        {
            Vector2 tile = new Vector2(x / 64, y / 64);
            if (location is Farm farm && farm.buildings.FirstOrDefault(b => b.occupiesTile(tile)) is Building building && building != null)
            {
                return building;
            }

            return null;
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

        internal static Character GetCharacterAt(GameLocation location, int x, int y)
        {
            var tileLocation = new Vector2(x / 64, y / 64);
            var rectangle = new Rectangle(x, y, 64, 64);
            if (location is Farm farm)
            {
                foreach (var animal in farm.animals.Values)
                {
                    if (animal.GetBoundingBox().Intersects(rectangle))
                    {
                        return animal;
                    }
                }
            }
            if (location is AnimalHouse animalHouse)
            {
                foreach (var animal in animalHouse.animals.Values)
                {
                    if (animal.GetBoundingBox().Intersects(rectangle))
                    {
                        return animal;
                    }
                }
            }

            foreach (var specialCharacter in location.characters.Where(c => c is Horse || c is Pet))
            {
                if (specialCharacter is Horse horse && horse.GetBoundingBox().Intersects(rectangle))
                {
                    return horse;
                }

                if (specialCharacter is Pet pet && pet.GetBoundingBox().Intersects(rectangle))
                {
                    return pet;
                }
            }

            return location.isCharacterAtTile(tileLocation);
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

        internal static string GetTreeTypeString(Tree tree)
        {
            switch (tree.treeType.Value)
            {
                case Tree.bushyTree:
                    return "Oak";
                case Tree.leafyTree:
                    return "Maple";
                case Tree.pineTree:
                    return "Pine";
                case Tree.mahoganyTree:
                    return "Mahogany";
                case Tree.mushroomTree:
                    return "Mushroom";
                case Tree.palmTree:
                    return "Palm_1";
                case Tree.palmTree2:
                    return "Palm_2";
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

        internal static TextureType GetTextureType(object obj)
        {
            switch (obj)
            {
                case Character character:
                    return TextureType.Character;
                case Flooring floor:
                    return TextureType.Flooring;
                case Tree tree:
                    return TextureType.Tree;
                case FruitTree fruitTree:
                    return TextureType.FruitTree;
                case Building building:
                    return TextureType.Building;
                case Furniture furniture:
                    return TextureType.Furniture;
                case Object craftable:
                    return TextureType.Craftable;
                case DecoratableLocation location:
                    return TextureType.Decoration;
                default:
                    return TextureType.Unknown;
            }
        }

        internal static bool HasCachedTextureName<T>(T type, bool probe = false)
        {
            if (type is Object obj && obj.modData.ContainsKey("AlternativeTextureNameCached"))
            {
                if (!probe)
                {
                    obj.modData["AlternativeTextureName"] = obj.modData["AlternativeTextureNameCached"];
                    obj.modData.Remove("AlternativeTextureNameCached");
                }

                return true;
            }

            return false;
        }

        internal static bool IsDGAUsed()
        {
            return _helper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets");
        }

        internal static bool IsDGAObject(object obj)
        {
            if (IsDGAUsed() && AlternativeTextures.apiManager.GetDynamicGameAssetsApi() is IDynamicGameAssetsApi api && api != null)
            {
                var dgaId = api.GetDGAItemId(obj);
                if (dgaId != null)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool AssignDefaultModData<T>(T type, string modelName, bool trackSeason = false, bool trackSheetId = false)
        {
            if (HasCachedTextureName(type))
            {
                return false;
            }

            var textureModel = new AlternativeTextureModel() { Owner = AlternativeTextures.DEFAULT_OWNER, Season = trackSeason ? Game1.currentSeason : String.Empty };
            switch (type)
            {
                case Object obj:
                    AssignObjectModData(obj, modelName, textureModel, -1, trackSeason, trackSheetId);
                    return true;
                case TerrainFeature terrain:
                    AssignTerrainFeatureModData(terrain, modelName, textureModel, -1, trackSeason);
                    return true;
                case Character character:
                    AssignCharacterModData(character, modelName, textureModel, -1, trackSeason);
                    return true;
                case Building building:
                    AssignBuildingModData(building, modelName, textureModel, -1, trackSeason);
                    return true;
                case DecoratableLocation decoratableLocation:
                    AssignDecoratableLocationModData(decoratableLocation, modelName, textureModel, -1, trackSeason);
                    return true;
                case Farm farm:
                    AssignFarmModData(farm, modelName, textureModel, -1, trackSeason);
                    return true;
            }

            return false;
        }

        internal static bool AssignModData<T>(T type, string modelName, bool trackSeason = false, bool trackSheetId = false)
        {
            if (HasCachedTextureName(type))
            {
                return false;
            }

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
                case Character character:
                    AssignCharacterModData(character, modelName, textureModel, selectedVariation, trackSeason);
                    return true;
                case Building building:
                    AssignBuildingModData(building, modelName, textureModel, selectedVariation, trackSeason);
                    return true;
                case DecoratableLocation decoratableLocation:
                    AssignDecoratableLocationModData(decoratableLocation, modelName, textureModel, selectedVariation, trackSeason);
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

        private static void AssignCharacterModData(Character character, string modelName, AlternativeTextureModel textureModel, int variation, bool trackSeason = false)
        {
            character.modData["AlternativeTextureOwner"] = textureModel.Owner;
            character.modData["AlternativeTextureName"] = String.Concat(textureModel.Owner, ".", modelName);

            if (trackSeason && !String.IsNullOrEmpty(textureModel.Season))
            {
                character.modData["AlternativeTextureSeason"] = Game1.GetSeasonForLocation(character.currentLocation);
            }

            character.modData["AlternativeTextureVariation"] = variation.ToString();
        }

        private static void AssignBuildingModData(Building building, string modelName, AlternativeTextureModel textureModel, int variation, bool trackSeason = false)
        {
            building.modData["AlternativeTextureOwner"] = textureModel.Owner;
            building.modData["AlternativeTextureName"] = String.Concat(textureModel.Owner, ".", modelName);

            if (trackSeason && !String.IsNullOrEmpty(textureModel.Season))
            {
                building.modData["AlternativeTextureSeason"] = Game1.currentSeason;
            }

            building.modData["AlternativeTextureVariation"] = variation.ToString();
        }

        private static void AssignDecoratableLocationModData(DecoratableLocation decoratableLocation, string modelName, AlternativeTextureModel textureModel, int variation, bool trackSeason = false)
        {
            decoratableLocation.modData["AlternativeTextureOwner"] = textureModel.Owner;
            decoratableLocation.modData["AlternativeTextureName"] = String.Concat(textureModel.Owner, ".", modelName);

            if (trackSeason && !String.IsNullOrEmpty(textureModel.Season))
            {
                decoratableLocation.modData["AlternativeTextureSeason"] = Game1.currentSeason;
            }

            decoratableLocation.modData["AlternativeTextureVariation"] = variation.ToString();
        }

        private static void AssignFarmModData(Farm farm, string modelName, AlternativeTextureModel textureModel, int variation, bool trackSeason = false)
        {
            farm.modData["AlternativeTextureOwner"] = textureModel.Owner;
            farm.modData["AlternativeTextureName"] = String.Concat(textureModel.Owner, ".", modelName);

            if (trackSeason && !String.IsNullOrEmpty(textureModel.Season))
            {
                farm.modData["AlternativeTextureSeason"] = Game1.currentSeason;
            }

            farm.modData["AlternativeTextureVariation"] = variation.ToString();
        }
    }
}
