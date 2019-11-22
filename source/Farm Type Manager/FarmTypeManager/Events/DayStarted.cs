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
    public partial class ModEntry : Mod
    {
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
    }
}
