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
        /// <summary>SMAPI console command. Creates a new bush with the specified size.</summary>
        /// <param name="command">The console command's name.</param>
        /// <param name="args">The space-delimited arguments entered with the command.</param>
        public static void AddBush(string command, string[] args)
        {
            if (!Context.IsWorldReady) //if the player is not currently in a loaded game
                return; //do nothing

            if (args.Length <= 0) //"add_bush" (invalid)
            {
                ModEntry.Instance.Monitor.Log($"Invalid number of arguments. Please include a bush size, e.g. \"{command} {Bush.largeBush}\". Type \"help {command}\" for formatting information.", LogLevel.Info);
                return;
            }
            else if (args.Length == 2) //"add_bush <size> [x]" (invalid)
            {
                ModEntry.Instance.Monitor.Log($"Invalid number of arguments. Please include both X and Y, e.g. \"{command} {Bush.largeBush} 64 19\". Type \"help {command}\" for formatting information.", LogLevel.Info);
                return;
            }

            int? size = ParseBushSize(args[0]); //attempt to parse the size argument

            if (size == null) //if parsing failed
            {
                ModEntry.Instance.Monitor.Log($"\"{args[0]}\" is not a recognized bush size. Type \"help {command}\" for formatting information.", LogLevel.Info);
                return;
            }

            int x;
            int y;

            if (args.Length >= 3) //if x y arguments were provided
            {
                if (int.TryParse(args[1], out x) == false) //try to parse the x argument; if it fails,
                {
                    ModEntry.Instance.Monitor.Log($"\"{args[1]}\" could not be parsed as an integer (x). Type \"help {command}\" for formatting information.", LogLevel.Info);
                    return;
                }
                else if (int.TryParse(args[2], out y) == false) //try to parse the y argument; if it fails,
                {
                    ModEntry.Instance.Monitor.Log($"\"{args[2]}\" could not be parsed as an integer (y). Type \"help {command}\" for formatting information.", LogLevel.Info);
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

            if (args.Length >= 4) //if a location argument was provided
            {
                //combine the remaining arguments into a single string
                string locationName = args[3]; //get the first location argument
                for (int count = 4; count < args.Length; count++) //for each argument after args[3]
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

            Bush bush = new Bush(new Vector2(x, y), size.Value, location); //create the new bush

            Rectangle playerBox = Game1.player.GetBoundingBox();

            while (playerBox.Intersects(bush.getBoundingBox())) //while this bush is colliding with the player
            {
                switch (Game1.player.FacingDirection) //based on which direction the player is facing
                {
                    default: //unknown facing position (should be unreachable)
                    case 0: //up
                        bush.tilePosition.Value = new Vector2(bush.tilePosition.X, bush.tilePosition.Y - 1); //move bush up 1 tile
                        break;
                    case 1: //right
                        bush.tilePosition.Value = new Vector2(bush.tilePosition.X + 1, bush.tilePosition.Y); //move bush right 1 tile
                        break;
                    case 2: //down
                        bush.tilePosition.Value = new Vector2(bush.tilePosition.X, bush.tilePosition.Y + 1); //move bush down 1 tile
                        break;
                    case 3: //left
                        bush.tilePosition.Value = new Vector2(bush.tilePosition.X - 1, bush.tilePosition.Y); //move bush left 1 tile
                        break;
                }
            }

            location.largeTerrainFeatures.Add(bush); //add the bush to the location
        }

        /// <summary>Parses the "size" argument of a console command. Returns null if parsing fails.</summary>
        /// <param name="size">The bush size argument.</param>
        /// <returns>The integer representing this bush's size. Null if parsing fails.</returns>
        private static int? ParseBushSize(string size)
        {
            switch (size.ToLower()) //based on the size string (converted to lowercase)
            {
                case "0":
                case "s":
                case "small":
                    return Bush.smallBush;
                case "1":
                case "m":
                case "med":
                case "medium":
                    return Bush.mediumBush;
                case "2":
                case "l":
                case "large":
                    return Bush.largeBush;
                case "3":
                case "t":
                case "green":
                case "greentea":
                case "greenteabush":
                case "tea":
                case "teabush":
                    return Bush.greenTeaBush;
                default:
                    return null;
            }
        }
    }
}
