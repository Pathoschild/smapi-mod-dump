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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities.Shops
{
    /*
     * TODO:             
     * //Need to load both textures and maps.
       //Need to edit the blueprints data file to include how it is built. Maybe make a BlueprintHelper.cs class? Or use BuildingBlueprintExtended.cs?
       //Need to add in the new buildings (StardewValley.Building) types themselves.
       //Need to code in the new game locations.
     * 
     */
    /// <summary>
    /// Deals with adding new buildings into the game.
    /// </summary>
    public class BuildingAssetLoader
    {



        /// <summary>
        /// See https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Content for API documentation.
        /// </summary>
        public void registerEvents()
        {
            RevitalizeModCore.ModHelper.Events.Content.AssetReady += this.Content_AssetReady;
            RevitalizeModCore.ModHelper.Events.Content.AssetRequested += this.Content_AssetRequested;
            RevitalizeModCore.ModHelper.Events.Content.AssetsInvalidated += this.Content_AssetsInvalidated;

            RevitalizeModCore.ModHelper.Events.Player.Warped += this.OnWarped;

            RevitalizeModCore.ModHelper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

            RevitalizeModCore.ModHelper.Events.Display.MenuChanged += this.OnMenuChanged;
        }

        /// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            this.FixWarps();
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

            BuildableGameLocation farm = e.NewLocation as BuildableGameLocation ?? e.OldLocation as BuildableGameLocation;
            if (farm != null)
            {
                for (int i = 0; i < farm.buildings.Count; ++i)
                {
                    var b = farm.buildings[i];
                    //NOTE: There may be a cleaner way to write this code to include many building types in a single check instead of a bunch of if statements.
                    /*
                    // This is probably a new building if it hasn't been converted yet.
                    if (b.buildingType.Value == "Shed2" && b is not BigShedBuilding)
                    {
                        //Log.Debug($"Converting big shed at ({b.tileX}, {b.tileY}) to actual big shed.");

                        farm.buildings[i] = new BigShedBuilding();
                        farm.buildings[i].buildingType.Value = b.buildingType.Value;
                        farm.buildings[i].daysOfConstructionLeft.Value = b.daysOfConstructionLeft.Value;
                        farm.buildings[i].indoors.Value = b.indoors.Value;
                        farm.buildings[i].tileX.Value = b.tileX.Value;
                        farm.buildings[i].tileY.Value = b.tileY.Value;
                        farm.buildings[i].tilesWide.Value = b.tilesWide.Value;
                        farm.buildings[i].tilesHigh.Value = b.tilesHigh.Value;
                        farm.buildings[i].load();
                    }
                    */
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

            //Examples to load assets.
            /*
            if (e.Name.IsEquivalentTo("Maps/ExtraCellar_Small"))
            {
                e.LoadFromModFile<xTile.Map>("ModAssets/Maps/Cellar/ExtraCellar_Small", StardewModdingAPI.Events.AssetLoadPriority.Medium);
            }
            */

            /*
            if (e.Name.IsEquivalentTo("Maps/ExtraCellar_Small"))
            {
                e.LoadFrom(() => {
                    //Return an asset provided in code instead of null!
                    return null;
                }, StardewModdingAPI.Events.AssetLoadPriority.Medium);
            }
            */

            //Example to edit an asset.
            /*
            if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectInformation"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<int, string>().Data;

                    foreach (int itemID in data.Keys.ToArray())
                    {
                        string[] fields = data[itemID].Split('/');
                        fields[1] = (int.Parse(fields[1]) * 2).ToString();
                        data[itemID] = string.Join("/", fields);
                    }
                });
            }
            */

            //Another building asset edit for blueprints.
            //Todo research this.
            //if(e.NameWithoutLocale.IsEquivalentTo("Data\\Blueprints")
            // asset.AsDictionary<string, string>().Data.Add("ExtraCellar_Small", $"337 25 390 999 388 999/7/3/3/2/-1/-1/ExtraCellar_Small/{I18n.ExtraCellar_Small_Name()}/{I18n.ExtraCellar_Small_Description()}/Buildings/none/96/96/20/null/Farm/250000/false");



            //throw new NotImplementedException();
        }

        private void Content_AssetReady(object sender, StardewModdingAPI.Events.AssetReadyEventArgs e)
        {

        }


        public void FixWarps()
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
            if (e.NewMenu is CarpenterMenu carp)
            {
                //var blueprints = this.Helper.Reflection.GetField<List<BluePrint>>(carp, "blueprints").GetValue();
                //blueprints.Add(new BluePrint("ExrtaCellar_Small"));
            }
        }
        


    }
}
