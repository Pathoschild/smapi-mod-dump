/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Yariazen/BuildableGreenhouse
**
*************************************************/

using SolidFoundations.Framework.Models.Backport;
using SolidFoundations.Framework.Models.ContentPack;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.IO;

namespace BuildableGreenhouse.ModExtension
{
    public static partial class ModExtension
    {
        private static ISolidFoundationsApi SolidFoundationsApi;

        private static partial void SolidFoundationsExtension()
        {
            Helper.Events.GameLoop.GameLaunched += initializeModel;
            Helper.Events.Content.LocaleChanged += applyTranslations;
        }

        private static void initializeModel(object sender, GameLaunchedEventArgs e)
        {
            ExtendedBuildingModel GreenhouseBuilding = SolidFoundationsApi.GetBuildingModel("BuildableGreenhouse.Greenhouse").Value;
            if(Config.StartWithGreenhouse)
                GreenhouseBuilding.BuildCondition = null;
            else
                GreenhouseBuilding.BuildCondition = "PLAYER_HAS_FLAG Any ccPantry";
            
            if (Config.BuildCost < 0)
                GreenhouseBuilding.BuildCost = 0;
            else
                GreenhouseBuilding.BuildCost = Config.BuildCost;

            if (Config.BuildDays < 0)
                GreenhouseBuilding.BuildDays = 0;
            else
                GreenhouseBuilding.BuildDays = Config.BuildDays;

            GreenhouseBuilding.BuildMaterials = getBuildingMaterial(Config.BuildingDifficulty);
            applyTranslations(sender, e);
        }

        private static void applyTranslations(object sender, object e)
        {
            var blueprintDict = Helper.GameContent.Load<Dictionary<string, string>>("Data\\Blueprints");
            string[] greenhouseData = blueprintDict["Greenhouse"].Split("/");
            string buildableGreenhouseName = greenhouseData[8];
            string buildableGreenhouseDescription = greenhouseData[9];

            ExtendedBuildingModel GreenhouseBuilding = SolidFoundationsApi.GetBuildingModel("BuildableGreenhouse.Greenhouse").Value;
            GreenhouseBuilding.Name = buildableGreenhouseName;
            GreenhouseBuilding.Description = buildableGreenhouseDescription;
            updateModel(GreenhouseBuilding, true);
        }

        private static void updateModel(ExtendedBuildingModel GreenhouseBuilding, bool translations = false)
        {
            Monitor.Log($"{Manifest.UniqueID} Updating BuildCondition: {GreenhouseBuilding.BuildCondition}", LogLevel.Trace);
            Monitor.Log($"{Manifest.UniqueID} Updating BuildCost: {GreenhouseBuilding.BuildCost}", LogLevel.Trace);
            Monitor.Log($"{Manifest.UniqueID} Updating BuildDays: {GreenhouseBuilding.BuildDays}", LogLevel.Trace);
            Monitor.Log($"{Manifest.UniqueID} Updating BuildMaterials: {BuildingMaterialsToString(GreenhouseBuilding.BuildMaterials)}", LogLevel.Trace);
            if(translations)
            {
                Monitor.Log($"{Manifest.UniqueID} Updating Name: {GreenhouseBuilding.Name}", LogLevel.Trace);
                Monitor.Log($"{Manifest.UniqueID} Updating Description: {GreenhouseBuilding.Description}", LogLevel.Trace);
            }
            SolidFoundationsApi.UpdateModel(GreenhouseBuilding);
        }

        private static List<BuildingMaterial> getBuildingMaterial(int value)
        {
            List<BuildingMaterial> materials = null;
            BuildingMaterial material1 = new BuildingMaterial();
            BuildingMaterial material2 = new BuildingMaterial();
            BuildingMaterial material3 = new BuildingMaterial();
            switch (value)
            {
                case 1:
                    {
                        materials = new List<BuildingMaterial>();
                        material1.ItemID = "390"; //stone
                        material1.Amount = 500;
                        materials.Add(material1);

                        material2.ItemID = "388"; //wood
                        material2.Amount = 100;
                        materials.Add(material2);

                        material3.ItemID = "355"; //ironbar
                        material3.Amount = 5;
                        materials.Add(material3);
                    }
                    break;
                case 2:
                    {
                        materials = new List<BuildingMaterial>();
                        material1.ItemID = "390"; //stone
                        material1.Amount = 500;
                        materials.Add(material1);

                        material2.ItemID = "709"; //hardwood
                        material2.Amount = 100;
                        materials.Add(material2);

                        material3.ItemID = "337"; //ironbar
                        material3.Amount = 5;
                        materials.Add(material3);
                    }
                    break;
            }
            return materials;
        }

        private static string BuildingMaterialsToString(List<BuildingMaterial> buildingMaterials)
        {
            string result = "";
            if (buildingMaterials != null)
                foreach (BuildingMaterial material in buildingMaterials)
                    result += $"{material.ItemID}: {material.Amount}, ";
            else
                result += "None";
            
            return result;
        }
    }

    public interface ISolidFoundationsApi
    {
        /*
        public class BroadcastEventArgs : EventArgs
        {
            public string BuildingId { get; set; }
            public Building Building { get; set; }
            public Farmer Farmer { get; set; }
            public Point TriggerTile { get; set; }
            public string Message { get; set; }
        }
        */

        // event EventHandler<BroadcastEventArgs> BroadcastSpecialActionTriggered;
        // event EventHandler BeforeBuildingSerialization; // TODO: Mark this obsolete with SDV v1.6
        // event EventHandler AfterBuildingRestoration; // TODO: Mark this obsolete with SDV v1.6

        // public void AddBuildingFlags(Building building, List<string> flags, bool isTemporary = true);
        // public void RemoveBuildingFlags(Building building, List<string> flags);
        // public bool DoesBuildingHaveFlag(Building building, string flag);
        // public KeyValuePair<bool, ExtendedBuildingModel> GetBuildingModel(Building building);
        public KeyValuePair<bool, ExtendedBuildingModel> GetBuildingModel(string modelId);
        public bool UpdateModel(ExtendedBuildingModel buildingModel);
    }
}