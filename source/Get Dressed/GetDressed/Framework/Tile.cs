namespace GetDressed.Framework
{
    /// <summary>Defines an override to apply to a tile position.</summary>
    internal class Tile
    {
        /*********
        ** Properties
        *********/
        /// <summary>The tile layer.</summary>
        public TileLayer Layer { get; }

        /// <summary>The tile layer name.</summary>
        public string LayerName { get; }

        /// <summary>The X tile coordinate.</summary>
        public int X { get; }

        /// <summary>The Y tile coordinate.</summary>
        public int Y { get; }

        /// <summary>The tile ID in the tilesheet.</summary>
        public int TileID { get; }

        /// <summary>The tilesheet for the <see cref="TileID"/>.</summary>
        public string Tilesheet { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="layer">The tile layer.</param>
        /// <param name="x">The X tile coordinate.</param>
        /// <param name="y">The Y tile coordinate.</param>
        /// <param name="tileID">The tile ID in the tilesheet.</param>
        /// <param name="tilesheet">The tilesheet for the <paramref name="tileID"/>.</param>
        public Tile(TileLayer layer, int x, int y, int tileID, string tilesheet)
        {
            this.Layer = layer;
            this.LayerName = layer.ToString();
            this.X = x;
            this.Y = y;
            this.TileID = tileID;
            this.Tilesheet = tilesheet;
        }
    }
}
