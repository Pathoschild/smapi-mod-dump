/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/clockworkhound/SDV-ChildBedConfig
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Linq;
using StardewValley.Locations;
using System.Collections.Generic;
using StardewValley.Buildings;

namespace ChildBedConfig
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        /*****************************/
        /**      Properties         **/
        /*****************************/
        ///<summary>The config file from the player</summary>
        private ModConfig Config;
        private Farmer Farmer;

        /*****************************/
        /**      Public methods     **/
        /*****************************/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            Farmer = new Farmer();
            Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }

        /// <summary>
        /// Modify the farmhouse tiles
        /// </summary>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs args)
        {
            GetFarmer();
            GameLocation farmhouse = Game1.getLocationFromName("FarmHouse");
            List<GameLocation> cabins = getCabins();

            //Check that the farmer has config set up, and that the house is upgraded to at least level 2
            if (Farmer.CharacterName != "NoName" && Game1.player.HouseUpgradeLevel > 1)
            {
                Monitor.Log("Crib and beds exist.  Removing them from map and replacing them with default tiles...");

                //Remove the crib
                if (!Farmer.ShowHomeCrib)
                {
                    removeTiles(farmhouse, "Front", 15, 2, 17, 4); //Get rid of the tiles from the front layer
                    removeTiles(farmhouse, "Buildings", 15, 3, 17, 5); //Get rid of the tiles from the building layer
                    
                    int wallpaper = farmhouse.getTileIndexAt(19, 3, "Buildings"); //get the wallpaper id so we can plaster it onto the tiles we're adding
                    int flooring = farmhouse.getTileIndexAt(19, 3, "Back"); //get the flooring id too

                    //Add wall tiles to 15 3, 16 3, and 17 3 on the Buildings layer, which were previously occupied by the crib
                    farmhouse.Map.GetLayer("Buildings").Tiles[15, 3] = new xTile.Tiles.StaticTile(farmhouse.Map.GetLayer("Buildings"), farmhouse.Map.GetTileSheet("walls_and_floors"), xTile.Tiles.BlendMode.Alpha, wallpaper);
                    farmhouse.Map.GetLayer("Buildings").Tiles[16, 3] = new xTile.Tiles.StaticTile(farmhouse.Map.GetLayer("Buildings"), farmhouse.Map.GetTileSheet("walls_and_floors"), xTile.Tiles.BlendMode.Alpha, wallpaper);
                    farmhouse.Map.GetLayer("Buildings").Tiles[17, 3] = new xTile.Tiles.StaticTile(farmhouse.Map.GetLayer("Buildings"), farmhouse.Map.GetTileSheet("walls_and_floors"), xTile.Tiles.BlendMode.Alpha, wallpaper);

                    //Add fooring tiles to the back layer where the crib was -- we'll have a weird black line there otherwise
                    farmhouse.Map.GetLayer("Back").Tiles[15, 3] = new xTile.Tiles.StaticTile(farmhouse.Map.GetLayer("Back"), farmhouse.Map.GetTileSheet("walls_and_floors"), xTile.Tiles.BlendMode.Alpha, flooring);
                    farmhouse.Map.GetLayer("Back").Tiles[16, 3] = new xTile.Tiles.StaticTile(farmhouse.Map.GetLayer("Back"), farmhouse.Map.GetTileSheet("walls_and_floors"), xTile.Tiles.BlendMode.Alpha, flooring);
                    farmhouse.Map.GetLayer("Back").Tiles[17, 3] = new xTile.Tiles.StaticTile(farmhouse.Map.GetLayer("Back"), farmhouse.Map.GetTileSheet("walls_and_floors"), xTile.Tiles.BlendMode.Alpha, flooring);
                }

                //Get rid of the bed closest to the crib
                if(!Farmer.ShowHomeBed1)
                {
                    removeTiles(farmhouse, "Buildings", 22, 4, 23, 6); //Get rid of the tiles from the building layer                    
                    removeTiles(farmhouse, "Front", 22, 3, 23, 5); //Get rid of the tiles from the front layer
                }

                //Get rid of the bed closest to the crib
                if (!Farmer.ShowHomeBed2)
                {
                    removeTiles(farmhouse, "Buildings", 26, 4, 27, 6); //Get rid of the tiles from the building layer                    
                    removeTiles(farmhouse, "Front", 26, 3, 27, 5); //Get rid of the tiles from the front layer
                }

                //If cabins exist on the farm, we go ahead and modify the children's beds there too
                if(cabins.Count > 0)
                {
                    foreach(GameLocation c in cabins)
                    {
                        Cabin cabin = (Cabin)c;

                        if(cabin.upgradeLevel > 1)
                        {
                            //Remove the crib
                            if (!Farmer.ShowCabinCrib)
                            {
                                removeTiles(cabin, "Buildings", 15, 3, 17, 5); //Get rid of the tiles from the building layer
                                removeTiles(cabin, "Front", 15, 2, 17, 4); //Get rid of the tiles from the front layer

                                int wallpaper = cabin.getTileIndexAt(18, 3, "Buildings"); //get the wallpaper id so we can plaster it onto the tiles we're adding
                                int flooring = cabin.getTileIndexAt(19, 3, "Back"); //get the flooring id too

                                //Add wall tiles to 15 3, 16 3, and 17 3 on the Buildings layer, which were previously occupied by the crib
                                cabin.Map.GetLayer("Buildings").Tiles[15, 3] = new xTile.Tiles.StaticTile(cabin.Map.GetLayer("Buildings"), cabin.Map.GetTileSheet("walls_and_floors"), xTile.Tiles.BlendMode.Alpha, wallpaper);
                                cabin.Map.GetLayer("Buildings").Tiles[16, 3] = new xTile.Tiles.StaticTile(cabin.Map.GetLayer("Buildings"), cabin.Map.GetTileSheet("walls_and_floors"), xTile.Tiles.BlendMode.Alpha, wallpaper);
                                cabin.Map.GetLayer("Buildings").Tiles[17, 3] = new xTile.Tiles.StaticTile(cabin.Map.GetLayer("Buildings"), cabin.Map.GetTileSheet("walls_and_floors"), xTile.Tiles.BlendMode.Alpha, wallpaper);

                                //Add fooring tiles to the back layer where the crib was -- we'll have a weird black line there otherwise
                                cabin.Map.GetLayer("Back").Tiles[15, 3] = new xTile.Tiles.StaticTile(cabin.Map.GetLayer("Back"), cabin.Map.GetTileSheet("walls_and_floors"), xTile.Tiles.BlendMode.Alpha, flooring);
                                cabin.Map.GetLayer("Back").Tiles[16, 3] = new xTile.Tiles.StaticTile(cabin.Map.GetLayer("Back"), cabin.Map.GetTileSheet("walls_and_floors"), xTile.Tiles.BlendMode.Alpha, flooring);
                                cabin.Map.GetLayer("Back").Tiles[17, 3] = new xTile.Tiles.StaticTile(cabin.Map.GetLayer("Back"), cabin.Map.GetTileSheet("walls_and_floors"), xTile.Tiles.BlendMode.Alpha, flooring);
                            }

                            //Get rid of the bed closest to the crib
                            if (!Farmer.ShowCabinBed1)
                            {
                                removeTiles(cabin, "Buildings", 22, 4, 23, 6); //Get rid of the tiles from the building layer                    
                                removeTiles(cabin, "Front", 22, 3, 23, 5); //Get rid of the tiles from the front layer
                            }

                            //Get rid of the bed closest to the crib
                            if (!Farmer.ShowCabinBed2)
                            {
                                removeTiles(cabin, "Buildings", 26, 4, 27, 6); //Get rid of the tiles from the building layer                    
                                removeTiles(cabin, "Front", 26, 3, 27, 5); //Get rid of the tiles from the front layer
                            }
                        }
                    }
                }
            }
            //If crib can't be found, the tiles are not touched
            else Monitor.Log("Could not find crib and beds.  No edits will be made.");
        }

        /// <summary>
        /// Find the name of the farmer in the active save file
        /// Needed to load options
        /// </summary>
        private void GetFarmer()
        {
            Farmer = new Farmer(); //clear the old data for the farmer

            //Loop through the list of farmers in config to find the current farmer
            for (int i = 0; i < Config.Farmers.Count; i++)
            {
                //If we find the farmer we make our Farmer object equal to this specific instance of Farmer
                if (Config.Farmers[i].CharacterName == Game1.player.Name)
                {
                    Farmer = Config.Farmers[i];
                    break;
                }
            }

            //Just output some info for the player
            if (Farmer.CharacterName.CompareTo("NoName") == 0)
            {
                Monitor.Log("No config information was found for this character.  No edits will be made to the map.", LogLevel.Info);
            }
            else
            {
                Monitor.Log("Config info found for " + Farmer.CharacterName + ".  Proceeding to edit map.");
            }
        }

        //Remove tiles from startX, startY to endX, endY
        private void removeTiles(GameLocation location, string layer, int startX, int startY, int endX, int endY)
        {
            for(int x = startX; x <= endX; x++)
            {
                for(int y = startY; y <= endY; y++)
                {
                    location.removeTile(x, y, layer);
                }
            }
        }

        public static List<GameLocation> getCabins()
        {
            List<GameLocation> list = Game1.locations.ToList();
            List<GameLocation> cabins = new List<GameLocation>();

            foreach (GameLocation location in Game1.locations)
                if (location is BuildableGameLocation bgl)
                    foreach (Building building in bgl.buildings)
                        if (building.indoors.Value != null)
                            list.Add(building.indoors.Value);

            foreach(GameLocation location in list)
            {
                if(location.Name == "Cabin")
                {
                    cabins.Add(location);
                }
            }

            return cabins;
        }
    }
}