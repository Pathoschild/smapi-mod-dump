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
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.World.Objects;
using StardewValley;
using StardewValley.Buildings;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities
{
    /// <summary>
    /// Utilities pertaining to the game locations in Stardew Valley.
    /// </summary>
    public class GameLocationUtilities
    {
        /// <summary>
        /// Gets a game location from a StardewLocation enum value.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static GameLocation GetGameLocation(Enums.StardewLocation location)
        {
            return Game1.getLocationFromName(Enum.GetName<Enums.StardewLocation>(location));
        }
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
        /// Checks to see if the hard mines are currently enabled for special benefits.
        /// </summary>
        /// <returns></returns>
        public static bool AreTheHardMinesEnabled()
        {
            return Game1.player.team.mineShrineActivated.Value == true;
        }

        /// <summary>
        /// Checks to see if the player is in the regular mine.
        /// </summary>
        /// <returns></returns>
        public static bool IsPlayerInMine()
        {
            return IsLocationInTheMines(Game1.player.currentLocation);
        }

        /// <summary>
        /// Checks to see if a given game location is in the mines.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static bool IsLocationInTheMines(GameLocation location)
        {
            if (location.Name.StartsWith("UndergroundMine"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see if a given location is the enterance to the mines.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static bool IsLocationTheEntranceToTheMines(GameLocation location)
        {
            if (location.Name.StartsWith("Mine") || location.Name.Equals("Mine"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see if the player is in the enterance to the mine.
        /// </summary>
        /// <returns></returns>
        public static bool IsPlayerInTheEnteranceToTheMines()
        {
            return IsLocationTheEntranceToTheMines(Game1.player.currentLocation);
        }

        /// <summary>
        /// Checks to see if a game location is in Skull Caves or not.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static bool IsLocationInSkullCaves(GameLocation location)
        {
            if (location.Name.Equals("SkullCave") || location.Name.StartsWith("SkullCave"))
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
            return IsLocationInSkullCaves(Game1.player.currentLocation);
        }

        /// <summary>
        /// Checks to see if the player is in the volcano dungeon mine.
        /// </summary>
        /// <returns></returns>
        public static bool IsPlayerInTheVolcanoDungeon()
        {
            return IsLocationTheVolcanoDungeon(Game1.player.currentLocation);
        }

        /// <summary>
        /// Checks to see if a given game location is in the volcano dungeon mine.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static bool IsLocationTheVolcanoDungeon(GameLocation location)
        {
            if (location.Name.StartsWith("VolcanoDungeon"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see if the player is in the caldera.
        /// </summary>
        /// <returns></returns>
        public static bool IsPlayerInTheCaldera()
        {
            return IsLocationTheCaldera(Game1.player.currentLocation);
        }

        /// <summary>
        /// Checks to see if a given location is the caldera.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static bool IsLocationTheCaldera(GameLocation location)
        {
            if (location.Name.StartsWith("Caldera"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the current mine level for the player. If the player is not in the mine this is -1.
        /// </summary>
        /// <returns></returns>
        public static int GetPlayerCurrentMineLevel()
        {
            if (IsPlayerInMine())
            {
                return CurrentMineLevel(Game1.player.currentLocation);
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets the current level of the the mines for a given game location, or returns -1 if the location is not in the mines.
        /// </summary>
        /// <param name="gameLocation"></param>
        /// <returns></returns>
        public static int CurrentMineLevel(GameLocation gameLocation)
        {
            if(gameLocation is StardewValley.Locations.MineShaft)
            {
                return (gameLocation as StardewValley.Locations.MineShaft).mineLevel;
            }
            return -1;
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
            foreach (Building b in Game1.getFarm().buildings)
            {
                locations.Add(b.indoors.Value);
            }
            return locations;
        }

        public static bool IsThereWaterAtThisTile(GameLocation location, int X, int Y)
        {
            if (location.doesTileHaveProperty((int)X, (int)Y, "Water", "Back") == null)
                return false;
            return true;
        }


    }
}
