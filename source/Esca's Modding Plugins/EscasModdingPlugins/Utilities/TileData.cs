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
using System.Linq;

namespace EscasModdingPlugins
{
    /// <summary>Data that applies to a specific set of <see cref="GameLocation"/> names and tiles. May be compared for relative priority and parsed from a tile property string.</summary>
    public abstract class TileData : IComparable
    {
        /**************/
        /* Properties */
        /**************/

        /// <summary>The monitor to use for log messages in static methods.</summary>
        public static IMonitor Monitor { protected get; set; } = null;

        /// <summary>The set of <see cref="GameLocation"/> names where this data is applicable.</summary>
        public List<string> Locations { get; set; } = new List<string>();
        /// <summary>The set of tiles where this data is applicable.</summary>
        public List<JsonRectangle> TileAreas { get; } = new List<JsonRectangle>();
        /// <summary>The relative priority of this data. Tiles affected by multiple data instances will use one with the highest priority.</summary>
        public int Priority { get; set; } = 0;

        /***********/
        /* Methods */
        /***********/

        /// <summary>Attempts to populate this instance with data parsed from a string.</summary>
        /// <param name="raw">The raw string to parse. Typically loaded from a tile property at a <see cref="GameLocation"/>.</param>
        /// <returns>True if parsing succeeded; false otherwise. If false, this instance should be unmodified from its previous state.</returns>
        public abstract bool TryParse(string raw);

        /// <summary>Gets the currently applicable data for a location and tile. Uses the named data asset and/or tile property.</summary>
        /// <remarks>
        /// The format for tile assets is currently assumed to be a <see cref="Dictionary{string, ITileData{T}}"/>.
        /// The dictionary keys are arbitrary unique string IDs; the values are instances of the data type.
        /// This format is used to improve compatibility with Content Patcher, which has some limitations when accessing lists.
        /// </remarks>
        /// <typeparam name="T">The type of data used for this feature. Must be an <see cref="ITileData{T}"/>.</typeparam>
        /// <param name="assetName">The name of the asset to use. Must be an <see cref="IDictionary{string, T}"/>.</param>
        /// <param name="tilePropertyName">The name of the tile property to use. Null if no tile property is implemented for this data.</param>
        /// <param name="location">The location to check.</param>
        /// <param name="tileX">The X value of the tile to check.</param>
        /// <param name="tileY">The Y value of the tile to check.</param>
        /// <returns>The highest-priority data that currently applies to the tile. Null if no data applies.</returns>
        public static T GetDataForTile<T>(string assetName, string tilePropertyName, GameLocation location, int tileX, int tileY) where T : TileData, new() //T must be a type of TileData with a default constructor
        {
            string locationName = location?.Name ?? "";

            if (Monitor?.IsVerbose == true)
                Monitor.LogOnce($"Loading tile data for {tileX},{tileY} at {locationName}. Asset name: \"{assetName}\". Tile property name: \"{tilePropertyName}\".", LogLevel.Trace);

            var asset = AssetHelper.GetAsset<IDictionary<string, T>>(assetName); //load the asset as a dictionary of arbitrary string keys and data values

            KeyValuePair<string, T>? entryToUse = null; //the entry to use (keeping the KVP so that the key can be used in log messages)

            foreach (var entry in asset) //for each data entry
            {
                if (entryToUse == null || entry.Value?.CompareTo(entryToUse.Value.Value) > 0) //if this entry has a higher priority than "entry to use"
                {
                    if (entry.Value.Locations?.Contains(locationName, StringComparer.OrdinalIgnoreCase) == true) //if this entry's locations include the target location name
                    {
                        if (entry.Value.TileAreas?.Exists(r => r.AsRect().Contains(tileX, tileY)) == true) //if this entry's tile areas contain the target tile
                        {
                            entryToUse = entry; //this is the highest-priority applicable entry so far
                        }
                    }
                }
            }

            if (entryToUse != null) //if any entries in the asset applied
            {
                if (Monitor?.IsVerbose == true)
                    Monitor.LogOnce($"Matching asset data found. Key: \"{entryToUse.Value.Key}\".", LogLevel.Trace);
                return entryToUse.Value.Value; //use the asset entry data
            }

            if (location?.doesTileHaveProperty(tileX, tileY, tilePropertyName, "Back") is string tilePropertyValue) //if no data assets applied, but the tile property exists
            {
                T data = new T();
                if (data.TryParse(tilePropertyValue)) //if the property value can be parsed into data
                {
                    if (Monitor?.IsVerbose == true)
                        Monitor.LogOnce($"Matching tile property data found. Value: \"{tilePropertyValue}\".", LogLevel.Trace);
                    return data; //use the tile property data
                }
            }

            //if no asset or tile property data applied
            return null;
        }

        /**********************/
        /* IComparable method */
        /**********************/

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1; //greater than null

            if (obj is TileData data)
                return this.Priority.CompareTo(data.Priority); //compare by priority values
            else
                throw new ArgumentException("Object is not TileData");
        }
    }
}
