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
using SolidFoundations.Framework.External.SpaceCore;
using SolidFoundations.Framework.Managers;
using SolidFoundations.Framework.Models.Buildings;
using SolidFoundations.Framework.Models.ContentPack;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SolidFoundations.Framework.Utilities
{
    internal static class SaveMigration
    {
        internal static void LoadBuildingCache(IMonitor monitor, BuildingManager buildingManager)
        {
            if (!Game1.IsMasterGame || String.IsNullOrEmpty(Constants.CurrentSavePath) || !File.Exists(Path.Combine(Constants.CurrentSavePath, "SolidFoundations", "buildings.json")))
            {
                return;
            }
            var customBuildingsExternalSavePath = Path.Combine(Constants.CurrentSavePath, "SolidFoundations", "buildings.json");

            // Get all known types for serialization
            Type[] knownTypes = Type.EmptyTypes;
            try
            {
                knownTypes = buildingManager.GetAllBuildingModels().Where(model => String.IsNullOrEmpty(model.IndoorMapType) is false && model.IndoorMapTypeAssembly.Equals("Stardew Valley", StringComparison.OrdinalIgnoreCase) is false).Select(model => Type.GetType($"{model.IndoorMapType},{model.IndoorMapTypeAssembly}")).ToArray();
                knownTypes = knownTypes.Concat(DGAIntegration.SpacecoreTypes).ToArray();
            }
            catch (Exception ex)
            {
                monitor.Log($"Failed to get known types, some buildings may fail to load: {ex}", LogLevel.Trace);
            }
            bool didLoadWithoutErrors = false;

            var externallySavedCustomBuildings = new List<GenericBuilding>();
            try
            {
                // Get the externally saved custom building objects
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<GenericBuilding>), knownTypes);

                using (StreamReader textReader = new StreamReader(customBuildingsExternalSavePath))
                {
                    externallySavedCustomBuildings = (List<GenericBuilding>)xmlSerializer.Deserialize(textReader);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    monitor.Log($"Failed initial loading of custom buildings: {ex}", LogLevel.Trace);
                    monitor.Log($"Attempting to remove unknown indoor types from the cached buildings...", LogLevel.Trace);

                    // Load the XML and remove any nodes with the xsi:type of "Mods_..."
                    XDocument doc = XDocument.Load(customBuildingsExternalSavePath);
                    var invalidNodes = doc.Descendants().Where(n => n.Name.LocalName == "indoors" && n.Attributes().FirstOrDefault(y => y.Name.LocalName == "type") is var nodeType && nodeType is not null && Toolkit.IsKnownVanillaIndoorType(nodeType.Value) is false).Select(n => n).ToList();

                    foreach (var node in invalidNodes)
                    {
                        monitor.Log($"Removing building with indoor type of {node.Attributes().FirstOrDefault(y => y.Name.LocalName == "type").Value}", LogLevel.Trace);
                        node.Remove();
                    }

                    // Attempt the secondary deserialization
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<GenericBuilding>), knownTypes);
                    externallySavedCustomBuildings = (List<GenericBuilding>)xmlSerializer.Deserialize(doc.CreateReader());

                    monitor.Log($"Removed {invalidNodes.Count} cached building(s) with invalid indoor types! See log for details.", LogLevel.Warn);
                    monitor.Log($"Cleanup of the cached buildings was successful!", LogLevel.Trace);

                    didLoadWithoutErrors = true;
                }
                catch (Exception secondaryEx)
                {
                    try
                    {
                        monitor.Log($"Failed second attempt at loading of custom buildings: {ex}", LogLevel.Trace);
                        monitor.Log($"Attempting to remove SpaceCore-related items from the cached buildings...", LogLevel.Trace);

                        // Load the XML and remove any nodes with the xsi:type of "Mods_..."
                        XDocument doc = XDocument.Load(customBuildingsExternalSavePath);
                        var invalidNodes = doc.Descendants().Where(n => n.Attributes().FirstOrDefault(y => y.Name.LocalName == "type") is var nodeType && nodeType is not null && nodeType.Value.Contains("Mods_", StringComparison.OrdinalIgnoreCase)).Select(n => n).ToList();

                        foreach (var node in invalidNodes)
                        {
                            monitor.Log($"Removing nodes prefixed with \"Mods_\" {node.Attributes().FirstOrDefault(y => y.Name.LocalName == "type").Value}", LogLevel.Trace);
                            node.Remove();
                        }

                        // Attempt the tertiary deserialization
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<GenericBuilding>), knownTypes);
                        externallySavedCustomBuildings = (List<GenericBuilding>)xmlSerializer.Deserialize(doc.CreateReader());

                        monitor.Log($"Removed {invalidNodes.Count} cached building(s) with invalid class types! See log for details.", LogLevel.Warn);
                        monitor.Log($"Cleanup of the cached buildings was successful!", LogLevel.Trace);

                        didLoadWithoutErrors = true;
                    }
                    catch (Exception tertiaryEx)
                    {
                        monitor.Log("Failed to load the cached Solid Foundation data: No custom buildings will be loaded!", LogLevel.Warn);
                        monitor.Log($"Failed to load the cached Solid Foundation data: {tertiaryEx}", LogLevel.Trace);

                        DisplayCacheWarning(monitor);
                        return;
                    }
                }
            }

            // Process each buildable location and restore any installed custom buildings
            foreach (GameLocation buildableLocation in Game1.locations.Where(l => l is GameLocation && l.modData.ContainsKey(ModDataKeys.LOCATION_CUSTOM_BUILDINGS)).ToList())
            {
                // Get the archived custom building data for this location
                var archivedBuildingsData = JsonSerializer.Deserialize<List<ArchivedBuildingData>>(buildableLocation.modData[ModDataKeys.LOCATION_CUSTOM_BUILDINGS]);

                // Go through each ArchivedBuildingData to confirm that a) the Id exists via BuildingManager and b) there is a match in locationNamesToCustomBuildings to its Id and TileLocation
                foreach (var archivedData in archivedBuildingsData)
                {
                    try
                    {
                        if (!buildingManager.DoesBuildingModelExist(archivedData.Id) || !externallySavedCustomBuildings.Any(b => b.LocationName == buildableLocation.NameOrUniqueName))
                        {
                            continue;
                        }

                        GenericBuilding customBuilding = externallySavedCustomBuildings.FirstOrDefault(b => b.Id == archivedData.Id && b.tileX == archivedData.TileX && b.tileY == archivedData.TileY);
                        if (customBuilding is null)
                        {
                            continue;
                        }

                        Building vanillaBuilding = Building.CreateInstanceFromId(customBuilding.Id, new Vector2(customBuilding.tileX, customBuilding.tileY));
                        vanillaBuilding.skinId.Value = customBuilding.skinID.Value;
                        vanillaBuilding.daysOfConstructionLeft.Value = 0;
                        vanillaBuilding.daysUntilUpgrade.Value = 0;
                        vanillaBuilding.indoors.Value = customBuilding.indoors;
                        customBuilding.buildingChests.ForEach(vanillaBuilding.buildingChests.Add);
                        if (MigrateCachedCustomBuildingForLocation(monitor, buildableLocation, vanillaBuilding) is false)
                        {
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        monitor.Log($"Failed to load cached custom building {archivedData.Id} at [{archivedData.TileX}, {archivedData.TileY}] within the map {buildableLocation.NameOrUniqueName}! See log for details.", LogLevel.Warn);
                        monitor.Log($"Failure to load the custom building: {ex}", LogLevel.Trace);

                        didLoadWithoutErrors = true;
                    }
                }
            }

            // Display one time warning that any buildings which failed to load will be lost upon saving
            if (didLoadWithoutErrors)
            {
                DisplayCacheWarning(monitor);
            }
        }

        private static void DisplayCacheWarning(IMonitor monitor)
        {
            monitor.Log("There were errors loading the cached Solid Foundation buildings. Any buildings which have failed to load will be lost after saving the game.", LogLevel.Alert);
        }

        private static bool MigrateCachedCustomBuildingForLocation(IMonitor monitor, GameLocation buildableLocation, Building customBuilding)
        {
            try
            {
                GameLocation interior = null;
                if (customBuilding.indoors.Value is not null)
                {
                    interior = customBuilding.indoors.Value;
                    if (Game1.locations.Contains(interior) is false && buildableLocation is not Farm)
                    {
                        //Game1.locations.Add(interior);
                    }
                }

                // Set the location
                customBuilding.parentLocationName.Value = buildableLocation.NameOrUniqueName;
                // Load the building
                customBuilding.load();
                // Establish any buildings within this building
                if (interior is GameLocation subBuildableLocation && subBuildableLocation is not null && subBuildableLocation.buildings is not null)
                {
                    foreach (var subBuilding in subBuildableLocation.buildings.ToList())
                    {
                        if (subBuilding is Building subCustomBuilding)
                        {
                            subBuildableLocation.buildings.Remove(subCustomBuilding);
                            MigrateCachedCustomBuildingForLocation(monitor, subBuildableLocation, subCustomBuilding);
                            continue;
                        }
                        // Handle vanilla buildings
                        subBuilding.load();
                        if (subBuilding.indoors.Value is not null && subBuilding.indoors.Value.warps is not null)
                        {
                            foreach (Warp warp in subBuilding.indoors.Value.warps)
                            {
                                warp.TargetName = subBuildableLocation.NameOrUniqueName;
                            }
                        }
                    }
                }
                // Restore the archived custom building
                buildableLocation.buildings.Add((Building)customBuilding);
                // Clear any grass and other debris
                var validIndexesForRemoval = new List<int>()
                {
                    343,
                    450,
                    294,
                    295,
                    675,
                    674,
                    784,
                    677,
                    676,
                    785,
                    679,
                    678,
                    786,
                    674
                };
                for (int x = 0; x < customBuilding.tilesWide.Value; x++)
                {
                    for (int y = 0; y < customBuilding.tilesHigh.Value; y++)
                    {
                        var targetTile = new Vector2(customBuilding.tileX.Value + x, customBuilding.tileY.Value + y);
                        if (buildableLocation.terrainFeatures.ContainsKey(targetTile) && buildableLocation.terrainFeatures[targetTile] is Grass grass && grass is not null)
                        {
                            buildableLocation.terrainFeatures.Remove(targetTile);
                        }
                        else if (buildableLocation.terrainFeatures.ContainsKey(targetTile) && buildableLocation.terrainFeatures[targetTile] is Tree tree && tree is not null)
                        {
                            buildableLocation.terrainFeatures.Remove(targetTile);
                        }
                        else if (buildableLocation.objects.ContainsKey(targetTile) && buildableLocation.objects[targetTile] is StardewValley.Object obj && obj is not null && validIndexesForRemoval.Contains(obj.ParentSheetIndex))
                        {
                            buildableLocation.objects.Remove(targetTile);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                monitor.Log($"Failed to setup cached custom building {customBuilding.buildingType.Value} at [{customBuilding.tileX}, {customBuilding.tileY}] within the map {buildableLocation.NameOrUniqueName}! See log for details.", LogLevel.Warn);
                monitor.Log($"Failed to setup cached custom building {customBuilding.buildingType.Value} at [{customBuilding.tileX}, {customBuilding.tileY}] within the map {buildableLocation.NameOrUniqueName}: {ex}", LogLevel.Trace);
                return false;
            }
            return true;
        }

        internal static void RetireBuildingCache()
        {
            if (!Game1.IsMasterGame || String.IsNullOrEmpty(Constants.CurrentSavePath))
            {
                return;
            }
            var externalSaveFolderPath = Path.Combine(Constants.CurrentSavePath, "SolidFoundations");

            // Cache the building.json, in case we need to restore it
            if (string.IsNullOrEmpty(Constants.CurrentSavePath) is false && File.Exists(Path.Combine(externalSaveFolderPath, "buildings.json")))
            {
                string cachedBuildingPath = Path.Combine(externalSaveFolderPath, "buildings_old.json");
                File.Move(Path.Combine(externalSaveFolderPath, "buildings.json"), cachedBuildingPath, true);

                SolidFoundations.monitor.Log("Retired building cache to buildings_old.json; Cached building restoration will not occur again.", LogLevel.Trace);
            }
        }
    }
}
