/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/demiacle/UiModSuite
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using UiModSuite.UiMods;
 
namespace UiModSuite.Options {
    public class ModOptionsElement {

        public const int defaultX = 8;
        public const int defaultY = 4;
        public const int defaultPixelWidth = 9;
        public Rectangle bounds;
        public string label;
        public int whichOption;
        public bool greyedOut;
        public Action toggleOptionDelegate;

        /// <summary>
        /// The base class for all custom ModOptions
        /// </summary>
        public ModOptionsElement( string label ) {
            this.label = label;
            this.bounds = new Rectangle( 8 * Game1.pixelZoom, 4 * Game1.pixelZoom, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom );
            this.whichOption = -1;
        }

        public ModOptionsElement( string label, int x, int y, int width, int height, int whichOption = -1 ) {

            if( x == -1 )
                x = 8 * Game1.pixelZoom;
            if( y == -1 )
                y = 4 * Game1.pixelZoom;

            this.bounds = new Rectangle( x, y, width, height );
            this.label = label;
            this.whichOption = whichOption;
        }

        public ModOptionsElement( string label, Rectangle bounds, int whichOption ) {

            this.whichOption = whichOption;
            this.label = label;
            this.bounds = bounds;
        }

        public virtual void receiveLeftClick( int x, int y ) {
        }

        public virtual void leftClickHeld( int x, int y ) {
        }

        public virtual void leftClickReleased( int x, int y ) {
        }

        public virtual void receiveKeyPress( Keys key ) {
        }

        public virtual void draw( SpriteBatch b, int slotX, int slotY ) {
            if( this.whichOption == -1 )
                SpriteText.drawString( b, this.label, slotX + this.bounds.X, slotY + this.bounds.Y + Game1.pixelZoom * 3, 999, -1, 999, 1f, 0.1f, false, -1, "", -1 );
            else
                Utility.drawTextWithShadow( b, this.label, Game1.dialogueFont, new Vector2( ( float ) ( slotX + this.bounds.X + this.bounds.Width + Game1.pixelZoom * 2 ), ( float ) ( slotY + this.bounds.Y ) ), this.greyedOut ? Game1.textColor * 0.33f : Game1.textColor, 1f, 0.1f, -1, -1, 1f, 3 );
        }
    }
}
