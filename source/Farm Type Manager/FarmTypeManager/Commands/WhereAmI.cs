using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace FarmTypeManager
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {
        ///<summary>Console command. Outputs the player's current location name, tile x/y coordinates, tile "Type" property (e.g. "Grass" or "Dirt"), tile "Diggable" status, and tile index.</summary>
        private void WhereAmI(string command, string[] args)
        {
            if (!Context.IsWorldReady) { return; } //if the player isn't in a fully loaded game yet, ignore this command

            GameLocation loc = Game1.currentLocation;
            int x = Game1.player.getTileX();
            int y = Game1.player.getTileY();
            int index = loc.getTileIndexAt(x, y, "Back");
            string type = loc.doesTileHaveProperty(x, y, "Type", "Back") ?? "[none]";
            string diggable = loc.doesTileHaveProperty(x, y, "Diggable", "Back");
            if (diggable == "T") { diggable = "Yes"; } else { diggable = "No"; };
            Monitor.Log($"Map name: {loc.Name}", LogLevel.Info);
			Monitor.Log($"Your location (x,y): {x},{y}", LogLevel.Info);
            Monitor.Log($"Terrain type: {type}", LogLevel.Info);
            Monitor.Log($"Diggable: {diggable}", LogLevel.Info);
            Monitor.Log($"Tile image index: {index}", LogLevel.Info);
        }
    }
}