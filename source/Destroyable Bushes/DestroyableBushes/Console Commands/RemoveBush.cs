/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/DestroyableBushes
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace DestroyableBushes
{
    public static partial class Commands
    {
        /// <summary>SMAPI console command. Removes a bush from the specified tile.</summary>
        /// <param name="command">The console command's name.</param>
        /// <param name="args">The space-delimited arguments entered with the command.</param>
        public static void RemoveBush(string command, string[] args)
        {
            if (!Context.IsWorldReady) //if the player is not currently in a loaded game
                return; //do nothing

            if (args.Length == 1) //"remove_bush [x]" (invalid)
            {
                ModEntry.Instance.Monitor.Log($"Invalid number of arguments. Please include both X and Y, e.g. \"{command} 64 19\". Type \"help {command}\" for formatting information.", LogLevel.Info);
                return;
            }

            int x;
            int y;

            if (args.Length >= 2) //if x y arguments were provided
            {
                if (int.TryParse(args[0], out x) == false) //try to parse the x argument; if it fails,
                {
                    ModEntry.Instance.Monitor.Log($"\"{args[0]}\" could not be parsed as an integer (x). Type \"help {command}\" for formatting information.", LogLevel.Info);
                    return;
                }
                else if (int.TryParse(args[1], out y) == false) //try to parse the y argument; if it fails,
                {
                    ModEntry.Instance.Monitor.Log($"\"{args[1]}\" could not be parsed as an integer (y). Type \"help {command}\" for formatting information.", LogLevel.Info);
                    return;
                }
            }
            else //if x y arguments were NOT provided
            {
                //get the player's current tile position
                Vector2 tile = Game1.player.Tile;
                x = (int)tile.X;
                y = (int)tile.Y;
            }

            GameLocation location;

            if (args.Length >= 3) //if a location argument was provided
            {
                //combine the remaining arguments into a single string
                string locationName = args[2]; //get the first location argument
                for (int count = 3; count < args.Length; count++) //for each argument after args[2]
                {
                    locationName += " " + args[count]; //add this argument to the location name, separated by a space
                }

                location = Game1.getLocationFromName(locationName); //attempt to get a location with the combined name
                if (location == null) //if no location matched the name
                {
                    ModEntry.Instance.Monitor.Log($"No location named \"{locationName}\" could be found. Type \"help {command}\" for formatting information.", LogLevel.Info);
                    return;
                }
            }
            else
            {
                location = Game1.player.currentLocation; //use the player's current location
                if (location == null) //if the player's location was null
                    return; //do nothing
            }

            //all necessary information has been parsed

            if (location.getLargeTerrainFeatureAt(x, y) is Bush bush && bush.size.Value != 4) //if there is a bush at this tile AND it's not a walnut bush
                location.largeTerrainFeatures.Remove(bush); //remove the bush
            else if (args.Length < 2) //if no bush was found AND this tile is the player's position
            {
                switch (Game1.player.FacingDirection) //based on which direction the player is facing
                {
                    case 0: //up
                        y--; //up 1 tile
                        break;
                    case 1: //right
                        x++; //right 1 tile
                        break;
                    case 2: //down
                        y++; //down 1 tile
                        break;
                    case 3: //left
                        x--; //left 1 tile
                        break;
                }

                if (location.getLargeTerrainFeatureAt(x, y) is Bush frontBush && frontBush.size.Value != 4) //if there is a bush on the tile in front of the player AND it's not a walnut bush
                {
                    location.largeTerrainFeatures.Remove(frontBush); //remove the bush
                }
            }
        }
    }
}
