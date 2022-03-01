/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/EscasModdingPlugins
**
*************************************************/

using StardewModdingAPI;
using System;

namespace EscasModdingPlugins
{
    /// <summary>The available user configuration settings.</summary>
    public class ModConfig
    {
        /// <summary>The currently active user configuration settings.</summary>
        public static ModConfig Instance { get; set; } = null;

        /// <summary>Loads <see cref="Instance"/> from the user's config.json file.</summary>
        /// <param name="helper">The helper instance to use for API access.</param>
        /// <param name="monitor">The monitor instance to use for console/log messages.</param>
        public static void Initialize(IModHelper helper, IMonitor monitor)
        {
            try
            {
                Instance = helper.ReadConfig<ModConfig>();
            }
            catch (Exception ex)
            {
                Instance = new ModConfig(); //use default config
                monitor.Log($"Error while reading config.json. Using default settings instead. Full error message: \n{ex}", LogLevel.Warn);
            }
        }

        /// <summary>True if bed placement should be enabled at all locations.</summary>
        public bool AllowBedPlacementEverywhere { get; set; } = false;
        /// <summary>True if the "passing out" penalty should be removed from all locations.</summary>
        public bool PassOutSafelyEverywhere { get; set; } = false;
        /// <summary>True if Mini-Fridge placement should be enabled at all locations.</summary>
        public bool AllowMiniFridgesEverywhere { get; set; } = false;
    }
}
