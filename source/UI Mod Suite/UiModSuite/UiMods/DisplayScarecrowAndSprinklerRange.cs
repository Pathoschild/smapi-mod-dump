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
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using UiModSuite.Options;

namespace UiModSuite.UiMods {
    internal class DisplayScarecrowAndSprinklerRange {

        List<Point> effectiveArea = new List<Point>();

        /// <summary>
        /// This mod displays a customizable range for sprinklers and scarecrows
        /// </summary>
        internal void toggleOption() {

            GraphicsEvents.OnPostRenderEvent -= drawTileOutlines;
            GameEvents.FourthUpdateTick -= checkDrawTileOutlines;

            if( ModOptionsPage.getCheckboxValue( ModOptionsPage.Setting.SHOW_SPRINKLER_SCARECROW_RANGE ) ) {
                GraphicsEvents.OnPostRenderEvent += drawTileOutlines;
                GameEvents.FourthUpdateTick += checkDrawTileOutlines;
            }
        }

        /// <summary>
        /// Finds the tiles that need to be drawn to highlight the range
        /// </summary>
        private void checkDrawTileOutlines( object sender, EventArgs e ) {

            effectiveArea.Clear();

            if( Game1.player.CurrentItem == null || Game1.activeClickableMenu != null || Game1.eventUp != false ) {
                return;
            }
            var player = Game1.player;

            string itemName = Game1.player.CurrentItem.Name;
            
            if( itemName.Contains( "arecrow" ) ) {

                int centerX = ( Game1.getMouseX() + Game1.viewport.X ) / Game1.tileSize;
                int centerY = ( Game1.getMouseY() + Game1.viewport.Y ) / Game1.tileSize;

                int width = 17;
                int height = 17;

                for( int w = 0; w < width; w++ ) {
                    for( int h = 0; h < height; h++ ) {

                        // Don't count distances of 12 from center
                        if( Math.Abs( w - 8 ) + Math.Abs( h - 8 ) > 12 ) {
                            continue;
                        }

                        effectiveArea.Add( new Point( tileUnderMouseX() + w - 8, tileUnderMouseY() + h - 8 ) );
                    }
                }

            } else if( itemName.Contains( "Iridium Sprinkler" ) ) {

                var highlightedLocation = ModEntry.modConfig.IridiumSprinkler;
                parseConfigToHighlightedArea( highlightedLocation );
                
            } else if( itemName.Contains( "Quality Sprinkler" ) ) {

                var highlightedLocation = ModEntry.modConfig.QualitySprinkler;
                parseConfigToHighlightedArea( highlightedLocation );

            } else if( itemName.Contains( "Sprinkler" ) ) {

                var highlightedLocation = ModEntry.modConfig.Sprinkler;
                parseConfigToHighlightedArea( highlightedLocation );

            }
        }

        /// <summary>
        /// Parses the highlighted are in the config file for sprinklers
        /// </summary>
        /// <param name="highlightedLocation">The config data</param>
        private void parseConfigToHighlightedArea( int[,] highlightedLocation ) {
            for( int col = 0; col < highlightedLocation.GetLength( 0 ); col++ ) {
                for( int row = 0; row < highlightedLocation.GetLength( 1 ); row++ ) {
                    if( highlightedLocation[ col, row ] == 1 ) {
                        effectiveArea.Add( new Point( tileUnderMouseX() + col - 5, tileUnderMouseY() + row - 5 ) );
                    }
                }
            }
        }

        private int tileUnderMouseX() {
            return ( Game1.getMouseX() + Game1.viewport.X ) / Game1.tileSize;
        }

        private int tileUnderMouseY() {
            return ( Game1.getMouseY() + Game1.viewport.Y ) / Game1.tileSize;
        }

        private void drawTileOutlines( object sender, EventArgs e ) {

            if( effectiveArea.Count < 1 ) {
                return;
            }

            foreach( var item in effectiveArea ) {
                Game1.spriteBatch.Draw( Game1.mouseCursors, Game1.GlobalToLocal( new Vector2( ( float ) ( item.X * Game1.tileSize ), ( float ) ( item.Y * Game1.tileSize ) ) ), new Rectangle?( new Rectangle( 194, 388, 16, 16 ) ), Color.White * 0.7f, 0.0f, Vector2.Zero, ( float ) Game1.pixelZoom, SpriteEffects.None, 0.01f );
            }
        }

    }
}