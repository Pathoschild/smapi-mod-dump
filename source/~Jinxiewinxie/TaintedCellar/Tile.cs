/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jinxiewinxie/StardewValleyMods
**
*************************************************/

namespace TaintedCellar
{
    /// <summary>Defines an override to apply to a tile position.</summary>
    public class Tile
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The tilesheet ID to modify.</summary>
        public string Tilesheet;

        /// <summary>The layer index to modify.</summary>
        public int LayerIndex;

        /// <summary>The X tile coordinate.</summary>
        public int X;

        /// <summary>The X tile coordinate.</summary>
        public int Y;

        /// <summary>The tile ID in the tilesheet.</summary>
        public int TileIndex;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="layerIndex">The layer index to modify.</param>
        /// <param name="x">The X tile coordinate.</param>
        /// <param name="y">The X tile coordinate.</param>
        /// <param name="tileIndex">The tilesheet ID to modify.</param>
        /// <param name="tilesheet">The tilesheet to modify.</param>
        public Tile(int layerIndex, int x, int y, int tileIndex, string tilesheet)
        {
            this.LayerIndex = layerIndex;
            this.X = x;
            this.Y = y;
            this.TileIndex = tileIndex;
            this.Tilesheet = tilesheet;
        }
    }
}
