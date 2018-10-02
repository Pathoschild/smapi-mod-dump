using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using UiModSuite.Options;
using static UiModSuite.Options.ModOptionsPage;

namespace UiModSuite.UiMods {
    class LuckOfDay {

        private ClickableTextureComponent icon;
        private string hoverText = "";

        /// <summary>
        /// This mod draws an icon displaying the luck of the day
        /// </summary>
        public void toggleOption() {

            LocationEvents.CurrentLocationChanged -= adjustIconXToBlackBorder;
            GraphicsEvents.OnPreRenderHudEvent -= drawDiceIcon;
            GraphicsEvents.OnPostRenderHudEvent -= drawHoverTextOverEverything;

            if( getCheckboxValue( Setting.SHOW_LUCK_ICON ) ) {
                adjustIconXToBlackBorder( null, null );
                LocationEvents.CurrentLocationChanged += adjustIconXToBlackBorder;
                GraphicsEvents.OnPreRenderHudEvent += drawDiceIcon;
                GraphicsEvents.OnPostRenderHudEvent += drawHoverTextOverEverything;
            }
        }

        /// <summary>
        /// Draws the dice icon
        /// </summary>
        internal void drawDiceIcon( object sender, EventArgs e ) {
            if( Game1.eventUp ) {
                return;
            }

            //TODO refactor this into new day
            var color = new Color( Color.White.ToVector4() );

            if( Game1.dailyLuck > 0.04d ) {
                hoverText = "You're feelin' lucky!!";
                color.B = 155;
                color.R = 155;
            } else if( Game1.dailyLuck < -0.04d ) {
                hoverText = "Maybe you should stay home today...";
                color.B = 155;
                color.G = 155;
            } else if( -0.04d <= Game1.dailyLuck && Game1.dailyLuck < 0 ){
                hoverText = "You're not feeling lucky at all today...";
                color.B = 165;
                color.G = 165;
                color.R = 165;
                color *= 0.8f;
            } else {
                hoverText = "Feelin' lucky... but not too lucky";
            }

            // Set icon position
            icon.bounds.X = IconHandler.getIconXPosition();

            icon.draw( Game1.spriteBatch, color, 1 );
        }

        /// <summary>
        /// Shifts the icon if there is a black border
        /// </summary>
        internal void adjustIconXToBlackBorder( object sender, EventArgsCurrentLocationChanged e ) {
            icon = new ClickableTextureComponent( "", new Rectangle( ( int ) DemiacleUtility.getWidthInPlayArea() - 134, 260, 10 * Game1.pixelZoom, 10 * Game1.pixelZoom ), "", "", Game1.mouseCursors, new Rectangle( 50, 428, 10, 14 ), Game1.pixelZoom );
        }

        /// <summary>
        /// Draws tooltip on hover
        /// </summary>
        internal void drawHoverTextOverEverything( object sender, EventArgs e ) {
            if( icon.containsPoint( Game1.oldMouseState.X, Game1.oldMouseState.Y ) ) {
                IClickableMenu.drawHoverText( Game1.spriteBatch, hoverText, Game1.dialogueFont );
            }
        }

    }
}
