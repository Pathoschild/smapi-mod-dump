using Microsoft.Xna.Framework;
using StardewValley;

namespace AllCropsAllSeasons.Framework
{
    /// <summary>Contains metadata for a crop on a tile.</summary>
    internal class CropTileState
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The tile position on the farm.</summary>
        public Vector2 Tile { get; }

        /// <summary>The crop instance.</summary>
        public Crop Crop { get; }

        /// <summary>The dirt state (one of <see cref="StardewValley.TerrainFeatures.HoeDirt.dry"/> or <see cref="StardewValley.TerrainFeatures.HoeDirt.watered"/>).</summary>
        public int State { get; }

        /// <summary>The fertilizer applied to the dirt.</summary>
        public int Fertilizer { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="tile">The tile position on the farm.</param>
        /// <param name="crop">The crop instance.</param>
        /// <param name="state">The dirt state (one of <see cref="StardewValley.TerrainFeatures.HoeDirt.dry"/> or <see cref="StardewValley.TerrainFeatures.HoeDirt.watered"/>).</param>
        /// <param name="fertilizer">The fertilizer applied to the dirt.</param>
        public CropTileState(Vector2 tile, Crop crop, int state, int fertilizer)
        {
            this.Tile = tile;
            this.Crop = crop;
            this.State = state;
            this.Fertilizer = fertilizer;
        }
    }
}
