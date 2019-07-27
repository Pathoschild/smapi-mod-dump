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
            /// <summary>Produces a list of x/y coordinates for valid, open tiles for object spawning at a location (based on a string describing two vectors).</summary>
            /// <param name="area">The SpawnArea describing the current area and its settings.</param>
            /// <param name="vectorString">A string describing two vectors. Parsed into vectors and used to find a rectangular area.</param>
            /// <param name="isLarge">True if the objects to be spawned are 2x2 tiles in size, otherwise false (1 tile).</param>
            /// <returns>A list of Vector2, each representing a valid, open tile for object spawning at the given location.</returns>
            public static List<Vector2> GetTilesByVectorString(SpawnArea area, string vectorString, bool isLarge)
            {
                GameLocation loc = Game1.getLocationFromName(area.MapName); //variable for the current location being worked on
                List<Vector2> validTiles = new List<Vector2>(); //x,y coordinates for tiles that are open & valid for new object placement
                List<Tuple<Vector2, Vector2>> vectorPairs = new List<Tuple<Vector2, Vector2>>(); //pairs of x,y coordinates representing areas on the map (to be scanned for valid tiles)

                //parse the "raw" string representing two coordinates into actual numbers, populating "vectorPairs"
                string[] xyxy = vectorString.Split(new char[] { ',', '/', ';' }); //split the string into separate strings based on various delimiter symbols
                if (xyxy.Length != 4) //if "xyxy" didn't split into the right number of strings, it's probably formatted poorly
                {
                    Monitor.Log($"Issue: This include/exclude area for the {area.MapName} map isn't formatted correctly: \"{vectorString}\"", LogLevel.Info);
                }
                else
                {
                    int[] numbers = new int[4]; //this section will convert "xyxy" into four numbers and store them here
                    bool success = true;
                    for (int i = 0; i < 4; i++)
                    {
                        if (Int32.TryParse(xyxy[i].Trim(), out numbers[i]) != true) //attempts to store each "xyxy" string as an integer in "numbers"; returns false if it failed
                        {
                            success = false;
                        }
                    }

                    if (success) //everything was successfully parsed, apparently
                    {
                        //convert the numbers to a pair of vectors and add them to the list
                        vectorPairs.Add(new Tuple<Vector2, Vector2>(new Vector2(numbers[0], numbers[1]), new Vector2(numbers[2], numbers[3])));
                    }
                    else
                    {
                        Monitor.Log($"Issue: This include/exclude area for the {area.MapName} map isn't formatted correctly: \"{vectorString}\"", LogLevel.Info);
                    }
                }

                //check the area marked by "vectorPairs" for valid, open tiles and populate "validTiles" with them
                foreach (Tuple<Vector2, Vector2> pair in vectorPairs)
                {
                    for (int y = (int)Math.Min(pair.Item1.Y, pair.Item2.Y); y <= (int)Math.Max(pair.Item1.Y, pair.Item2.Y); y++) //use the lower Y first, then the higher Y; should define the area regardless of which corners/order the user wrote down
                    {
                        for (int x = (int)Math.Min(pair.Item1.X, pair.Item2.X); x <= (int)Math.Max(pair.Item1.X, pair.Item2.X); x++) //loops for each tile on the map, from the top left (x,y == 0,0) to bottom right, moving horizontally first
                        {
                            Vector2 tile = new Vector2(x, y);
                            if (IsTileValid(area, new Vector2(x, y), isLarge)) //if the tile is clear of any obstructions
                            {
                                validTiles.Add(tile); //add to list of valid spawn tiles
                            }
                        }
                    }
                }

                return validTiles;
            }
        }
    }
}