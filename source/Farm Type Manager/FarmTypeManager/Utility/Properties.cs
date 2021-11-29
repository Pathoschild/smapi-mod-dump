/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

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
            /// <summary>Parses today's weather from several booleans into a "Weather" enum.</summary>
            /// <returns>A "Weather" enum describing today's weather.</returns>
            public static Weather WeatherForToday()
            {
                if (Game1.isSnowing)
                    return Weather.Snow;
                if (Game1.isRaining)
                    return Game1.isLightning ? Weather.Lightning : Weather.Rain;
                if (SaveGame.loaded?.isDebrisWeather ?? Game1.isDebrisWeather)
                    return Weather.Debris;

                return Weather.Sunny;
            }

            /// <summary>Encapsulates IMonitor.Log() for this mod's static classes. Must be given an IMonitor in the ModEntry class to produce output.</summary>
            public static class Monitor
            {
                private static IMonitor monitor;

                public static IMonitor IMonitor
                {
                    set
                    {
                        monitor = value;
                    }
                }

                /// <summary>Log a message for the player or developer.</summary>
                /// <param name="message">The message to log.</param>
                /// <param name="level">The log severity level.</param>
                public static void Log(string message, LogLevel level = LogLevel.Debug)
                {
                    if (monitor != null) //if the monitor is ready
                    {
                        if (MConfig.EnableTraceLogMessages || level != LogLevel.Trace) //if trace messages are enabled OR this isn't a trace message
                        {
                            monitor.Log(message, level);
                        }
                    }
                }

                /// <summary>Log a message for the player or developer, but only if the message has not been logged already this session.</summary>
                /// <param name="message">The message to log.</param>
                /// <param name="level">The log severity level.</param>
                public static void LogOnce(string message, LogLevel level = LogLevel.Debug)
                {
                    if (monitor != null) //if the monitor is ready
                    {
                        if (MConfig.EnableTraceLogMessages || level != LogLevel.Trace) //if trace messages are enabled OR this isn't a trace message
                        {
                            monitor.LogOnce(message, level);
                        }
                    }
                }

                /// <summary>Log a message that only appears when IMonitor.IsVerbose is enabled.</summary>
                /// <param name="message">The message to log.</param>
                public static void VerboseLog(string message)
                {
                    if (monitor != null) //if the monitor is ready
                    {
                        if (MConfig.EnableTraceLogMessages) //if trace messages are enabled (since verbose logs are equivalent to trace-level)
                        {
                            monitor.VerboseLog(message);
                        }
                    }
                }
            }

            /// <summary>The helper provided to this mod by SMAPI. Should be set during mod startup.</summary>
            public static IModHelper Helper { get; set; }

            /// <summary>A list of all config data for the current farm, related save data, and content pack (if applicable).</summary>
            public static List<FarmData> FarmDataList = new List<FarmData>();

            /// <summary>A series of object lists to be spawned during the current in-game day.</summary>
            public static List<List<TimedSpawn>> TimedSpawns = new List<List<TimedSpawn>>();

            /// <summary>The API for Expanded Preconditions Utility (EPU), used to check precondition strings. Null if unavailable.</summary>
            public static IConditionsChecker EPUConditionsChecker { get; set; } = null;

            /// <summary>The API for Dynamic Game Assets (DGA), used to spawn its custom items. Null if unavailable.</summary>
            public static IDynamicGameAssetsApi DGAItemAPI { get; set; } = null;

            /// <summary>The global settings for this mod. Should be set during mod startup.</summary>
            public static ModConfig MConfig { get; set; }

            /// <summary>Random number generator shared throughout the mod. Initialized automatically.</summary>
            public static Random RNG { get; } = new Random();

            /// <summary>A marker used to track other mods' changes to the the game's start time and minimize related issues.</summary>
            public static StardewTime StartOfDay { get; set; } = 600;

            /// <summary>Set to true when the "DayEnding" SMAPI event begins, then set to false when SMAPI's "DayStarted" event ends. Used to detect Android/mod-related save events.</summary>
            public static bool DayIsEnding { get; set; } = false;

            /// <summary>Set to true while the Stardew is currently saving the game, whether mid-day or end-of-day.</summary>
            public static bool GameIsSaving { get; set; } = false;

            /// <summary>Enumerated list of farm types, in the order used by Stardew's internal code (e.g. Farm.cs)</summary>
            public enum FarmTypes { Standard, Riverland, Forest, Hilltop, Wilderness, FourCorners, Beach }

            /// <summary>Enumerated list of player skills, in the order used by Stardew's internal code (e.g. Farmer.cs).</summary>
            public enum Skills { Farming, Fishing, Foraging, Mining, Combat, Luck }

            /// <summary>Enumerated list of weather condition types, in the order used by Stardew's internal code (e.g. Game1.cs)</summary>
            public enum Weather { Sunny, Rain, Debris, Lightning, Festival, Snow, Wedding }
        }
    }
}
