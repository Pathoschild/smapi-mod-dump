using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;

namespace Demiacle.ImprovedQualityOfLife {

    /// <summary>
    /// This horse overrides and makes a horse small enough to travel between 1 square tiles. This horse is replaced with the normal horse right before saving so it won't interfere with the serializer
    /// </summary>
    internal class BetterHorse : Horse {
        public BetterHorse() : base( 0, 0 ) {
            speed = 9;
        }

        /// <summary>
        /// Overrides the normal hit detection behavior
        /// </summary>
        public override Rectangle GetBoundingBox() {
            Rectangle boundingBox = base.GetBoundingBox();

            if( mounting ) {
                return boundingBox;
            }

            boundingBox.Inflate( -14 - Game1.pixelZoom, 0);
            return boundingBox;
        }

    }
}