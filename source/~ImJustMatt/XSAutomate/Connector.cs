/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;

namespace XSAutomate
{
    internal class Connector : IAutomatable
    {
        /// <summary>Construct an instance.</summary>
        /// <param name="location">The location which contains the machine.</param>
        /// <param name="tileArea">The tile area covered by the machine.</param>
        public Connector(GameLocation location, Rectangle tileArea)
        {
            Location = location;
            TileArea = tileArea;
        }
        
        /// <inheritdoc />
        public Connector(GameLocation location, Vector2 tile)
            : this(location, new Rectangle((int) tile.X, (int) tile.Y, 1, 1))
        {
        }
        
        /// <inheritdoc />
        public GameLocation Location { get; }
        
        /// <inheritdoc />
        public Rectangle TileArea { get; }
    }
}