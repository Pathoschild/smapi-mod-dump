using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using UiModSuite.UiMods;

namespace UiModSuite.Options {
    class ModOptionsPlusMinus : ModOptionsElement {

        public static Rectangle minusButtonSource = new Rectangle( 177, 345, 7, 8 );
        public static Rectangle plusButtonSource = new Rectangle( 184, 345, 7, 8 );
        public List<string> options = new List<string>();
        public const int pixelsWide = 7;
        public int selected;
        public bool isChecked;
        public static bool snapZoomPlus;
        public static bool snapZoomMinus;
        private Rectangle minusButton;
        private Rectangle plusButton;

        /// <summary>
        /// Plus Minus option for the ModOptionsPage ** UNTESTED
        /// </summary>
        public ModOptionsPlusMinus( string label, int whichOption, List<string> options, int x = -1, int y = -1 )
          : base( label, x, y, 7 * Game1.pixelZoom, 7 * Game1.pixelZoom, whichOption ) {
            this.options = options;
            // set value to loaded option value
            if( x == -1 )
                x = 8 * Game1.pixelZoom;
            if( y == -1 )
                y = 4 * Game1.pixelZoom;
            this.bounds = new Rectangle( x, y, 7 * Game1.pixelZoom * 2 + ( int ) Game1.dialogueFont.MeasureString( "%%%%" ).X, 8 * Game1.pixelZoom );
            this.label = label;
            this.whichOption = whichOption;
            this.minusButton = new Rectangle( x, 4 + Game1.pixelZoom * 3, 7 * Game1.pixelZoom, 8 * Game1.pixelZoom );
            this.plusButton = new Rectangle( this.bounds.Right - 8 * Game1.pixelZoom, 4 + Game1.pixelZoom * 3, 7 * Game1.pixelZoom, 8 * Game1.pixelZoom );
        }

        public override void receiveLeftClick( int x, int y ) {
            if( this.greyedOut || this.options.Count <= 0 )
                return;
            int selected1 = this.selected;
            if( this.minusButton.Contains( x, y ) && this.selected != 0 ) {
                this.selected = this.selected - 1;
                OptionsPlusMinus.snapZoomMinus = true;
                Game1.playSound( "drumkit6" );
            } else if( this.plusButton.Contains( x, y ) && this.selected != this.options.Count - 1 ) {
                this.selected = this.selected + 1;
                OptionsPlusMinus.snapZoomPlus = true;
                Game1.playSound( "drumkit6" );
            }
            if( this.selected < 0 )
                this.selected = 0;
            else if( this.selected >= this.options.Count )
                this.selected = this.options.Count - 1;
            int selected2 = this.selected;
            if( selected1 == selected2 )
                return;
            Game1.options.changeDropDownOption( this.whichOption, this.selected, this.options );
        }

        public override void draw( SpriteBatch b, int slotX, int slotY ) {
            b.Draw( Game1.mouseCursors, new Vector2( ( float ) ( slotX + this.minusButton.X ), ( float ) ( slotY + this.minusButton.Y ) ), new Rectangle?( OptionsPlusMinus.minusButtonSource ), Color.White * ( this.greyedOut ? 0.33f : 1f ) * ( this.selected == 0 ? 0.5f : 1f ), 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, SpriteEffects.None, 0.4f );
            b.DrawString( Game1.dialogueFont, this.selected >= this.options.Count || this.selected == -1 ? "" : this.options[ this.selected ], new Vector2( ( float ) ( slotX + this.minusButton.X + this.minusButton.Width + Game1.pixelZoom ), ( float ) ( slotY + this.minusButton.Y ) ), Game1.textColor );
            b.Draw( Game1.mouseCursors, new Vector2( ( float ) ( slotX + this.plusButton.X ), ( float ) ( slotY + this.plusButton.Y ) ), new Rectangle?( OptionsPlusMinus.plusButtonSource ), Color.White * ( this.greyedOut ? 0.33f : 1f ) * ( this.selected == this.options.Count - 1 ? 0.5f : 1f ), 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, SpriteEffects.None, 0.4f );
            if( OptionsPlusMinus.snapZoomMinus ) {
                Game1.setMousePosition( slotX + this.minusButton.Center.X, slotY + this.minusButton.Center.Y );
                OptionsPlusMinus.snapZoomMinus = false;
            } else if( OptionsPlusMinus.snapZoomPlus ) {
                Game1.setMousePosition( slotX + this.plusButton.Center.X, slotY + this.plusButton.Center.Y );
                OptionsPlusMinus.snapZoomPlus = false;
            }
            base.draw( b, slotX, slotY );
        }
    }
}
