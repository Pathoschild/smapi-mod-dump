using System;
using System.Collections.Generic;
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
            ModConfig conf; //settings contained in the mod's config.json file

            //attempt to load and update/save the default.json FarmConfig file
            try
            {
                Utility.Config = Helper.Data.ReadJsonFile<FarmConfig>($"data/default.json"); //load the default.json config file (null if it doesn't exist)

                if (Utility.Config == null) //no default.json config file
                {
                    Utility.Config = new FarmConfig(); //load the (built-in) default config settings
                }

                Helper.Data.WriteJsonFile($"data/default.json", Utility.Config); //create or update the default.json config file
            }
            catch (Exception ex) //if there's an error while loading the json file, try to explain it in the user's log & then skip any further DayStarted behaviors
            {
                Utility.Monitor.Log($"Warning: Your default config file (default.json) could not be parsed correctly. If you load a character without their own config file, most of this mod's features will be disabled. Please edit the file, or delete it before restarting/loading to generate a new config file. The auto-generated error message is displayed below.", LogLevel.Warn);
                Utility.Monitor.Log($"----------", LogLevel.Warn); //visual break to slightly improve clarity, based on user feedback
                Utility.Monitor.Log($"{ex.Message}", LogLevel.Warn);
            }

            Utility.Config = null; //prevent errors later in the loading process

            //attempt to load the config.json ModConfig file and activate its settings
            try
            {
                conf = helper.ReadConfig<ModConfig>(); //create or load the config.json file

                if (conf.EnableWhereAmICommand == true) //if enabled, add the WhereAmI method as a console command
                {
                    helper.ConsoleCommands.Add("whereami", "Outputs coordinates and other information about the player's current location.", WhereAmI);
                }

                helper.WriteConfig<ModConfig>(conf); //update the config.json file (e.g. to add settings from new versions of the mod)
            }
            catch (Exception ex) //if the config.json file can't be parsed correctly, try to explain it in the user's log & then skip any config-related behaviors
            {
                Utility.Monitor.Log($"Warning: This mod's config.json file could not be parsed correctly. Related settings will be disabled. Please edit the file, or delete it and reload the game to generate a new config file. The original error message is displayed below.", LogLevel.Warn);
                Utility.Monitor.Log($"{ex.Message}", LogLevel.Warn);
            }
        }

        /// <summary>Tasks performed after the game begins a new day, including when loading a save.</summary>
        private void DayStarted(object sender, EventArgs e)
        {
            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything; most of this mod's functions should be limited to the host player

            Utility.Config = null; //avoid any errors elsewhere in the loading process
            try
            {
                Utility.Config = Helper.Data.ReadJsonFile<FarmConfig>($"data/{Constants.SaveFolderName}.json"); //load the current save's config file (null if it doesn't exist)
            }
            catch (Exception ex) //if there's an error while loading the json file, try to explain it in the user's log & then skip any further DayStarted behaviors
            {
                Utility.Monitor.Log($"Warning: Your character's config file ({Constants.SaveFolderName}.json) could not be parsed correctly. Most of this mod's features will be disabled. Please edit the file, or delete it and reload your save to generate a new config file. The auto-generated error message is displayed below.", LogLevel.Warn);
                Utility.Monitor.Log($"----------", LogLevel.Warn); //visual break to slightly improve clarity, based on user feedback
                Utility.Monitor.Log($"{ex.Message}", LogLevel.Warn);
                return;
            }

            if (Utility.Config == null) //no config file for this save
            {
                //attempt to load the default.json config file
                try
                {
                    Utility.Config = Helper.Data.ReadJsonFile<FarmConfig>($"data/default.json"); //load the default.json config file (null if it doesn't exist)
                }
                catch (Exception ex) //if there's an error while loading the json file, try to explain it in the user's log & then skip any further DayStarted behaviors
                {
                    Utility.Monitor.Log($"Warning: Your default config file (default.json) could not be parsed correctly, and your character doesn't have their own config file yet. Most of this mod's features will be disabled. Please edit the file, or delete it and reload your save to generate a new config file. The auto-generated error message is displayed below.", LogLevel.Warn);
                    Utility.Monitor.Log($"----------", LogLevel.Warn); //visual break to slightly improve clarity, based on user feedback
                    Utility.Monitor.Log($"{ex.Message}", LogLevel.Warn);
                    return;
                }

                if (Utility.Config == null) //no default.json config file
                {
                    Utility.Config = new FarmConfig(); //load the (built-in) default config settings
                }

                Helper.Data.WriteJsonFile($"data/default.json", Utility.Config); //create or update the default.json config file
            }

            Helper.Data.WriteJsonFile($"data/{Constants.SaveFolderName}.json", Utility.Config); //create or update the config file for the current save

            //run the methods providing the mod's main features
            ObjectSpawner.ForageGeneration();
            ObjectSpawner.LargeObjectGeneration();
            ObjectSpawner.OreGeneration();
        }

        /// <summary>Tasks performed before a day ends, i.e. right before saving. This is also called when a new farm is created, *before* DayStarted.</summary>
        private void DayEnding(object sender, EventArgs e)
        {
            if (Utility.Config != null) //avoid errors when this process runs prior to loading a config, e.g. when a new farm is created
            {
                //update the internal save data
                Utility.Config.Internal_Save_Data.WeatherForYesterday = Utility.WeatherForToday();

                //save any changes to the player's config file
                Helper.Data.WriteJsonFile($"data/{Constants.SaveFolderName}.json", Utility.Config);
            }
        }

        /// <summary>Tasks performed when the player returns to the title screen from an active game session.</summary>
        private void ReturnedToTitle(object sender, EventArgs e)
        {
            Utility.Config = null; //null the config file to avoid any related errors, e.g. re-using the data for a different save
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