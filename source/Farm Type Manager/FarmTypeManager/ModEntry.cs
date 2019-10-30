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
            Helper.Events.GameLoop.TimeChanged += TimeChanged;
            Helper.Events.GameLoop.DayEnding += DayEnding;
            Helper.Events.GameLoop.ReturnedToTitle += ReturnedToTitle;

            Utility.Monitor.IMonitor = Monitor; //pass the monitor for use by other areas of this mod's code

            Utility.LoadModConfig(helper); //attempt to load the config.json ModConfig file

            if (Utility.MConfig?.EnableWhereAmICommand == true) //if enabled, add the WhereAmI method as a console command
            {
                helper.ConsoleCommands.Add("whereami", "Outputs coordinates and other information about the player's current location.", WhereAmI);
            }
        }

        /// <summary>Tasks performed after the game begins a new day, including when loading a save.</summary>
        private void DayStarted(object sender, EventArgs e)
        {
            //attempt to load the config.json ModConfig file and update its settings
            //note: this already happens in the Entry method, but doing it here allows certain settings to be changed while the game is running
            Utility.LoadModConfig(Helper);

            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything; most of this mod's functions should be limited to the host player

            Utility.LoadFarmData(Helper); //load all available data files

            Monitor.Log($"Checking for saved objects that went missing overnight...", LogLevel.Trace);
            foreach (FarmData data in Utility.FarmDataList) //for each loaded set of data
            {
                if (data.Pack != null) //if this data is from a content pack
                {
                    Monitor.VerboseLog($"Checking objects from content pack: {data.Pack.Manifest.Name}");
                }
                else //this data is from this mod's own folders
                {
                    Monitor.VerboseLog($"Checking objects from FarmTypeManager/data/{Constants.SaveFolderName}_SaveData.save");
                }

                Utility.ReplaceProtectedSpawnsOvernight(data.Save); //protect unexpired spawns listed in the save data
            }

            Utility.TimedSpawns.Clear(); //clear any existing spawn data

            //run each generation process to fill the TimedSpawns list for today
            Generation.ForageGeneration();
            Generation.LargeObjectGeneration();
            Generation.OreGeneration();
            Generation.MonsterGeneration();

            Generation.SpawnTimedSpawns(Utility.TimedSpawns, 600); //spawn anything set to appear at 6:00AM
        }

        /// <summary>Tasks performed when the the game's clock time changes, i.e. every 10 in-game minutes. (Note: This event *sometimes* fires at 6:00AM: apaprently every load after the first.)</summary>
        private void TimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything; most of this mod's functions should be limited to the host player

            if (e.NewTime != 600) //if it's not currently 6:00AM
            {
                Generation.SpawnTimedSpawns(Utility.TimedSpawns, e.NewTime); //spawn anything set to appear at the current time
            }
        }

        /// <summary>Tasks performed before a day ends, i.e. right before saving. This is also called when a new farm is created, *before* DayStarted.</summary>
        private void DayEnding(object sender, EventArgs e)
        {
            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything; most of this mod's functions should be limited to the host player

            if (Utility.FarmDataList == null || Utility.FarmDataList.Count < 1) { return; } //if the farm data list is blank, do nothing (e.g. when called by a newly created farm)

            foreach (FarmData data in Utility.FarmDataList) //for each set of farm data
            {
                if (data.Pack != null) //if this data is from a content pack
                {
                    Monitor.VerboseLog($"Processing save data for content pack: {data.Pack.Manifest.Name}");
                }
                else //this data is from this mod's own folders
                {
                    Monitor.VerboseLog($"Processing save data for FarmTypeManager/data/{Constants.SaveFolderName}_SaveData.save");
                }

                Utility.ProcessObjectExpiration(data.Save); //remove expired objects & update saved expiration data

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
    }
}