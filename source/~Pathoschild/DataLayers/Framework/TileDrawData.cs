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

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Pathoschild.Stardew.DataLayers.Framework
{
    /// <summary>Aggregate drawing metadata for a tile.</summary>
    internal class TileDrawData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The tile position.</summary>
        public Vector2 TilePosition { get; }

        /// <summary>The overlay colors to draw.</summary>
        public HashSet<Color> Colors { get; } = new HashSet<Color>();

        /// <summary>The border colors to draw.</summary>
        public IDictionary<Color, TileEdge> BorderColors { get; } = new Dictionary<Color, TileEdge>();


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="position">The tile position.</param>
        public TileDrawData(Vector2 position)
        {
            this.TilePosition = position;
        }
    }
}
