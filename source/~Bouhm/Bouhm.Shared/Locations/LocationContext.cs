/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;

namespace Bouhm.Shared.Locations
{
    /// <summary>A location in the location graph with metadata and links to neighboring locations.</summary>
    internal class LocationContext
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The location type (e.g. outdoors, building, or room).</summary>
        public LocationType Type { get; set; }

        /// <summary>The name of the root outdoor location which directly or indirectly contains this location, if any.</summary>
        public string Root { get; set; }

        /// <summary>The name of the immediate parent which contains this location, if any.</summary>
        public string Parent { get; set; }

        /// <summary>The names of locations directly reachable via outgoing warps from this location, with the target tile position for each warp.</summary>
        public Dictionary<string, Vector2> Neighbors { get; set; } = new();

        /// <summary>The names of locations directly contained by this location.</summary>
        public List<string> Children { get; set; }

        /// <summary>The default entry tile for incoming warps to this location.</summary>
        public Vector2 Warp { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Get the pixel position on which incoming players will warp based on the <see cref="Warp"/> tile.</summary>
        public Vector2 GetWarpPixelPosition()
        {
            return LocationContext.GetWarpPixelPosition(this.Warp);
        }

        /// <summary>Get the pixel position on which incoming players will warp based on the given arrival tile.</summary>
        /// <param name="warpTile">The warp tile on which players will arrive.</param>
        public static Vector2 GetWarpPixelPosition(Vector2 warpTile)
        {
            return new(
                warpTile.X * Game1.tileSize + Game1.tileSize / 2,
                warpTile.Y * Game1.tileSize - Game1.tileSize * 3 / 2
            );
        }
    }
}
