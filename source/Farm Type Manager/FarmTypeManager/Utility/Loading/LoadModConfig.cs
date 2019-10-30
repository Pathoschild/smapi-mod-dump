using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Attempts to load the config.json file into Utility.MConfig.</summary>
            /// <param name="helper">The IModHelper interface provided by SMAPI.</param>
            public static void LoadModConfig(IModHelper helper)
            {
                try
                {
                    MConfig = helper.ReadConfig<ModConfig>(); //create or load the config.json file

                    helper.WriteConfig(MConfig); //update the config.json file (e.g. to add settings from new versions of the mod)
                }
                catch (Exception ex) //if the config.json file can't be parsed correctly, try to explain it in the user's log & then skip any config-related behaviors
                {
                    Monitor.Log($"Warning: This mod's config.json file could not be parsed correctly. Some related settings will be disabled. Please edit the file, or delete it and reload the game to generate a new config file. The auto-generated error message is displayed below:", LogLevel.Warn);
                    Monitor.Log($"----------", LogLevel.Warn);
                    Monitor.Log($"{ex.Message}", LogLevel.Warn);

                    MConfig = null; //clear MConfig to avoid using old data
                }
            }
        }
    }
}