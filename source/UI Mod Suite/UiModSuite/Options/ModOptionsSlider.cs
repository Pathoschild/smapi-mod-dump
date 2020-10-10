/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/demiacle/UiModSuite
**
*************************************************/

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
    public class ModOptionsSlider : OptionsElement {

        public static Rectangle sliderBGSource = new Rectangle( 403, 383, 6, 6 );
        public static Rectangle sliderButtonRect = new Rectangle( 420, 441, 10, 6 );
        public const int pixelsWide = 48;
        public const int pixelsHigh = 6;
        public const int sliderButtonWidth = 10;
        public const int sliderMaxValue = 100;
        public int value;

        /// <summary>
        /// Options slider for ModOptionsPage **UNTESTED
        /// </summary>
        public ModOptionsSlider( string label, int whichOption, int x = -1, int y = -1 )
          : base( label, x, y, 48 * Game1.pixelZoom, 6 * Game1.pixelZoom, whichOption ) {
            // set value to loaded settings value
        }

        public override void leftClickHeld( int x, int y ) {
            if( this.greyedOut )
                return;
            base.leftClickHeld( x, y );
            this.value = x >= this.bounds.X ? ( x <= this.bounds.Right - 10 * Game1.pixelZoom ? ( int ) ( ( double ) ( ( float ) ( x - this.bounds.X ) / ( float ) ( this.bounds.Width - 10 * Game1.pixelZoom ) ) * 100.0 ) : 100 ) : 0;
            Game1.options.changeSliderOption( this.whichOption, this.value );
        }

        public override void receiveLeftClick( int x, int y ) {
            if( this.greyedOut )
                return;
            base.receiveLeftClick( x, y );
            this.leftClickHeld( x, y );
        }

        public override void draw( SpriteBatch b, int slotX, int slotY ) {
            base.draw( b, slotX, slotY );
            IClickableMenu.drawTextureBox( b, Game1.mouseCursors, OptionsSlider.sliderBGSource, slotX + this.bounds.X, slotY + this.bounds.Y, this.bounds.Width, this.bounds.Height, Color.White, ( float ) Game1.pixelZoom, false );
            b.Draw( Game1.mouseCursors, new Vector2( ( float ) ( slotX + this.bounds.X ) + ( float ) ( this.bounds.Width - 10 * Game1.pixelZoom ) * ( ( float ) this.value / 100f ), ( float ) ( slotY + this.bounds.Y ) ), new Rectangle?( OptionsSlider.sliderButtonRect ), Color.White, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, SpriteEffects.None, 0.9f );
        }
    }

}
