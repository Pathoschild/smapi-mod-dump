using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
using UiModSuite.Options;

namespace UiModSuite.UiMods {
    class AccurateHearts {
        
        private List<ClickableTextureComponent> friendNames;
        private SocialPage socialPage;

        /// <summary>
        /// Displays a partial heart fill between heart levels on the social page
        /// </summary>
        public void toggleVisibleHearts() {

            GraphicsEvents.OnPostRenderGuiEvent -= drawHeartFills;
            MenuEvents.MenuChanged -= OnMenuChange;

            if( ModOptionsPage.getCheckboxValue( ModOptionsPage.Setting.SHOW_HEART_FILLS ) ) {
                MenuEvents.MenuChanged += OnMenuChange;
                GraphicsEvents.OnPostRenderGuiEvent += drawHeartFills;
            }
        }

        /// <summary>
        /// Stores the useful data from the social page when the GameMenu is brought up
        /// </summary>
        internal void OnMenuChange( object sender, EventArgsClickableMenuChanged e ) {
            StardewValley.Farmer x = Game1.player;
            if( !( Game1.activeClickableMenu is GameMenu ) ) {
                return;
            }
            
            var pages = ( List<IClickableMenu> ) typeof( GameMenu ).GetField( "pages", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( Game1.activeClickableMenu );
            
            // Get the list of friends in their correct order
            for( int k = 0; k < pages.Count; k++ ) {
                if( pages[k] is SocialPage ) {
                    socialPage = (SocialPage) pages[ k ];
                    friendNames = ( List<ClickableTextureComponent> ) typeof( SocialPage ).GetField( "friendNames", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( pages[ k ] );
                }
            }
        }

        public void drawHeartFills( object sender, EventArgs e ) {
            if(  Game1.activeClickableMenu is GameMenu == false ) {
                return;
            }

            var gameMenu = ( GameMenu ) Game1.activeClickableMenu;

            if( gameMenu.currentTab != GameMenu.socialTab ) {
                return;
            }

            var slotPosition = ( int ) typeof( SocialPage ).GetField( "slotPosition", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( socialPage );

            int offsetYforEachSlot = 0;
            for( int i = slotPosition; i < slotPosition + 5; i++ ) {

                // Safety check... this exists inside the client so lets reproduce here just in case
                if( i > friendNames.Count ) {
                    return;
                }

                int distanceAwayFromFirstHeartPositionY = 130;

                int yPosition = Game1.activeClickableMenu.yPositionOnScreen + distanceAwayFromFirstHeartPositionY + offsetYforEachSlot;

                offsetYforEachSlot += 112;      
                
                // Fill Hearts
                int friendshipPointsForCurrentLevel = 0;
                int friendshipLevel = 0;

                if( Game1.player.friendships.ContainsKey( friendNames[ i ].name ) && Game1.player.friendships[ friendNames[ i ].name ][0] > 0  ) {
                    friendshipPointsForCurrentLevel = Game1.player.friendships[ friendNames[ i ].name ][ 0 ] % 250;
                    friendshipLevel = Game1.player.friendships[ friendNames[ i ].name ][ 0 ] / 250;
                } else {
                    continue; 
                }
                int totalFriendshipPoints = Game1.player.friendships[ friendNames[ i ].name ][ 0 ];

                // If hearts are maxed, draw nothing
                if( totalFriendshipPoints > 3000 ) {
                    return;
                } else if( friendNames[ i ].name != Game1.player.spouse && totalFriendshipPoints > 2500 ) {
                    return;
                }
                drawEachIndividualSquare( friendshipLevel, friendshipPointsForCurrentLevel,  yPosition );

                // ReDraw the mouse
                if( !Game1.options.hardwareCursor ) {
                    Game1.spriteBatch.Draw( Game1.mouseCursors, new Vector2( ( float ) Game1.getMouseX(), ( float ) Game1.getMouseY() ), new Microsoft.Xna.Framework.Rectangle?( Game1.getSourceRectForStandardTileSheet( Game1.mouseCursors, Game1.mouseCursor, 16, 16 ) ), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f );
                }
            }

            // ReDraw the tooltip
            var defaultHoverText = ( string ) typeof( GameMenu ).GetField( "hoverText", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( gameMenu );

            IClickableMenu.drawHoverText( Game1.spriteBatch, defaultHoverText, Game1.smallFont, 0, 0, -1, ( string ) null, -1, ( string[] ) null, ( Item ) null, 0, -1, -1, -1, -1, 1f, ( CraftingRecipe ) null );
        }

        /// <summary>
        /// Draws a fill for hearts based on friendship level.
        /// </summary>
        /// <param name="friendshipLevel">The current heart level. This will shift the heart fill to the right depending on level</param>
        /// <param name="friendshipPoints">This is the amount of friendship points for this level</param>
        /// <param name="bounds">The checkbox bounds. This is just an easier way to draw y axis</param>
        private void drawEachIndividualSquare( int friendshipLevel, int friendshipPoints, int yPosition ) {

            // 12 amount of amountOfPixelsToFill is the max
            // Ratios are a tad off so the last point is only half as much... its fine
            int amountOfPixelsToFill = friendshipPoints / 20;
            int heartLevelOffsetX;

            if( friendshipLevel > 10 ) {
                heartLevelOffsetX = 32 * ( friendshipLevel - 10 );
                yPosition += ( 7 * 4 );
            } else {
               heartLevelOffsetX = 32 * friendshipLevel;
            }

            int[,] heartFillArray = {
                 { 1, 1, 0, 1, 1, },
                 { 1, 1, 1, 1, 1, },
                 { 0, 1, 1, 1, 0, },
                 { 0, 0, 1, 0, 0, }
            };

            int distanceAwayFromFirstHeartPositionX = 316;

            // Draw the squares from bottom to top and left to right
            for( int row = 3; row >= 0; row-- ) {
                for( int column = 0; column < 5; column++ ) {
                    if( amountOfPixelsToFill < 1 ) {
                        return;
                    }
                    
                    

                    if( heartFillArray[ row, column ] == 1 ) {
                        Game1.spriteBatch.Draw( Game1.staminaRect, new Rectangle( Game1.activeClickableMenu.xPositionOnScreen + distanceAwayFromFirstHeartPositionX + heartLevelOffsetX + ( column * 4 ), yPosition + 14 + ( row * 4 ), 4, 4 ), Color.Crimson );
                        amountOfPixelsToFill--;
                    }
                }
            }
        }

    }
}
