/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

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
            string tmxName = Utility.GetTMXBuildableName(loc.Name);
            int x = Game1.player.getTileX();
            int y = Game1.player.getTileY();
            int index = loc.getTileIndexAt(x, y, "Back");
            string type = loc.doesTileHaveProperty(x, y, "Type", "Back") ?? "[none]";
            string diggable = loc.doesTileHaveProperty(x, y, "Diggable", "Back");
            if (diggable == "T") { diggable = "Yes"; } else { diggable = "No"; };

            if (tmxName == null) //if this is a typical map
            {
                Utility.Monitor.Log($"Map name: {loc.Name}", LogLevel.Info);
            }
            else //if this is a buildable location added by TMXLoader
            {
                Utility.Monitor.Log($"Map name: {tmxName}", LogLevel.Info);
            }
            Utility.Monitor.Log($"Your location (x,y): {x},{y}", LogLevel.Info);
            Utility.Monitor.Log($"Terrain type: {type}", LogLevel.Info);
            Utility.Monitor.Log($"Diggable: {diggable}", LogLevel.Info);
            Utility.Monitor.Log($"Tile image index: {index}", LogLevel.Info);
        }
    }
}