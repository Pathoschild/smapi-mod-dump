using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>An object that returns valid "spawnable" tiles from a generated tile list.</summary>
        private class TileValidator
        {
            /// <summary>The GameLocation containing the listed tiles.</summary>
            private GameLocation Location;
            /// <summary>A list of possible spawn tiles to validate. This should always match TileSet.</summary>
            private List<Vector2> TileList;
            /// <summary>A set of possible spawn tiles to validate. This should always match TileList.</summary>
            private HashSet<Vector2> TileSet;
            /// <summary>The strictness level to use when checking tiles.</summary>
            private string StrictTileChecking;
            /// <summary>A set of sizes for which no valid tiles are available.</summary>
            private HashSet<Point> DepletedSizes = new HashSet<Point>();

            /// <param name="location">The GameLocation containing the listed tiles.</param>
            /// <param name="tileList">A list of possible spawn tiles for a particular spawn area.</param>
            /// <param name="strictTileChecking">The strict tile checking setting to be used.</param>
            public TileValidator(GameLocation location, List<Vector2> tileList, string strictTileChecking)
            {
                Location = location;
                TileList = tileList;
                ShuffleTileList(); //randomize TileList's order
                TileSet = new HashSet<Vector2>(TileList); //create a set based on the list
                StrictTileChecking = strictTileChecking;
            }

            /// <summary>Returns a random valid tile for an object of the given size, or null if none are available.</summary>
            /// <param name="size">A point representing the X,Y tile size of the object to be spawned.</param>
            /// <returns>A valid "spawnable" tile. Null if none are available.</returns>
            public Vector2? GetTile(Point size)
            {
                if (DepletedSizes.Contains(size)) //if this size has previously returned "null"
                {
                    return null; //don't bother to check
                }

                for (int index = 0; index < TileList.Count; index++) //for each listed tile
                {
                    bool allTilesInSet = true; //whether all the tiles necessary for the given size exist in TileSet

                    //for each tile necessary for the given size
                    for (int x = 0; x < size.X; x++)
                    {
                        for (int y = 0; y < size.Y; y++)
                        {
                            Vector2 tileToCheck = new Vector2(TileList[index].X + x, TileList[index].Y + y); //the tile currently being checked
                            if (!TileSet.Contains(tileToCheck)) //if this tile isn't in the set
                            {
                                allTilesInSet = false; //"tile" can't be used for the given size
                                break; //skip the rest of the "y" loop
                            }
                        }

                        if (!allTilesInSet)
                            break; //skip the rest of the "x" loop
                    }

                    if (!allTilesInSet)
                        continue; //skip to the next tile index

                    bool allTilesValid = true;

                    //for each tile necessary for the given size
                    for (int x = 0; x < size.X; x++)
                    {
                        for (int y = 0; y < size.Y; y++)
                        {
                            Vector2 tileToCheck = new Vector2(TileList[index].X + x, TileList[index].Y + y); //the tile currently being checked
                            if (!Utility.IsTileValid(Location, tileToCheck, new Point(1, 1), StrictTileChecking)) //if the tile being checked is NOT valid
                            {
                                if (TileList.IndexOf(tileToCheck) <= index) //if the invalid tile is the "index" tile OR before it in the list
                                    index--; //decrement by 1 to avoid looping errors

                                //remove the tile from future checks
                                TileList.Remove(tileToCheck);
                                TileSet.Remove(tileToCheck);

                                allTilesValid = false;
                                break; //skip the rest of the "y" loop
                            }
                        }

                        if (!allTilesValid)
                            break; //skip the rest of the "x" loop
                    }

                    if (allTilesValid) //if all necessary tiles were valid
                    {
                        Vector2 finalTile = TileList[index]; //get the indexed tile

                        //remove the tile from the list and set
                        TileList.RemoveAt(index);
                        TileSet.Remove(finalTile);

                        return finalTile;
                    }
                    else //if a tile was invalid
                    {
                        continue; //skip to the next tile index
                    } 
                }

                //if no listed tiles are valid for the given size
                DepletedSizes.Add(size); //add this size to the list, making it faster to "reject" further requests for this size
                return null; 
            }

            /// <summary>Randomize the order of items in TileList.</summary>
            private void ShuffleTileList()
            {
                for (int current = TileList.Count - 1; current > 0; current--) //for each tile's index (counting backward and excluding 0)
                {
                    int random = Utility.RNG.Next(current + 1); //get a random index between 0 and this tile's index
                    //swap the current tile with the tile at the random index
                    Vector2 temp = TileList[random];
                    TileList[random] = TileList[current];
                    TileList[current] = temp;
                }
            }
        }
    }
}