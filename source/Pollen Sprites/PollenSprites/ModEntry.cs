using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

namespace PollenSprites
{
    /// <summary>The class representing this mod to SMAPI and other mods.</summary>
    public class ModEntry : Mod
    {
        /// <summary>This mod's current instance, used for easy access by other classes.</summary>
        public static Mod Instance { get; set; } = null;

        /// <summary>This mod's config.json file settings.</summary>
        public static ModConfig ModConfig { get; set; } = null;

        /// <summary>This mod's entry point.</summary>
        public override void Entry(IModHelper helper)
        {
            Instance = this; //set the static instance of this mod

            try
            {
                ModConfig = helper.ReadConfig<ModConfig>(); //try to load config.json
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to load config.json file. The default settings will be used instead.", LogLevel.Warn);
                Monitor.Log($"Auto-generated error message:", LogLevel.Warn);
                Monitor.Log(ex.Message, LogLevel.Warn);
            }

            if (ModConfig == null) //if config.json didn't load
                ModConfig = new ModConfig(); //use the default version

            helper.Events.GameLoop.GameLaunched += GMCM.EnableGMCM; //enable GMCM's compatibility event
            helper.Events.GameLoop.ReturnedToTitle += SeedManager.GameLoop_ReturnedToTitle_ClearAllSeedsList; //enable SeedManager's list-clearing event
        }
    }
}
