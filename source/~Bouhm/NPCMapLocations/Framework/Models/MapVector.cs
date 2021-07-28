/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

using Newtonsoft.Json;

namespace NPCMapLocations.Framework.Models
{
    // Class for Location Vectors
    // Maps the tileX and tileY in a game location to the location on the map
    public class MapVector
    {
        [JsonConstructor]
        public MapVector(int x, int y)
        {
            this.MapX = x;
            this.MapY = y;
        }

        [JsonConstructor]
        public MapVector(int x, int y, int tileX, int tileY)
        {
            this.MapX = x;
            this.MapY = y;
            this.TileX = tileX;
            this.TileY = tileY;
        }

        public int TileX { get; set; } // tileX in a game location
        public int TileY { get; set; } // tileY in a game location
        public int MapX { get; set; } // Absolute position relative to map
        public int MapY { get; set; } // Absolute position relative to map
    }
}
