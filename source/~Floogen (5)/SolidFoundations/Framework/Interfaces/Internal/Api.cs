/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolidFoundations.Framework.Models.ContentPack;
using SolidFoundations.Framework.Models.ContentPack.Actions;
using SolidFoundations.Framework.Patches.Buildings;
using SolidFoundations.Framework.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidFoundations.Framework.Interfaces.Internal
{
    public interface IApi
    {
        public class BroadcastEventArgs : EventArgs
        {
            public string BuildingId { get; set; }
            public Building Building { get; set; }
            public Farmer Farmer { get; set; }
            public Point TriggerTile { get; set; }
            public string Message { get; set; }
        }

        event EventHandler<BroadcastEventArgs> BroadcastSpecialActionTriggered;
        event EventHandler BeforeBuildingSerialization; // TODO: Mark this obsolete with SDV v1.6
        event EventHandler AfterBuildingRestoration; // TODO: Mark this obsolete with SDV v1.6

        public void AddBuildingFlags(Building building, List<string> flags, bool isTemporary = true);
        public void RemoveBuildingFlags(Building building, List<string> flags);
        public bool DoesBuildingHaveFlag(Building building, string flag);
        public KeyValuePair<bool, string> PlaceBuilding(string modelIdCaseSensitive, BuildableGameLocation location, Vector2 tileLocation);
        public KeyValuePair<bool, string> ConstructBuildingImmediately(string modelIdCaseSensitive, BuildableGameLocation location, Vector2 tileLocation);
        public KeyValuePair<bool, ExtendedBuildingModel> GetBuildingModel(Building building);
        public KeyValuePair<bool, ExtendedBuildingModel> GetBuildingModel(string modelId);
        public bool UpdateModel(ExtendedBuildingModel buildingModel);
        public KeyValuePair<bool, string> GetBuildingTexturePath(string modelIdCaseInsensitive);
        public KeyValuePair<bool, Texture2D> GetBuildingTexture(string modelIdCaseSensitive);

        /*
         * Example usage (modifies the "Crumbling Mineshaft" model from the Mystical Buildings SF pack)
         * 
         * var api = apiManager.GetSolidFoundationsApi();
         * var responseToModel = api.GetBuildingModel("PeacefulEnd.SolidFoundations.MysticalBuildings_CrumblingMineshaft");
         * if (responseToModel.Key is true)
         * {
         *   responseToModel.Value.BuildCost = 100;
         *   responseToModel.Value.BuildCondition = null;
         *   Monitor.Log($"Was model update successful: {api.UpdateModel(responseToModel.Value)}", LogLevel.Warn);
         * }
         */
    }

    public class Api : IApi
    {
        public event EventHandler<IApi.BroadcastEventArgs> BroadcastSpecialActionTriggered;
        public event EventHandler BeforeBuildingSerialization;
        public event EventHandler AfterBuildingRestoration;

        internal void OnSpecialActionTriggered(IApi.BroadcastEventArgs e)
        {
            EventHandler<IApi.BroadcastEventArgs> handler = BroadcastSpecialActionTriggered;
            if (handler is not null)
            {
                handler(this, e);
            }
        }

        internal void OnBeforeBuildingSerialization(EventArgs e)
        {
            EventHandler handler = BeforeBuildingSerialization;
            if (handler is not null)
            {
                handler(this, e);
            }
        }

        internal void OnAfterBuildingRestoration(EventArgs e)
        {
            EventHandler handler = AfterBuildingRestoration;
            if (handler is not null)
            {
                handler(this, e);
            }
        }

        public void AddBuildingFlags(Building building, List<string> flags, bool isTemporary = true)
        {
            foreach (var flag in flags)
            {
                var flagKey = String.Concat(ModDataKeys.FLAG_BASE, ".", flag.ToLower());
                building.modData[flagKey] = (isTemporary ? SpecialAction.FlagType.Temporary : SpecialAction.FlagType.Permanent).ToString();
            }
        }

        public void RemoveBuildingFlags(Building building, List<string> flags)
        {
            foreach (var flag in flags)
            {
                var flagKey = String.Concat(ModDataKeys.FLAG_BASE, ".", flag.ToLower());
                building.modData.Remove(flagKey);
            }
        }

        public bool DoesBuildingHaveFlag(Building building, string flag)
        {
            var flagKey = String.Concat(ModDataKeys.FLAG_BASE, ".", flag.ToLower());
            return building.modData.Keys.Any(k => k.Equals(flagKey, StringComparison.OrdinalIgnoreCase));
        }

        public KeyValuePair<bool, string> PlaceBuilding(string modelIdCaseSensitive, BuildableGameLocation location, Vector2 tileLocation)
        {
            if (String.IsNullOrEmpty(modelIdCaseSensitive) || SolidFoundations.buildingManager.DoesBuildingModelExist(modelIdCaseSensitive) is false)
            {
                return new KeyValuePair<bool, string>(false, $"No match for model {modelIdCaseSensitive}");
            }
            else if (location is null)
            {
                return new KeyValuePair<bool, string>(false, "BuildableGameLocation is null!");
            }

            var blueprint = new BluePrint(modelIdCaseSensitive);
            if (GameLocationPatch.AttemptToBuildStructure(location, blueprint, null, tileLocation, skipFarmerCheck: true) is false)
            {
                return new KeyValuePair<bool, string>(false, "Failed to place structure, see log for details.");
            }

            return new KeyValuePair<bool, string>(true, $"Succesfully placed {modelIdCaseSensitive} at {location.Name} on tile {tileLocation}!");
        }

        public KeyValuePair<bool, string> ConstructBuildingImmediately(string modelIdCaseSensitive, BuildableGameLocation location, Vector2 tileLocation)
        {
            if (String.IsNullOrEmpty(modelIdCaseSensitive) || SolidFoundations.buildingManager.DoesBuildingModelExist(modelIdCaseSensitive) is false)
            {
                return new KeyValuePair<bool, string>(false, $"No match for model {modelIdCaseSensitive}");
            }
            else if (location is null)
            {
                return new KeyValuePair<bool, string>(false, "BuildableGameLocation is null!");
            }

            var blueprint = new BluePrint(modelIdCaseSensitive);
            blueprint.daysToConstruct = 0;
            if (GameLocationPatch.AttemptToBuildStructure(location, blueprint, null, tileLocation, skipFarmerCheck: true) is false)
            {
                return new KeyValuePair<bool, string>(false, "Failed to construct structure, see log for details.");
            }

            return new KeyValuePair<bool, string>(true, $"Succesfully constructed {modelIdCaseSensitive} at {location.Name} on tile {tileLocation}!");
        }

        public KeyValuePair<bool, ExtendedBuildingModel> GetBuildingModel(Building building)
        {
            if (building is null || building is not GenericBuilding genericBuilding)
            {
                return new KeyValuePair<bool, ExtendedBuildingModel>(false, null);
            }

            return new KeyValuePair<bool, ExtendedBuildingModel>(true, genericBuilding.Model);
        }

        public KeyValuePair<bool, ExtendedBuildingModel> GetBuildingModel(string modelIdCaseSensitive)
        {
            if (String.IsNullOrEmpty(modelIdCaseSensitive) || SolidFoundations.buildingManager.DoesBuildingModelExist(modelIdCaseSensitive) is false)
            {
                return new KeyValuePair<bool, ExtendedBuildingModel>(false, null);
            }

            return new KeyValuePair<bool, ExtendedBuildingModel>(true, SolidFoundations.buildingManager.GetSpecificBuildingModel(modelIdCaseSensitive));
        }

        public bool UpdateModel(ExtendedBuildingModel buildingModel)
        {
            if (buildingModel is null || SolidFoundations.buildingManager.DoesBuildingModelExist(buildingModel.ID) is false)
            {
                return false;
            }

            return SolidFoundations.buildingManager.UpdateModel(buildingModel);
        }

        public KeyValuePair<bool, string> GetBuildingTexturePath(string modelIdCaseInsensitive)
        {
            if (String.IsNullOrEmpty(modelIdCaseInsensitive) || SolidFoundations.buildingManager.DoesBuildingModelExist(modelIdCaseInsensitive) is false)
            {
                return new KeyValuePair<bool, string>(false, null);
            }

            return new KeyValuePair<bool, string>(true, SolidFoundations.buildingManager.GetTextureAsset(modelIdCaseInsensitive.ToLower()));
        }

        public KeyValuePair<bool, Texture2D> GetBuildingTexture(string modelIdCaseSensitive)
        {
            if (String.IsNullOrEmpty(modelIdCaseSensitive) || SolidFoundations.buildingManager.DoesBuildingModelExist(modelIdCaseSensitive) is false)
            {
                return new KeyValuePair<bool, Texture2D>(false, null);
            }

            var attemptToGetTexturePath = GetBuildingTexturePath(modelIdCaseSensitive);
            if (attemptToGetTexturePath.Key is false)
            {
                return new KeyValuePair<bool, Texture2D>(false, null);
            }

            return new KeyValuePair<bool, Texture2D>(true, Game1.content.Load<Texture2D>(attemptToGetTexturePath.Value));
        }
    }
}
