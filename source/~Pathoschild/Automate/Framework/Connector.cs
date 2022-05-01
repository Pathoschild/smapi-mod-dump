/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

#nullable disable

using Microsoft.Xna.Framework;
using StardewValley;

namespace Pathoschild.Stardew.Automate.Framework
{
    /// <summary>An entity which connects machines and chests in a machine group, but otherwise has no logic of its own.</summary>
    internal class Connector : IAutomatable
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The location which contains the machine.</summary>
        public GameLocation Location { get; }

        /// <summary>The tile area covered by the machine.</summary>
        public Rectangle TileArea { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="location">The location which contains the machine.</param>
        /// <param name="tileArea">The tile area covered by the machine.</param>
        public Connector(GameLocation location, Rectangle tileArea)
        {
            this.Location = location;
            this.TileArea = tileArea;
        }

        /// <summary>Construct an instance.</summary>
        /// <param name="location">The location which contains the machine.</param>
        /// <param name="tile">The tile covered by the machine.</param>
        public Connector(GameLocation location, Vector2 tile)
            : this(location, new Rectangle((int)tile.X, (int)tile.Y, 1, 1)) { }
    }
}
