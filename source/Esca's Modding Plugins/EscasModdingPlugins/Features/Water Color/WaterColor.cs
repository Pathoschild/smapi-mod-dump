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
using StardewValley;
using System;
using System.Collections.Generic;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;

namespace EscasModdingPlugins
{
    /// <summary>Allows mods to override a location's water color. Uses map properties.</summary>
    public static class WaterColor
    {
        /// <summary>The name of the map property used by this patch.</summary>
		public static string MapPropertyName { get; set; } = null;

        /// <summary>True if this class's behavior is currently enabled.</summary>
        public static bool Enabled { get; private set; } = false;

        /// <summary>The helper instance to use for API access.</summary>
        private static IModHelper Helper { get; set; } = null;

        /// <summary>The monitor instance to use for console/log messages.</summary>
        private static IMonitor Monitor { get; set; } = null;

        /// <summary>Initializes this class and enables its features.</summary>
        /// <param name="helper">The helper instance to use for API access.</param>
        /// <param name="monitor">The monitor instance to use for console/log messages.</param>
        public static void Enable(IModHelper helper, IMonitor monitor)
        {
            if (Enabled)
                return; //do nothing

            //store args
            Helper = helper;
            Monitor = monitor;

            //initialize assets/properties
            MapPropertyName = ModEntry.PropertyPrefix + "WaterColor";

            //initialize events
            helper.Events.GameLoop.DayStarted += DayStarted_UpdateWaterColor;
            helper.Events.GameLoop.TimeChanged += TimeChanged_UpdateWaterColor;
            helper.Events.Player.Warped += Warped_UpdateWaterColor;

            Enabled = true;
        }

        private static void DayStarted_UpdateWaterColor(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            CachedWaterColors.Clear(); //clear the cache to allow changes by the game

            HashSet<GameLocation> locationsToUpdate = new HashSet<GameLocation>(); //create a set of distinct locations to update
            foreach (Farmer player in Game1.getOnlineFarmers()) //for each active player
            {
                if (player.currentLocation != null) //if this player has a location
                    locationsToUpdate.Add(player.currentLocation); //add it to the set
            }
            foreach (GameLocation location in locationsToUpdate) //for each location in the set
            {
                UpdateWaterColor(location); //update it
            }
        }

        private static void TimeChanged_UpdateWaterColor(object sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            HashSet<GameLocation> locationsToUpdate = new HashSet<GameLocation>(); //create a set of distinct locations to update
            foreach (Farmer player in Game1.getOnlineFarmers()) //for each active player
            {
                if (player.currentLocation != null) //if this player has a location
                    locationsToUpdate.Add(player.currentLocation); //add it to the set
            }
            foreach (GameLocation location in locationsToUpdate) //for each location in the set
            {
                UpdateWaterColor(location); //update it
            }
        }

        private static void Warped_UpdateWaterColor(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            if (e.NewLocation != null)
                UpdateWaterColor(e.NewLocation); //update for the warping player's new location
        }

        /// <summary>Contains the "original" water color of each location prior to any changes made by this class. Entries are removed after reverting to the original color.</summary>
        private static Dictionary<string, Color> CachedWaterColors = new Dictionary<string, Color>();

        /// <summary>Updates the provided location's water color based on this class's custom properties.</summary>
        /// <param name="location">The location to update.</param>
        private static void UpdateWaterColor(GameLocation location)
        {
            if (!Context.IsWorldReady || !Context.IsMainPlayer || location == null) //if the local player is NOT hosting a game session OR their location is null
                return; //do nothing

            string locationName = location.Name ?? "";
            Color? customColor = GetCustomWaterColor(location);
            if (customColor.HasValue) //if a custom water color was found
            {
                if (!CachedWaterColors.ContainsKey(locationName)) //if a cached color does NOT exist for this location
                    CachedWaterColors[locationName] = location.waterColor.Value; //cache its current color

                location.waterColor.Value = customColor.Value; //apply the custom color
            }
            else //if a custom water color was NOT found
            {
                if (CachedWaterColors.TryGetValue(locationName, out Color cachedColor)) //if the "original" color was cached
                {
                    location.waterColor.Value = cachedColor; //apply it
                    CachedWaterColors.Remove(locationName); //remove it from the cache
                }
            }
        }

        /// <summary>Determines the current water color to use for the given location.</summary>
        /// <param name="location">The location to check.</param>
        /// <returns>The color to use for this location's water. Null if no custom color currently exists for this location.</returns>
        private static Color? GetCustomWaterColor(GameLocation location)
        {
            if (location.Map.Properties.TryGetValue(MapPropertyName, out var mapPropertyObject)) //if the location has a non-null map property
            {
                string mapProperty = mapPropertyObject.ToString() ?? ""; //get the map property as a string
                string[] args = mapProperty.Trim().Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries); //split the property value along spaces and remove any blank args

                Color? colorToUse = null;

                switch (args.Length) //based on the number of args...
                {
                    case 0: //if no args exist, treat it as blank (don't modify the color)
                        break;
                    case 1: //if 1 arg exists, treat it as a color name
                        if (Enum.TryParse(args[0], true, out KnownColor namedColor)) //if this is a recognized color name
                        {
                            System.Drawing.Color systemColor = System.Drawing.Color.FromKnownColor(namedColor); //convert it to a System.Drawing.Color
                            colorToUse = new Color(systemColor.R, systemColor.G, systemColor.B, systemColor.A); //convert it to an XNA color and use it
                        }
                        else
                        {
                            Monitor.LogOnce($"Failed to parse map property {MapPropertyName}. Value: \"{mapProperty}\". It appears to be an unrecognized color name.", LogLevel.Debug);
                        }
                        break;
                    case 3: //if 3 args exist, treat it as an RGB value
                        if (byte.TryParse(args[0], out byte r) && byte.TryParse(args[1], out byte g) && byte.TryParse(args[2], out byte b)) //if the args parse into bytes successfully (values from 0-255)
                        {
                            colorToUse = new Color(r, g, b, 0.5f); //convert it to an XNA color and use it (default to 50% transprency, imitating most Stardew water colors)
                        }
                        else
                        {
                            Monitor.LogOnce($"Failed to parse map property {MapPropertyName}. Value: \"{mapProperty}\". It appears to be an invalid RGB value.", LogLevel.Debug);
                        }
                        break;
                    case 4: //if 4 args exist, treat it as an RGBA value
                        if (byte.TryParse(args[0], out r) && byte.TryParse(args[1], out g) && byte.TryParse(args[2], out b) && byte.TryParse(args[3], out byte a)) //if the args parse into bytes successfully (values from 0-255)
                        {
                            colorToUse = new Color(r, g, b, a); //convert it to an XNA color and use it
                        }
                        else
                        {
                            Monitor.LogOnce($"Failed to parse map property {MapPropertyName}. Value: \"{mapProperty}\". It appears to be an invalid RGBA value.", LogLevel.Debug);
                        }
                        break;
                    default: //invalid number of args
                        Monitor.Log($"Failed to parse map property {MapPropertyName}. Value: \"{mapProperty}\". It appears to have an invalid number of arguments.", LogLevel.Debug);
                        break;
                }

                if (colorToUse != null) //if map property exists and was parsed successfully
                {
                    if (Monitor.IsVerbose)
                        Monitor.Log($"Overriding water color based on map property {MapPropertyName}. Location: {location.Name}. Property value: {mapProperty}.", LogLevel.Trace);
                    return colorToUse.Value;
                }
            }

            return null; //no valid map property found; return no color
        }
    }
}