/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Revitalize.Framework.World.Objects;
using Revitalize.Framework.Objects;
using StardewValley;
using StardewValley.Buildings;

namespace Revitalize.Framework.Utilities
{
    /// <summary>
    /// Deals with helping with locational logic.
    /// </summary>
    public class LocationUtilities
    {

        /// <summary>
        /// The int value used by SDV to determine if a farm is a hilltop farm.
        /// </summary>
        public static int Farm_HilltopFarmNumber
        {
            get
            {
                return 3;
            }
        }


        /// <summary>
        /// Checks to see if the player is in the regular mine.
        /// </summary>
        /// <returns></returns>
        public static bool IsPlayerInMine()
        {
            if (Game1.player.currentLocation.Name.StartsWith("UndergroundMine"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see if the player is in the enterance to the mine.
        /// </summary>
        /// <returns></returns>
        public static bool IsPlayerInMineEnterance()
        {
            if (Game1.player.currentLocation.Name.StartsWith("Mine") || Game1.player.currentLocation.Name == "Mine")
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Checks to see if the player is in skull cave.
        /// </summary>
        /// <returns></returns>
        public static bool IsPlayerInSkullCave()
        {
            if (Game1.player.currentLocation.Name == "SkullCave" || Game1.player.currentLocation.Name.StartsWith("SkullCave"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the current mine level for the player. If the player is not in the mine this is -1.
        /// </summary>
        /// <returns></returns>
        public static int CurrentMineLevel()
        {
            if (IsPlayerInMine())
            {
                return (Game1.player.currentLocation as StardewValley.Locations.MineShaft).mineLevel;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets the tile width and height for the map.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static Vector2 GetLocationTileDimensions(GameLocation location)
        {
            Vector2 dimensions = new Vector2(location.Map.GetLayer("Back").LayerWidth, location.Map.GetLayer("Back").LayerHeight);
            //ModCore.log("Dimensions of map is: " + dimensions);
            return dimensions;
        }

        /// <summary>
        /// Gets all open positions for this location for this object.
        /// </summary>
        /// <param name="Location"></param>
        /// <param name="TestObject"></param>
        /// <returns></returns>
        public static List<Vector2> GetOpenObjectTiles(GameLocation Location, CustomObject TestObject)
        {
            Vector2 dimensions = GetLocationTileDimensions(Location);
            List<Vector2> openTiles = new List<Vector2>();
            for (int i = 0; i < dimensions.X; i++)
            {
                for (int j = 0; j < dimensions.Y; j++)
                {
                    Vector2 tile = new Vector2(i, j);
                    if (TestObject.canBePlacedHere(Location, tile))
                    {
                        openTiles.Add(tile);
                    }
                }
            }
            return openTiles;
        }

        /// <summary>
        /// Checks to see if the farm for the game is the hilltop farm.
        /// </summary>
        /// <returns></returns>
        public static bool Farm_IsFarmHiltopFarm()
        {
            return Game1.whichFarm == Farm_HilltopFarmNumber;
        }

        public static List<GameLocation> GetAllLocations()
        {
            List<GameLocation> locations = new List<GameLocation>();
            foreach (GameLocation location in Game1.locations)
            {
                locations.Add(location);
            }
            foreach(Building b in Game1.getFarm().buildings)
            {
                locations.Add(b.indoors.Value);
            }
            return locations;
        }

        public static bool IsThereWaterAtThisTile(GameLocation location, int X, int Y)
        {
            if ( location.doesTileHaveProperty((int)X, (int)Y, "Water", "Back") == null)
                return false;
            return true;
        }
    }
}
