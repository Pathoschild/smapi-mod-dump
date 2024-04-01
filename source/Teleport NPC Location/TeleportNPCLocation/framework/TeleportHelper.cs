/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chencrstu/TeleportNPCLocation
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;

namespace TeleportNPCLocation.framework
{
	public class TeleportHelper
	{

        /// <summary>teleport to NPC current Location.</summary>
        /// <param name="npc">The npc model.</param>
        public static void teleportToNPCLocation(NPC npc)
        {
            // Get npc location
            GameLocation location = npc.currentLocation;

            // GameLocation Location = Utility.fuzzyLocationSearch(locationName);
            if (location == null)
                return;

            Action teleportFunction = delegate {
                //Insert here the coordinates you want to teleport to
                //int[] offset = FindNPCAroundSpace(location, npc);
                int[] offset = { 0, 0, 0 };

                int X = npc.TilePoint.X + offset[0];
                int Y = npc.TilePoint.Y + offset[1];

               

                // The direction you want the Farmer to face after the teleport
                // 0 = up, 1 = right, 2 = down, 3 = left
                int direction = offset[2];

                // The teleport command itself
                Game1.warpFarmer(new LocationRequest(location.NameOrUniqueName, location.uniqueName.Value != null, location), X, Y, direction);
            };



            // Delayed action to be executed after a set time (here 0,1 seconds)
            // Teleporting without the delay may prove to be problematic
            DelayedAction.functionAfterDelay(teleportFunction, 100);


        }

        /// unused
        /// <summary>Find the empty tile around the npc</summary>
        /// <param name="location">The location model.</param>
        /// <param name="npc">The npc model.</param>
        private static int[] FindNPCAroundSpace(GameLocation location, NPC npc)
        {
            // Define offset array
            int[,] tileOffset = { { -1, 0, 1 }, { 1, 0, 3 }, { 0, -1, 2 }, { 0, 1, 0 }, { -1, -1, 1 }, { 1, -1, 3 }, { -1, 1, 1 }, { 1, 1, 3 } };
            int[] result = { 0, 0, 0 };

            for (int i = 0; i < tileOffset.GetLength(0); i++)
            {
                int xTile = npc.TilePoint.X + tileOffset[i, 0];
                int yTile = npc.TilePoint.Y + tileOffset[i, 1];

                if (location.doesTileHaveProperty(xTile, yTile, "Water", "Back") != null)
                    continue;

                if (location.doesTileHaveProperty(xTile, yTile, "Passable", "Buildings") == null)
                    continue;

                result = new int[] { tileOffset[i, 0], tileOffset[i, 1], tileOffset[i, 2] };

            }
            return result;
        }
    }
}

