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
            Helper.Events.GameLoop.DayStarted += DayStarted; //tell SMAPI to run the DayStarted event when necessary
            Utility.Monitor.IMonitor = Monitor; //pass the monitor for use by other areas of this mod's code

            ModConfig conf; //settings contained in the mod's config.json file
            try
            {
                conf = helper.ReadConfig<ModConfig>(); //create or load the config.json file
            }
            catch (Exception ex) //if the config.json file can't be parsed correctly, try to explain it in the user's log & then skip any config-related behaviors
            {
                Utility.Monitor.Log($"Warning: This mod's config.json file could not be parsed correctly. Related settings will be disabled. Please edit the file, or delete it and reload the game to generate a new config file. The original error message is displayed below.", LogLevel.Warn);
                Utility.Monitor.Log($"{ex.Message}", LogLevel.Warn);
                return;
            }

            if (conf.EnableWhereAmICommand == true) //if enabled, add the WhereAmI method as a console command
            {
                helper.ConsoleCommands.Add("whereami", "Outputs coordinates and other information about the player's current location.", WhereAmI);
            }
        }

        /// <summary>Tasks performed after the game begins a new day, including when loading a save.</summary>
        private void DayStarted(object sender, EventArgs e)
        {
            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything; most of this mod's functions should be limited to the host player

            try
            {
                Utility.Config = Helper.Data.ReadJsonFile<FarmConfig>($"data/{Constants.SaveFolderName}.json"); //load the current save's config file ([null] if it doesn't exist)
            }
            catch (Exception ex) //if there's an error while loading the json file, try to explain it in the user's log & then skip any further DayStarted behaviors
            {
                Utility.Monitor.Log($"Warning: Your character's config file could not be parsed correctly. Most of this mod's functions will be disabled. Please edit the file, or delete it and reload your save to generate a new config file. The original error message is displayed below.", LogLevel.Warn);
                Utility.Monitor.Log($"{ex.Message}", LogLevel.Warn);
                return;
            }

            if (Utility.Config == null) //no config file for this save?
            {
                Utility.Config = new FarmConfig(); //load the default config settings
                Helper.Data.WriteJsonFile($"data/{Constants.SaveFolderName}.json", Utility.Config); //create a config file for the current save
            }

            //run the methods providing the mod's main features
            ObjectSpawner.ForageGeneration();
            ObjectSpawner.LargeObjectGeneration();
            ObjectSpawner.OreGeneration();

            //NOTE: This will reformat any changes the player has made with the default SMAPI formatting
            Helper.Data.WriteJsonFile($"data/{Constants.SaveFolderName}.json", Utility.Config);
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