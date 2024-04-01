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
using SolidFoundations.Framework.Extensions;
using SolidFoundations.Framework.Utilities;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;

namespace SolidFoundations.Framework.Managers
{
    internal class LightManager
    {
        private IMonitor _monitor;
        private IModHelper _helper;

        private Dictionary<Guid, List<LightSource>> _buildingGuidsToLights;

        public LightManager(IMonitor monitor, IModHelper helper)
        {
            _monitor = monitor;
            _helper = helper;

            _buildingGuidsToLights = new Dictionary<Guid, List<LightSource>>();
        }

        internal List<LightSource> GetLightSources(Building building)
        {
            if (building is null || _buildingGuidsToLights.ContainsKey(building.id.Value) is false)
            {
                return new List<LightSource>();
            }

            return _buildingGuidsToLights[building.id.Value];
        }

        internal void UpdateLights(Building building, GameLocation gameLocation, GameTime time)
        {
            if (building is null || _buildingGuidsToLights.ContainsKey(building.id.Value) is false || SolidFoundations.buildingManager.DoesBuildingModelExist(building.buildingType.Value) is false)
            {
                return;
            }
            var extendedModel = SolidFoundations.buildingManager.GetSpecificBuildingModel(building.buildingType.Value);

            if (extendedModel.Lights is not null && gameLocation is not null && gameLocation.sharedLights is not null)
            {
                var startingTile = new Point(building.tileX.Value, building.tileY.Value);

                // Add the required lights
                int lightCount = 0;
                foreach (var lightModel in extendedModel.Lights)
                {
                    lightCount++;

                    lightModel.ElapsedMilliseconds += time.ElapsedGameTime.Milliseconds;
                    if (lightModel.ElapsedMilliseconds < lightModel.GetUpdateInterval())
                    {
                        continue;
                    }
                    lightModel.GetUpdateInterval(recalculateValue: true);
                    lightModel.ElapsedMilliseconds = 0f;

                    int lightIdentifier = Toolkit.GetLightSourceIdentifierForBuilding(startingTile, lightCount);
                    if (gameLocation.hasLightSource(lightIdentifier) is false)
                    {
                        continue;
                    }

                    gameLocation.sharedLights[lightIdentifier].radius.Value = lightModel.GetRadius();
                }
            }
        }

        internal void ClearLights(Building building, GameLocation gameLocation)
        {
            if (building is null)
            {
                return;
            }

            foreach (var lightSource in building.GetLightSources())
            {
                if (gameLocation.hasLightSource(lightSource.Identifier))
                {
                    gameLocation.removeLightSource(lightSource.Identifier);
                }
            }

            if (_buildingGuidsToLights.ContainsKey(building.id.Value))
            {
                _buildingGuidsToLights[building.id.Value].Clear();
            }
        }

        internal void SetLightSources(Building building, List<LightSource> lightSources)
        {
            // Add the existing lights
            _buildingGuidsToLights[building.id.Value] = lightSources;
        }
    }
}
