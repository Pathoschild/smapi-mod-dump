/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SolidFoundations.Framework.Models.ContentPack;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidFoundations.Framework.Managers
{
    internal class BuildingManager
    {
        private IMonitor _monitor;
        private IModHelper _helper;

        private Dictionary<string, string> _assetPathToMap;
        private Dictionary<string, string> _assetToTileSheet;
        private Dictionary<string, string> _assetPathToTexturePath;
        private Dictionary<string, Texture2D> _assetPathToTexture;
        private Dictionary<string, ExtendedBuildingModel> _idToModels;

        public BuildingManager(IMonitor monitor, IModHelper helper)
        {
            _monitor = monitor;
            _helper = helper;

            _assetPathToMap = new Dictionary<string, string>();
            _assetToTileSheet = new Dictionary<string, string>();
            _assetPathToTexturePath = new Dictionary<string, string>();
            _assetPathToTexture = new Dictionary<string, Texture2D>();
            _idToModels = new Dictionary<string, ExtendedBuildingModel>();
        }

        public void Reset()
        {
            _assetPathToMap.Clear();
            _assetToTileSheet.Clear();
            _assetPathToTexture.Clear();
            _idToModels.Clear();
        }

        public void AddBuilding(ExtendedBuildingModel model)
        {
            _idToModels[model.ID] = model;

            Game1.buildingData[model.ID] = model;
        }

        public bool UpdateModel(ExtendedBuildingModel model)
        {
            if (model is null || DoesBuildingModelExist(model.ID) is false)
            {
                return false;
            }

            _idToModels[model.ID] = model;
            Game1.buildingData[model.ID] = model;

            return true;
        }

        public void AddMapAsset(string assetPath, string pathToMap)
        {
            if (String.IsNullOrEmpty(assetPath) || String.IsNullOrEmpty(pathToMap))
            {
                return;
            }

            _assetPathToMap[$"Maps/{assetPath}"] = pathToMap;
        }

        public void AddTileSheetAsset(string assetPath, string pathToTileSheet)
        {
            if (String.IsNullOrEmpty(assetPath) || String.IsNullOrEmpty(pathToTileSheet))
            {
                return;
            }

            _assetToTileSheet[assetPath] = pathToTileSheet;
        }

        public void AddTextureAsset(string assetPath, string pathToTexture, IContentPack contentPack)
        {
            if (String.IsNullOrEmpty(assetPath) || String.IsNullOrEmpty(pathToTexture) || contentPack is null)
            {
                return;
            }

            _assetPathToTexture[assetPath] = contentPack.ModContent.Load<Texture2D>(pathToTexture);
            _assetPathToTexturePath[assetPath] = pathToTexture;
        }

        public string GetMapAsset(string assetPath)
        {
            if (_assetPathToMap.ContainsKey(assetPath))
            {
                return _assetPathToMap[assetPath];
            }

            return null;
        }

        public string GetTileSheetAsset(string assetPath)
        {
            if (_assetToTileSheet.ContainsKey(assetPath))
            {
                return _assetToTileSheet[assetPath];
            }

            return null;
        }

        public string GetTextureAssetPath(string assetPath)
        {
            if (String.IsNullOrEmpty(assetPath) is false && _assetPathToTexturePath.ContainsKey(assetPath))
            {
                return _assetPathToTexturePath[assetPath];
            }

            return null;
        }

        public Texture2D GetTextureAsset(string assetPath)
        {
            if (String.IsNullOrEmpty(assetPath) is false && _assetPathToTexture.ContainsKey(assetPath))
            {
                return _assetPathToTexture[assetPath];
            }

            return null;
        }

        public List<ExtendedBuildingModel> GetAllBuildingModels()
        {
            return _idToModels.Values.ToList();
        }

        public Dictionary<string, ExtendedBuildingModel> GetIdToModels()
        {
            return _idToModels;
        }

        public ExtendedBuildingModel GetSpecificBuildingModel(string buildingId)
        {
            return String.IsNullOrEmpty(buildingId) is false && _idToModels.ContainsKey(buildingId) ? _idToModels[buildingId] : null;
        }

        public bool DoesBuildingModelExist(string buildingId)
        {
            return String.IsNullOrEmpty(buildingId) is false && _idToModels.ContainsKey(buildingId);
        }

        public List<Building> GetAllActiveBuildings(GameLocation buildableGameLocation)
        {
            var activeBuildings = new List<Building>();
            foreach (var building in buildableGameLocation.buildings)
            {
                if (building is null || DoesBuildingModelExist(building.buildingType.Value))
                {
                    continue;
                }
                activeBuildings.Add(building);

                if (building.indoors.Value is not null && building.indoors.Value is GameLocation subBuildableGameLocation)
                {
                    activeBuildings.AddRange(GetAllActiveBuildings(subBuildableGameLocation));
                }
            }

            return activeBuildings;
        }

        public List<Building> GetAllActiveBuildings()
        {
            var activeBuildings = new List<Building>();
            foreach (var location in Game1.locations)
            {
                if (location is not GameLocation gameLocation || gameLocation.buildings is null)
                {
                    continue;
                }

                activeBuildings.AddRange(GetAllActiveBuildings(gameLocation));
            }

            return activeBuildings;
        }
    }
}
