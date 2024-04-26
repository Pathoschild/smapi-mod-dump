/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/PollenSprites
**
*************************************************/

using StardewModdingAPI;
using System;

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

            helper.Events.GameLoop.GameLaunched += GMCM.Initialize; //enable GMCM's compatibility event
            helper.Events.GameLoop.ReturnedToTitle += SeedManager.GameLoop_ReturnedToTitle_ClearAllSeedsList; //enable SeedManager's list-clearing event
        }
    }
}
