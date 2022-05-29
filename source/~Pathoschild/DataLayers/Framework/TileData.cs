/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace Pathoschild.Stardew.DataLayers.Framework
{
    /// <summary>Metadata for a tile.</summary>
    internal class TileData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The associated legend entry.</summary>
        public LegendEntry Type { get; }

        /// <summary>The tile position.</summary>
        public Vector2 TilePosition { get; }

        /// <summary>The overlay color.</summary>
        public Color Color { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="tile">The tile position.</param>
        /// <param name="type">The associated legend entry.</param>
        /// <param name="color">The overlay color.</param>
        public TileData(Vector2 tile, LegendEntry type, Color color)
        {
            this.TilePosition = tile;
            this.Type = type;
            this.Color = color;
        }

        /// <summary>Construct an instance.</summary>
        /// <param name="tile">The tile position.</param>
        /// <param name="type">The associated legend entry.</param>
        public TileData(Vector2 tile, LegendEntry type)
            : this(tile, type, type.Color) { }
    }
}
