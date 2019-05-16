using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace FarmTypeManager
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {
        ///<summary>Tasks performed when the mod initially loads.</summary>
        public override void Entry(IModHelper helper)
        {
            //tell SMAPI to run event methods when necessary
            Helper.Events.GameLoop.DayStarted += DayStarted;
            Helper.Events.GameLoop.DayEnding += DayEnding;
            Helper.Events.GameLoop.ReturnedToTitle += ReturnedToTitle;

            Utility.Monitor.IMonitor = Monitor; //pass the monitor for use by other areas of this mod's code

            //attempt to load the config.json ModConfig file and activate its settings
            try
            {
                Utility.MConfig = helper.ReadConfig<ModConfig>(); //create or load the config.json file

                if (Utility.MConfig.EnableWhereAmICommand == true) //if enabled, add the WhereAmI method as a console command
                {
                    helper.ConsoleCommands.Add("whereami", "Outputs coordinates and other information about the player's current location.", WhereAmI);
                }

                helper.WriteConfig<ModConfig>(Utility.MConfig); //update the config.json file (e.g. to add settings from new versions of the mod)
            }
            catch (Exception ex) //if the config.json file can't be parsed correctly, try to explain it in the user's log & then skip any config-related behaviors
            {
                Utility.Monitor.Log($"Warning: This mod's config.json file could not be parsed correctly. Related settings will be disabled. Please edit the file, or delete it and reload the game to generate a new config file. The auto-generated error message is displayed below:", LogLevel.Warn);
                Utility.Monitor.Log($"----------", LogLevel.Warn); //visual break to slightly improve clarity, based on user feedback
                Utility.Monitor.Log($"{ex.Message}", LogLevel.Warn);
            }
        }

        /// <summary>Tasks performed after the game begins a new day, including when loading a save.</summary>
        private void DayStarted(object sender, EventArgs e)
        {
            //attempt to load the config.json ModConfig file and update its settings
            //NOTE: this already happens in the Entry method, but doing it here allows certain settings to be changed while the game is running
            try
            {
                Utility.MConfig = Helper.ReadConfig<ModConfig>(); //create or load the config.json file

                Helper.WriteConfig<ModConfig>(Utility.MConfig); //update the config.json file (e.g. to add settings from new versions of the mod)
            }
            catch (Exception ex) //if the config.json file can't be parsed correctly, try to explain it in the user's log & then skip any config-related behaviors
            {
                Utility.Monitor.Log($"Warning: This mod's config.json file could not be parsed correctly. Some related settings will be disabled. Please edit the file, or delete it and reload the game to generate a new config file. The auto-generated error message is displayed below:", LogLevel.Warn);
                Utility.Monitor.Log($"----------", LogLevel.Warn); //visual break to slightly improve clarity, based on user feedback
                Utility.Monitor.Log($"{ex.Message}", LogLevel.Warn);
            }

            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything; most of this mod's functions should be limited to the host player

            Utility.LoadFarmData(Helper); //load all available data files

            //run the methods providing the mod's main features
            ObjectSpawner.ForageGeneration();
            ObjectSpawner.LargeObjectGeneration();
            ObjectSpawner.OreGeneration();
        }

        /// <summary>Tasks performed before a day ends, i.e. right before saving. This is also called when a new farm is created, *before* DayStarted.</summary>
        private void DayEnding(object sender, EventArgs e)
        {
            if (Utility.FarmDataList == null || Utility.FarmDataList.Count < 1) { return; } //if the farm data list is blank, do nothing (e.g. when called by a newly created farm)

            //update information in each save file
            foreach (FarmData data in Utility.FarmDataList)
            {
                data.Save.WeatherForYesterday = Utility.WeatherForToday(); //update saved weather info

                if (data.Pack != null) //if this data is from a content pack
                {
                    data.Pack.WriteJsonFile(Path.Combine("data", $"{Constants.SaveFolderName}_SaveData.save"), data.Save); //update the save file for that content pack
                }
                else //this data is from this mod's own folders
                {
                    Helper.Data.WriteJsonFile(Path.Combine("data", $"{Constants.SaveFolderName}_SaveData.save"), data.Save); //update the save file in this mod's own folders
                }
            }
    }

        /// <summary>Tasks performed when the player returns to the title screen from an active game session.</summary>
        private void ReturnedToTitle(object sender, EventArgs e)
        {
            Utility.FarmDataList = new List<FarmData>(); //clear this list to avoid any possible errors caused by a previous farm's data
        }

        ///<summary>Console command. Outputs the player's current location name, tile x/y coordinates, tile "Type" property (e.g. "Grass" or "Dirt"), tile "Diggable" status, and tile index.</summary>
        private void WhereAmI(string command, string[] args)
        {
            if (!Context.IsWorldReady) { return; } //if the player isn't in a fully loaded game yet, ignore this command

            GameLocation loc = Game1.currentLocation;
            int x = Game1.player.getTileX();
            int y = Game1.player.getTileY();
            int index = loc.getTileIndexAt(x, y, "Back");
            string type = loc.doesTileHaveProperty(x, y, "Type", "Back") ?? "[none]";
            string diggable = loc.doesTileHaveProperty(x, y, "Diggable", "Back");
            if (diggable == "T") { diggable = "Yes"; } else { diggable = "No"; };
            Monitor.Log($"Map name: {loc.Name}", LogLevel.Info);
			Monitor.Log($"Your location (x,y): {x},{y}", LogLevel.Info);
            Monitor.Log($"Terrain type: {type}", LogLevel.Info);
            Monitor.Log($"Diggable: {diggable}", LogLevel.Info);
            Monitor.Log($"Tile image index: {index}", LogLevel.Info);
        }
    }
}