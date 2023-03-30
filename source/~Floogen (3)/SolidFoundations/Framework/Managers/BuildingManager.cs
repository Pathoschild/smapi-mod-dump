/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using SolidFoundations.Framework.Models.ContentPack;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidFoundations.Framework.Managers
{
    internal class BuildingManager
    {
        private IMonitor _monitor;
        private IModHelper _helper;

        private Dictionary<string, string> _assetPathToMap;
        private Dictionary<string, string> _assetToTileSheet;
        private Dictionary<string, string> _assetPathToTexture;
        private Dictionary<string, ExtendedBuildingModel> _idToModels;

        public BuildingManager(IMonitor monitor, IModHelper helper)
        {
            _monitor = monitor;
            _helper = helper;

            _assetPathToMap = new Dictionary<string, string>();
            _assetToTileSheet = new Dictionary<string, string>();
            _assetPathToTexture = new Dictionary<string, string>();
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
        }

        public bool UpdateModel(ExtendedBuildingModel model)
        {
            if (model is null || DoesBuildingModelExist(model.ID) is false)
            {
                return false;
            }
            _idToModels[model.ID] = model;

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

        public void AddTextureAsset(string assetPath, string pathToTexture)
        {
            if (String.IsNullOrEmpty(assetPath) || String.IsNullOrEmpty(pathToTexture))
            {
                return;
            }

            _assetPathToTexture[assetPath] = pathToTexture;
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

        public string GetTextureAsset(string assetPath)
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

        // TODO: After SDV v1.6, revise GetAllActiveBuildings to handle all buildings, instead of just SF buildings
        public List<GenericBuilding> GetAllActiveBuildings(BuildableGameLocation buildableGameLocation)
        {
            var activeBuildings = new List<GenericBuilding>();
            foreach (var building in buildableGameLocation.buildings)
            {
                if (building is not GenericBuilding genericBuilding || genericBuilding is null)
                {
                    continue;
                }
                activeBuildings.Add(genericBuilding);

                if (genericBuilding.indoors.Value is not null && genericBuilding.indoors.Value is BuildableGameLocation subBuildableGameLocation)
                {
                    activeBuildings.AddRange(GetAllActiveBuildings(subBuildableGameLocation));
                }
            }

            return activeBuildings;
        }

        public List<GenericBuilding> GetAllActiveBuildings()
        {
            var activeBuildings = new List<GenericBuilding>();
            foreach (var location in Game1.locations)
            {
                if (location is not BuildableGameLocation buildableGameLocation || buildableGameLocation.buildings is null)
                {
                    continue;
                }

                activeBuildings.AddRange(GetAllActiveBuildings(buildableGameLocation));
            }

            return activeBuildings;
        }
    }
}
