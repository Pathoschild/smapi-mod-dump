using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using UiModSuite.UiMods;

namespace UiModSuite.Options {
    internal class ModOptionsPageButton : IClickableMenu {

        public Rectangle bounds = new Rectangle();
        private ModOptionsPageHandler optionPageHandler;
        private bool hasClicked = false;

        /// <summary>
        /// This is the tab button that is drawn on the main GameMenu. It opens a ModOptionsPage
        /// </summary>
        public ModOptionsPageButton( ModOptionsPageHandler optionPageHandler ) {

            this.optionPageHandler = optionPageHandler;

            width = 64;
            height = 64;

            var gameMenu = (GameMenu) Game1.activeClickableMenu;

            int offsetX = 200;
            int offsetY = 16;

            xPositionOnScreen = gameMenu.xPositionOnScreen + gameMenu.width - offsetX;
            yPositionOnScreen = gameMenu.yPositionOnScreen + offsetY;

            bounds.Width = width;
            bounds.Height = height;
            bounds.X = xPositionOnScreen;
            bounds.Y = yPositionOnScreen;

            ControlEvents.MouseChanged += onLeftClick;
        }

        public void onLeftClick( object sender, EventArgsMouseStateChanged e ) {

            if( e.NewState.LeftButton != ButtonState.Pressed || !( Game1.activeClickableMenu is GameMenu ) ) {
                hasClicked = false;
                return;
            }

            if( ( Game1.activeClickableMenu as GameMenu ).currentTab == GameMenu.mapTab ) {
                return;
            }

            if ( bounds.Contains( e.NewPosition.X, e.NewPosition.Y ) && !hasClicked ) {
                base.receiveLeftClick( e.NewPosition.X, e.NewPosition.Y, true );
                optionPageHandler.setActiveClickableMenuToModOptionsPage();
                Game1.playSound( "smallSelect" );
                hasClicked = true;
            }

        }

        public override void draw( SpriteBatch b ) {
            base.draw( b );

            var gameMenu = ( GameMenu ) Game1.activeClickableMenu;

            if( gameMenu.currentTab != 8 ) {
                yPositionOnScreen = Game1.activeClickableMenu.yPositionOnScreen + 16;
            } else {
                yPositionOnScreen = Game1.activeClickableMenu.yPositionOnScreen + 24;
            }

            // Draw tab
            Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( xPositionOnScreen, yPositionOnScreen ), new Rectangle( 16, 368, 16, 16 ), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f );

            // Draw icon
            b.Draw( Game1.mouseCursors, new Vector2( xPositionOnScreen + 8, yPositionOnScreen + 14 ), new Rectangle( 128 / 4, 2688 / 4, 64 / 4, 64 / 4 ), Color.White, 0, Vector2.Zero, 3f, SpriteEffects.None, 1 );

            // Draw hover text
            if( bounds.Contains( Game1.getMouseX(), Game1.getMouseY() ) ) {
                IClickableMenu.drawHoverText( Game1.spriteBatch, "Mod Options", Game1.smallFont );
            }

            drawMouse( b );
        }

        public override void receiveRightClick( int x, int y, bool playSound = true ) { }

    }
}