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
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using UiModSuite.UiMods;

namespace UiModSuite.Options {
    public class ModOptionsCheckbox : ModOptionsElement {

        public static Rectangle sourceRectUnchecked = new Rectangle( 227, 425, 9, 9 );
        public static Rectangle sourceRectChecked = new Rectangle( 236, 425, 9, 9 );
        public const int pixelsWide = 9;
        public bool isChecked;

        /// <summary>
        /// This class creates a checkbox to be used in the ModOptionsPage
        /// </summary>
        public ModOptionsCheckbox( string label, int whichOption, Action toggleOptionDelegate, int x = -1, int y = -1 )
          : base( label, x, y, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom, whichOption ) {
            this.toggleOptionDelegate = toggleOptionDelegate;

            if ( ModEntry.modData.boolSettings.ContainsKey( whichOption ) == false ) {
                ModEntry.modData.boolSettings.Add( whichOption, true );
            }

            isChecked = ModEntry.modData.boolSettings[ whichOption ];
            toggleOptionDelegate.Invoke();
        }

        /// <summary>
        /// Changes the checkbox and updates ModData
        /// </summary>
        public override void receiveLeftClick( int x, int y ) {
            if( this.greyedOut )
                return;
            Game1.playSound( "drumkit6" );
            base.receiveLeftClick( x, y );
            this.isChecked = !this.isChecked;
            ModEntry.modData.boolSettings[ whichOption ] = isChecked;
            toggleOptionDelegate.Invoke();
        }

        /// <summary>
        /// Draws the checkbox
        /// </summary>
        public override void draw( SpriteBatch b, int slotX, int slotY ) {
            b.Draw( Game1.mouseCursors, new Vector2( ( float ) ( slotX + this.bounds.X ), ( float ) ( slotY + this.bounds.Y ) ), new Rectangle?( this.isChecked ? OptionsCheckbox.sourceRectChecked : OptionsCheckbox.sourceRectUnchecked ), Color.White * ( this.greyedOut ? 0.33f : 1f ), 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, SpriteEffects.None, 0.4f );
            base.draw( b, slotX, slotY );
        }

    }
}
