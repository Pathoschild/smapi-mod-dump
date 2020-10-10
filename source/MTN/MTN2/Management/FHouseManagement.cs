/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/MTN2
**
*************************************************/

using Microsoft.Xna.Framework;
using MTN2.MapData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Management {
    internal class FHouseManagement {
        private readonly FarmManagement farmManagement;

        /// <summary></summary>
        public int FurnitureLayout {
            get {
                return farmManagement.LoadedFarm.FurnitureLayoutFromCanon;
            }
        }

        /// <summary></summary>
        public Point FrontPorch {
            get {
                return new Point(farmManagement.LoadedFarm.FarmHouse.PointOfInteraction.X, farmManagement.LoadedFarm.FarmHouse.PointOfInteraction.Y);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="farmManagement"></param>
        public FHouseManagement(FarmManagement farmManagement) {
            this.farmManagement = farmManagement;
        }

        /// <summary>
        /// Gets the X and Y coordinates of the top left point of the Farmhouse in Vector2
        /// Form. Allows offsetting the coordinates. 
        /// </summary>
        /// <param name="OffsetX">The Offset value for the X Coordinate</param>
        /// <param name="OffsetY">The Offset value for the Y Coordinate</param>
        /// <returns>The coordinates in Vector2 form.</returns>
        public Vector2 FarmHouseCoords(float OffsetX, float OffsetY, bool Canon) {
            if (Canon || farmManagement.LoadedFarm.FarmHouse == null) {
                return FarmHouseCoordsCanon(OffsetX, OffsetY);
            }
            Placement? Coordinates = farmManagement.LoadedFarm.FarmHouse.Coordinates;
            return new Vector2((Coordinates.Value.X * 64f) + OffsetX, (Coordinates.Value.Y * 64f) + OffsetY);
        }

        /// <summary>
        /// Gets the original (Canon) farmhouse coordinate values in Vector2 form. Allows
        /// offsetting the coordinates
        /// </summary>
        /// <param name="OffsetX">The Offset value for the X Coordinate</param>
        /// <param name="OffsetY">The Offset value for the Y Coordinate</param>
        /// <returns>The canon coordinates in Vector2 form.</returns>
        protected Vector2 FarmHouseCoordsCanon(float OffsetX, float OffsetY) {
            return new Vector2(3712f + OffsetX, 520f + OffsetY);
        }

        /// <summary>
        /// Computes and returns the Layer Depth value needed to properly render the Farmhouse.
        /// Returns the original (canon) Layer Depth if the farm is not a custom farm.
        /// </summary>
        /// <returns>The proper layer depth. Used in Spritebatch.Draw</returns>
        public float FarmHouseLayerDepth(bool Canon) {
            if (Canon || farmManagement.LoadedFarm.FarmHouse == null) {
                return 0.075f;
            } else {
                return ((farmManagement.LoadedFarm.FarmHouse.PointOfInteraction.Y - 5 + 3) * 64) / 10000f;
            }
        }
    }
}
