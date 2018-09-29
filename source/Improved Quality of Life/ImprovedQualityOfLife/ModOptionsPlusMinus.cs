using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Demiacle.ImprovedQualityOfLife {
    class ModOptionsPlusMinus : OptionsPlusMinus {

        /// <summary>
        /// A custom PlusMinus class for AlterTimeSpeed mod
        /// </summary>
        public ModOptionsPlusMinus( string label, int defaultSetting, List<string> options, int x = -1, int y = -1 ) : base( label, -2, options, options, x, y ) {

            if( ModEntry.modData.intOptions.ContainsKey( label ) ) {
                selected = ModEntry.modData.intOptions[ label ];
            } else {
                selected = defaultSetting;
                ModEntry.modData.intOptions[ label ] = selected;
            }
        }

        /// <summary>
        /// Changes the selected option only usable currently for AlterTimeSpeed mod
        /// </summary>
        public override void receiveLeftClick( int x, int y ) {

            var minusButton = (Rectangle) typeof( OptionsPlusMinus ).GetField( "minusButton", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( this );
            var plusButton = (Rectangle) typeof( OptionsPlusMinus ).GetField( "plusButton", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( this );

            if( this.greyedOut || this.options.Count <= 0 )
                return;
            int previousOptions = this.selected;

            if( minusButton.Contains( x, y ) && this.selected != 0 ) {
                this.selected = this.selected - 1;
                Game1.playSound( "drumkit6" );
            } else if( plusButton.Contains( x, y ) && this.selected != this.options.Count - 1 ) {
                this.selected = this.selected + 1;
                Game1.playSound( "drumkit6" );
            }

            if( this.selected < 0 )
                this.selected = 0;

            else if( this.selected >= this.options.Count )
                this.selected = this.options.Count - 1;

            int updatedOption = this.selected;

            if( previousOptions != updatedOption ) {
                ModEntry.modData.intOptions[ label ] = selected;
                ModEntry.updateModData();
            }
        }

    }
}
