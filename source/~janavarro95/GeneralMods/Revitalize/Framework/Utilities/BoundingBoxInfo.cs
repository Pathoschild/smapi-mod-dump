using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Revitalize.Framework.Objects;
using StardewValley;

namespace Revitalize.Framework.Utilities
{
    /// <summary>
    /// Deals with calculating bounding boxes on objects.
    /// </summary>
    public class BoundingBoxInfo
    {
        /// <summary>
        /// The number of tiles in size for this bounding box;
        /// </summary>
        public Rectangle tileSize;
        /// <summary>
        /// The pixel offset for this bounding box dimensions.
        /// </summary>
        public Rectangle pixelOffsets;

        public BoundingBoxInfo()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="TileSize">How big in tiles this bounding box is.</param>
        public BoundingBoxInfo(Rectangle TileSize)
        {
            this.tileSize = TileSize;
            this.pixelOffsets = new Rectangle(0, 0, 0, 0);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="TileSize">How big in tiles this bounding box is.</param>
        /// <param name="PixelOffsets">The offset in size and position in pixels.</param>
        public BoundingBoxInfo(Rectangle TileSize,Rectangle PixelOffsets)
        {
            this.tileSize = TileSize;
            this.pixelOffsets = PixelOffsets;
        }

    }
}
