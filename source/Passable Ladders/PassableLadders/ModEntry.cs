using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace PassableLadders
{
    public class ModEntry : Mod
    {
        //List of Ladders that have been found
        public SerializableDictionary<Vector2, Tile> ladderList = new SerializableDictionary<Vector2, Tile>();

        //Property to make a tile "passable"
        public PropertyValue propValue = new PropertyValue("Passable");

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            GameEvents.FourthUpdateTick += this.CheckAreaAroundPlayerForLadder;
            LocationEvents.ObjectsChanged += this.PlacedStairs;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>
        /// Method used by event methods to get the tiles adjacent to the player and check if any of them are a ladder. This is determined by checking the index of the tile for 173.
        /// </summary>
        private void CheckAdjacentTilesForLadder()
        {
            List<Vector2> adjTiles = Utility.getAdjacentTileLocations(Game1.player.getTileLocation());
            Layer currentLayer = Game1.currentLocation.map.GetLayer("Buildings");

            foreach (Vector2 tile in adjTiles)
            {
                Tile currentTile = currentLayer.PickTile(new Location((int)tile.X, (int)tile.Y) * Game1.tileSize, Game1.viewport.Size);
                if (currentTile != null && currentTile.TileIndex == 173 && !ladderList.ContainsKey(new Vector2((int)tile.X, (int)tile.Y)))
                {
                    this.Monitor.Log($"Ladder found at {tile}", LogLevel.Info);
                    currentTile.TileIndexProperties.Add(new KeyValuePair<string, PropertyValue>("Passable", propValue));
                    ladderList.Add(new Vector2((int)tile.X, (int)tile.Y), currentTile);
                }
            }
        }

        /// <summary>Method invoked every fourth update tick. This is to be fast enough for a player running up near a ladder for the code to add a passable property to it. It's not needed every update tick.</summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event data</param>
        private void CheckAreaAroundPlayerForLadder(object sender, EventArgs e)
        {
            if (Game1.currentLocation is MineShaft)
            {
                CheckAdjacentTilesForLadder();
            }
        }

        /// <summary>
        /// Method invoked when the player creates an object. This is to account for if the player creates and places stairs, which would turn into a ladder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlacedStairs(object sender, EventArgsLocationObjectsChanged e)
        {
            if(Game1.currentLocation is MineShaft)
            {
                CheckAdjacentTilesForLadder();
            }
        }
    }
}
