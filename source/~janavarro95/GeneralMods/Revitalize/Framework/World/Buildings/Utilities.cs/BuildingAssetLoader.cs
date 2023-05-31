/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Constants.Ids.Buildings;
using Omegasis.Revitalize.Framework.Constants.Ids.GameLocations;
using Omegasis.Revitalize.Framework.Constants.PathConstants;
using Omegasis.Revitalize.Framework.Constants.PathConstants.Data;
using Omegasis.Revitalize.Framework.Managers;
using Omegasis.Revitalize.Framework.Player;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.World.Buildings;
using Omegasis.Revitalize.Framework.World.Buildings.Utilities.cs;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using static Omegasis.Revitalize.Framework.Constants.Enums;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities
{
    /// <summary>
    /// Deals with adding new buildings into the game.
    ///
    /// To add a new building a few things are needed.
    /// 1. A new BlueprintHelper.json file (renamed for the building) loacated in ModAssets/Data/BuildingBlueprintHelpers. These will be auto loaded and can be deeply nested.
    /// 2. A new graphic located at ModAsserts/Graphics/Buildings/... These will be auto loaded into the <see cref="TextureManagers.Buildings"/> texture manager for use by file name.
    /// 3. Display strings located in the mod's content pack. Located in {ContentPack}/ModAssets/Strings/Buildings/DisplayStrings. File name doesn't matter and will be auto laoded. Can also be deeply nested.
    /// 4. Add a new id to building type to <see cref="BuildingTypeToCSharpType"/>
    /// 5. Make the new Building c# class.
    /// 5.5. Need to update: Content_AssetRequested field. Might be able to rewrite this later.
    /// 6. (Optional) A .tbin map file (Optional, use only if building has indoors location). Maps will be stored in ModAssets/Maps/
    /// 7. (Optional) Make the new GameLocation c# class. (Not necessary for buildings you interact with.
    /// </summary>
    public class BuildingAssetLoader
    {
        /// <summary>
        /// Holds a reference from a building type to the c# equivalent code type. This is how the actual structure and it's internal location are created.
        /// </summary>
        public static Dictionary<string, Type> BuildingTypeToCSharpType = new Dictionary<string, Type>()
            {
                {BuildingIds.ExtraCellar,typeof(ExtraCellarBuilding) },
                {BuildingIds.DimensionalStorageUnit,typeof(DimensionalStorageUnitBuilding) }
            };

        /// <summary>
        /// All of the loaded blueprints for the mod.
        /// </summary>
        public static Dictionary<string, BlueprintHelper> Blueprints = new Dictionary<string, BlueprintHelper>();

        /// <summary>
        /// See https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Content for API documentation.
        /// </summary>
        public void registerEvents()
        {
            RevitalizeModCore.ModHelper.Events.Content.AssetReady += this.Content_AssetReady;
            RevitalizeModCore.ModHelper.Events.Content.AssetRequested += this.Content_AssetRequested;
            RevitalizeModCore.ModHelper.Events.Content.AssetsInvalidated += this.Content_AssetsInvalidated;

            RevitalizeModCore.ModHelper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;

            RevitalizeModCore.ModHelper.Events.Player.Warped += this.OnWarped;

            RevitalizeModCore.ModHelper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

            RevitalizeModCore.ModHelper.Events.Display.MenuChanged += this.OnMenuChanged;

            //BlueprintHelper helper = new BlueprintHelper();
            //JsonUtilities.WriteJsonFile(helper, DataPaths.TemplatesPath, "JsonBlueprintHelperTemplate.json");
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            List<BlueprintHelper> blueprintHelpers = JsonUtilities.LoadJsonFilesFromDirectories<BlueprintHelper>(DataPaths.BulidingBlueprintHelpers);

            foreach (BlueprintHelper helper in blueprintHelpers)
            {
                if (!Blueprints.ContainsKey(helper.revitalizeBuildingId))
                {
                    Blueprints.Add(helper.revitalizeBuildingId, helper);
                }
                else
                {
                    Revitalize.RevitalizeModCore.logWarning("Didn't add duplicated building key: " + helper.revitalizeBuildingId);
                }
            }
        }

        private void Content_AssetsInvalidated(object sender, StardewModdingAPI.Events.AssetsInvalidatedEventArgs e)
        {

        }

        private void Content_AssetRequested(object sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
        {
            //Need to load both textures and maps.
            //Need to edit the blueprints data file to include how it is built.
            //Need to add in the new buildings themselves.
            //Need to code in the new game locations.

            if (e.NameWithoutLocale.IsEquivalentTo("Data\\Blueprints"))
                e.Edit(this.EditBluePrints);

            //Load all building graphics.
            foreach(string s in Blueprints.Keys)
            {
                if (e.NameWithoutLocale.IsEquivalentTo(GetBuildingsAssetName(s)))
                {
                    //Graphic name should be the last part of the building id for simplicity.
                    e.LoadFrom(() => TextureManagers.Buildings.getTexture(s.Split(".").Last()), AssetLoadPriority.Exclusive);
                }
            }

            //Find a way to maybe automate loading maps as well?? Depends on how many of these I need to make.
            //Need to check both ids foer the extra celler here since I accidentally changed this afterwards so this is to prevent losing a save file....
            if (e.NameWithoutLocale.IsEquivalentTo(GetMapsAssetName(BuildingIds.ExtraCellar)) || e.NameWithoutLocale.IsEquivalentTo(GetMapsAssetName(GameLocationIds.ExtraCellar)))
                e.LoadFromModFile<xTile.Map>(this.getMapFileFromName("ExtraCellar.tbin"), AssetLoadPriority.Exclusive);
        }

        private string getMapFileFromName(string MapName)
        {
            return Path.Combine(RelativePaths.ModAssets_Maps_Folder, MapName);
        }

        public static string GetBuildingsAssetName(string Id)
        {
            return string.Format("Buildings\\{0}", Id);
        }

        public static string GetMapsAssetName(string Id)
        {
            return string.Format("Maps\\{0}", Id);
        }

        private void Content_AssetReady(object sender, StardewModdingAPI.Events.AssetReadyEventArgs e)
        {

        }



        /// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {

            //cache them in the mods' data.

            this.fixWarps();
        }

        /// <summary>
        /// Converts buildings to their actual building vriants. Credits to SpaceChase0 for figuring this out for his MoreBuildingsMod https://github.com/spacechase0/StardewValleyMods/blob/develop/MoreBuildings/Mod.cs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (!e.IsLocalPlayer)
                return;

            BuildableGameLocation loc = e.NewLocation as BuildableGameLocation ?? e.OldLocation as BuildableGameLocation;
            if (loc != null)
            {
                for (int i = 0; i < loc.buildings.Count; ++i)
                {
                    foreach (KeyValuePair<string, Type> pair in BuildingTypeToCSharpType)
                    {
                        this.updateFarmBuilding<Building>(loc, i, pair.Key, pair.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Updates a farm's building type to be the actual value type from c# code. To support additional types, update <see cref="BuildingTypeToCSharpType"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="location"></param>
        /// <param name="farmBuildingIndex"></param>
        /// <param name="DesiredBuildingType"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool updateFarmBuilding<T>(BuildableGameLocation location, int farmBuildingIndex, string DesiredBuildingType, Type type)
        {
            var b = location.buildings[farmBuildingIndex];
            if (b.buildingType.Value == DesiredBuildingType && !TypeUtilities.IsSameType(b.GetType(), type))
            {
                Game1.showRedMessage("Converting building!");
                location.buildings[farmBuildingIndex] = (StardewValley.Buildings.Building)Activator.CreateInstance(type);
                location.buildings[farmBuildingIndex].buildingType.Value = b.buildingType.Value;
                location.buildings[farmBuildingIndex].daysOfConstructionLeft.Value = b.daysOfConstructionLeft.Value;
                location.buildings[farmBuildingIndex].indoors.Value = b.indoors.Value;
                location.buildings[farmBuildingIndex].tileX.Value = b.tileX.Value;
                location.buildings[farmBuildingIndex].tileY.Value = b.tileY.Value;
                location.buildings[farmBuildingIndex].tilesWide.Value = b.tilesWide.Value;
                location.buildings[farmBuildingIndex].tilesHigh.Value = b.tilesHigh.Value;
                location.buildings[farmBuildingIndex].load();
                return true;
            }
            return false;
        }


        private void fixWarps()
        {
            foreach (var loc in Game1.locations)
            {
                if (loc is BuildableGameLocation buildable)
                {
                    foreach (var building in buildable.buildings)
                    {
                        if (building.indoors.Value == null)
                            continue;


                        building.indoors.Value.updateWarps();
                        building.updateInteriorWarps();
                    }
                }
            }
        }


        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is CarpenterMenu carpenterMenu)
            {
                var blueprints = Revitalize.RevitalizeModCore.ModHelper.Reflection.GetField<List<BluePrint>>(carpenterMenu, "blueprints").GetValue();
                foreach(string s in Blueprints.Keys)
                {
                    //Can add special conditional logic for hiding specific blueprints.
                    if (s.Equals(BuildingIds.DimensionalStorageUnit) && !BuildingUtilities.HasBuiltDimensionalStorageUnitOnFarm() && PlayerUtilities.GetNumberOfGoldenWalnutsFound()>=100)
                    {
                        blueprints.Add(new BluePrint(s));
                        continue;
                    }
                    if(s.Equals(BuildingIds.ExtraCellar) && Game1.player.HouseUpgradeLevel >= 2)
                    {
                        blueprints.Add(new BluePrint(s));
                        continue;
                    }
                    blueprints.Add(new BluePrint(s));
                }
            }
        }

        public void EditBluePrints(IAssetData asset)
        {
            if (Blueprints == null) return;

            foreach (var kvp in Blueprints)
            {
                asset.AsDictionary<string, string>().Data.Add(kvp.Key, kvp.Value.toBlueprintString());
            }
        }

    }
}
