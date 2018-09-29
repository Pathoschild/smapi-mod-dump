using StardewValley;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Demiacle.ImprovedQualityOfLife {
    class DemiacleUtility {

        /// <summary>
        /// Creates a dialogue that only shows up if a menu is not open. And retries every second to see if menu has closed
        /// This is needed to stop a bug of disapearing items on forced exits of menu
        /// </summary>
        public static void createSafeDelayedDialogue( string dialogue, int timer ) {
            Task.Factory.StartNew( () => {
                System.Threading.Thread.Sleep( timer );
                while( true ) {
                    System.Threading.Thread.Sleep( 1000 );
                    if( !( Game1.activeClickableMenu is StardewValley.Menus.GameMenu ) ) {
                        Game1.setDialogue( dialogue, true );
                        return;
                    }
                }
            } );
        }

        /// <summary>
        /// Gets the width of the current play area not including black borders
        /// </summary>
        /// <returns>Play area width</returns>
        public static float getWidthInPlayArea() {
            if( Game1.isOutdoorMapSmallerThanViewport() ) {
                float positionX = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right;
                int currentMapSize = ( Game1.currentLocation.map.Layers[ 0 ].LayerWidth * Game1.tileSize );
                float blackSpace = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - currentMapSize;
                positionX = positionX - ( blackSpace / 2 );
                return positionX;
            } else {
                return Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right;
            }
        }

        /// <summary>
        /// Gets the width of the current play area not including black borders
        /// </summary>
        /// <param name="location">The GameLocation of the play area to check</param>
        /// <returns>Play area width</returns>
        public static float getWidthInPlayArea( GameLocation location) {
            if( Game1.isOutdoorMapSmallerThanViewport() ) {
                float positionX = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right;
                int currentMapSize = ( location.map.Layers[ 0 ].LayerWidth * Game1.tileSize );
                float blackSpace = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - currentMapSize;
                positionX = positionX - ( blackSpace / 2 );
                return positionX;
            } else {
                return Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right;
            }
        }

        /// <summary>
        /// Uses reflection to copy fields from one object to the other. This allows greater extendibility
        /// </summary>
        /// <param name="objectToCopyTo"></param>
        /// <param name="objectToCopyFrom"></param>
        public static void copyFields( object objectToCopyTo, object objectToCopyFrom ) {
            Type typeToUse = objectToCopyFrom.GetType();

            FieldInfo[] fields = typeToUse.GetFields( BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public );

            foreach( FieldInfo field in fields ) {
                var fieldToCopy = field.GetValue( objectToCopyFrom );
                field.SetValue( objectToCopyTo, fieldToCopy );
            }
        }

    }
}

// The following is just for safe keeping

/*
  var midFrame = Game1.player.sprite.currentAnimation[ 3 ];

            // Add a delegate to the 4th frame
            if( midFrame.frameBehavior != overrideScytheTool ) {

                midFrame.frameBehavior = overrideScytheTool;
                Game1.player.sprite.currentAnimation[ 3 ] = midFrame;
                return;

            }
            
*/

// Below is a list of things worth keeping as a reference

// Draw the mouse
// Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( ( float ) Game1.getMouseX(), ( float ) Game1.getMouseY() ), new Microsoft.Xna.Framework.Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.mouseCursors, Game1.mouseCursor, 16, 16 ) ), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f );

// Draw Tabs
// Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( this.xPositionOnScreen - panel1X + 60, this.yPositionOnScreen + 20 ), new Rectangle( 1 * 16, 368, 16, 16 ), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f );
