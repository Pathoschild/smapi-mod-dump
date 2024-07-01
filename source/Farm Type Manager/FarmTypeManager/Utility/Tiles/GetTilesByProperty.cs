/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Produces a list of x/y coordinates for object spawning at a location (based on tile properties, e.g. the "grass" type).</summary>
            /// <param name="location">The game location to be checked.</param>
            /// <param name="type">A string representing the tile property to match, or a special term used for some additional checks.</param>
            /// <returns>A list of Vector2, each representing a tile for object spawning at the given location.</returns>
            public static List<Vector2> GetTilesByProperty(GameLocation location, string type)
            {
                List<Vector2> tiles = new List<Vector2>(); //a list of x,y coordinates for new object placement

                if (string.IsNullOrWhiteSpace(type)) //if no valid type was provided
                    return tiles; //return the empty list

                //get the total size of the current map
                int mapX = location.Map.DisplayWidth / Game1.tileSize;
                int mapY = location.Map.DisplayHeight / Game1.tileSize;

                //if this type includes a specific tile property and value, store them here
                string[] nameAndValue;

                if (type.Contains(':', StringComparison.Ordinal)) //if the tile contains a colon
                    nameAndValue = type.Split(':', 2, StringSplitOptions.TrimEntries); //split the type into a property name and value, then trim whitespace (e.g. "PropertyName: PropertyValue" becomes "PropertyName" and "PropertyValue")
                else
                    nameAndValue = null;

                //the following loops should populate a list of tiles for spawning
                for (int y = 0; y < mapY; y++)
                {
                    for (int x = 0; x < mapX; x++) //loops for each tile on the map, from the top left (x,y == 0,0) to bottom right, moving horizontally first
                    {
                        if (nameAndValue != null) //if this type uses the "name and value" format (see above)
                        {
                            string currentValue = location.doesTileHavePropertyNoNull(x, y, nameAndValue[0], "Back"); //get the property's value on this tile ("" if null)

                            if (currentValue.Equals(nameAndValue[1], StringComparison.OrdinalIgnoreCase)) //if the tile's value matches the given value (case-insensitive)
                            {
                                tiles.Add(new Vector2(x, y));
                            }
                        }
                        else if (type.Equals("diggable", StringComparison.OrdinalIgnoreCase))
                        {
                            if (location.doesTileHaveProperty(x, y, "Diggable", "Back") != null) //if this tile has the "Diggable" property with any value (note: SDV almost never cares about the text of this property)
                            {
                                tiles.Add(new Vector2(x, y));
                            }
                        }
                        else if (type.Equals("all", StringComparison.OrdinalIgnoreCase))
                        {
                            tiles.Add(new Vector2(x, y)); //add this tile to the list, regardless of its properties
                        }
                        else //check for the given value in the tile's "Type" property, e.g. "Grass" or "Dirt"
                        {
                            string currentType = location.doesTileHavePropertyNoNull(x, y, "Type", "Back"); //get the "Type" property's value on this tile ("" if null)

                            if (currentType.Equals(type, StringComparison.OrdinalIgnoreCase)) //if the tile's value matches the given value (case-insensitive)
                            {
                                tiles.Add(new Vector2(x, y));
                            }
                        }
                    }
                }
                return tiles; //return the completed list
            }
        }
    }
}