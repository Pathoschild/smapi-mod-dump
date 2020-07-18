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
using Harmony;

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
                x = Game1.player.getTileX();
                y = Game1.player.getTileY();
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

            if (location.getLargeTerrainFeatureAt(x, y) is Bush bush) //if there is a bush at this tile
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

                if (location.getLargeTerrainFeatureAt(x, y) is Bush frontBush) //if there is a bush on the tile in front of the player
                {
                    location.largeTerrainFeatures.Remove(frontBush); //remove the bush
                }
            }
        }
    }
}
