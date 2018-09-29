using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using UiModSuite.Options;


namespace UiModSuite.UiMods {
    internal class ShowTravelingMerchant {

        List<int> daysMerchantVisits = new List<int>() { 5, 7, 12, 14, 19, 21, 26, 28 };

        /// <summary>
        /// This mod shows an icon when the traveling merchant is in town
        /// </summary>
        public void toggleShowTravelingMerchant() {
            GraphicsEvents.OnPreRenderHudEvent -= drawTravelingMerchant;

            if( ModOptionsPage.getCheckboxValue( ModOptionsPage.Setting.SHOW_TRAVELING_MERCHANT ) == true ) {
                GraphicsEvents.OnPreRenderHudEvent += drawTravelingMerchant;
            }
        }

        /// <summary>
        /// Draw it!
        /// </summary>
        private void drawTravelingMerchant( object sender, EventArgs e ) {
            if( daysMerchantVisits.Contains( Game1.dayOfMonth )  && Game1.eventUp == false ) {
                var clickableTextureComponent = new ClickableTextureComponent( new Rectangle( IconHandler.getIconXPosition(), 260, 40, 40 ), Game1.content.Load<Texture2D>( "LooseSprites\\Cursors" ), new Rectangle( 192, 1411, 20, 20 ), 2 );
                clickableTextureComponent.draw( Game1.spriteBatch );

                if( clickableTextureComponent.containsPoint( Game1.getMouseX(), Game1.getMouseY() ) ) {
                    string tooltip = $"Traveling merchant is in town!";
                    IClickableMenu.drawHoverText( Game1.spriteBatch, tooltip, Game1.dialogueFont );
                }
            }
        }

    }
}