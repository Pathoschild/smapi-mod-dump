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
            /// <summary>Produces a set of x/y coordinates for possible tiles for object spawning at a location (based on a string describing two vectors).</summary>
            /// <param name="location">The game location to be checked.</param>
            /// <param name="vectorString">A string describing two vectors. Parsed into vectors and used to find a rectangular area.</param>
            /// <returns>A list of Vector2, each representing a tile for object spawning at the given location.</returns>
            public static HashSet<Vector2> GetTilesByVectorString(GameLocation location, string vectorString)
            {
                HashSet<Vector2> tiles = new HashSet<Vector2>(); //x,y coordinates for tiles in the provided range
                List<Tuple<Vector2, Vector2>> vectorPairs = new List<Tuple<Vector2, Vector2>>(); //pairs of x,y coordinates representing areas on the map (to be scanned for existing tiles)

                //parse the "raw" string representing two coordinates into actual numbers, populating "vectorPairs"
                string[] xyxy = vectorString.Split(new char[] { ',', '/', ';' }); //split the string into separate strings based on various delimiter symbols
                if (xyxy.Length != 4) //if "xyxy" didn't split into the right number of strings, it's probably formatted poorly
                {
                    Monitor.Log($"Issue: This include/exclude area for the \"{location.Name}\" map isn't formatted correctly: \"{vectorString}\"", LogLevel.Info);
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
                        Monitor.Log($"Issue: This include/exclude area for the \"{location.Name}\" map isn't formatted correctly: \"{vectorString}\"", LogLevel.Info);
                    }
                }

                //get the total size of the current map
                int mapX = location.Map.DisplayWidth / Game1.tileSize;
                int mapY = location.Map.DisplayHeight / Game1.tileSize;

                foreach (Tuple<Vector2, Vector2> pair in vectorPairs) //for each pair of coordinates
                {
                    //get the specific "corners" of the rectangle represented by these coordinates
                    int xMin = (int)Math.Min(pair.Item1.X, pair.Item2.X);
                    int xMax = (int)Math.Max(pair.Item1.X, pair.Item2.X);
                    int yMin = (int)Math.Min(pair.Item1.Y, pair.Item2.Y);
                    int yMax = (int)Math.Max(pair.Item1.Y, pair.Item2.Y);

                    if (xMin >= mapX || yMin >= mapY || xMax < 0 || yMax < 0) //if the entire rectangle is out of bounds
                    {
                        continue; //skip to the next pair
                    }

                    //limit the rectangle to the edge of the current map
                    xMax = Math.Min(xMax, mapX - 1);
                    yMax = Math.Min(yMax, mapY - 1);

                    for (int y = yMin; y <= yMax; y++) //for each Y value in the rectangular area denoted by the coordinates
                    {
                        for (int x = xMin; x <= xMax; x++) //for each X value in the rectangular area denoted by the coordinates
                        {
                            tiles.Add(new Vector2(x, y)); //add to the list
                        }
                    }
                }

                return tiles;
            }
        }
    }
}