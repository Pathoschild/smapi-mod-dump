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
            public static void LoadModConfig()
            {
                try
                {
                    MConfig = Helper.ReadConfig<ModConfig>(); //create or load the config.json file

                    Helper.WriteConfig(MConfig); //update the config.json file (e.g. to add settings from new versions of the mod)
                }
                catch (Exception ex) //if the config.json file can't be parsed correctly, try to explain it in the user's log & then skip any config-related behaviors
                {
                    MConfig = new ModConfig(); //use the default config settings to avoid errors

                    Monitor.Log($"This mod's config.json file could not be parsed correctly. The default settings will be used instead. Please edit the file, or delete it and restart the game to generate a new config file. The auto-generated error message is displayed below:", LogLevel.Warn);
                    Monitor.Log($"----------", LogLevel.Warn);
                    Monitor.Log($"{ex.Message}", LogLevel.Warn);
                }
            }
        }
    }
}