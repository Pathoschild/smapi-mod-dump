using Microsoft.Xna.Framework;
using StardewValley;

namespace UiModSuite.UiMods {
    internal class ExpPointDisplay {

        private float experiencePoints;
        private Vector2 position;
        private int alpha = 100;

        /// <summary>
        /// The experience point popup when exp is gained
        /// </summary>
        public ExpPointDisplay( float experiencePoints, Vector2 position ) {
            this.position = position;
            this.experiencePoints = experiencePoints;
        }

        internal void draw() {
            position.Y -= 0.5f;
            alpha -= 1;

            Color insideColor = Color.PaleTurquoise * ( alpha / 100f );
            Color borderColor = Color.DarkSlateGray * ( alpha / 100f );

            Game1.drawWithBorder( $"Exp {experiencePoints}", borderColor, insideColor, new Vector2( position.X - 28, position.Y - 130 ), 0, 0.8f, 0 );
        }

        /// <summary>
        /// Used for detecting when to remove
        /// </summary>
        internal bool isInvisible() {
            return ( alpha <= 2 );
        }

    }
}