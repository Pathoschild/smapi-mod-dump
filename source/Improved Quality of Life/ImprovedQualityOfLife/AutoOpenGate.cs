using System;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Demiacle.ImprovedQualityOfLife {
    internal class AutoOpenGate {

        private Fence gateToClose;
        private int gateX;
        private int gateY;

        // TODO to lock gates open add listener to right click on gates and when right clicked will remain open and show lock icon and added to a list  of ignored gates. Right click again to remove from list but also remain open. Must also be in adjacent tile to lock

        /// <summary>
        /// This mod automatically opens the gate when you get near it and closes it when you move away from it
        /// </summary>
        public AutoOpenGate() {
            GameEvents.UpdateTick += checkForGate;
        }

        /// <summary>
        /// Checks the current 4 squares near the player and if one square is holding a gate it will open it, and close it once you move far enough away
        /// </summary>
        private void checkForGate( object sender, EventArgs e ) {

            if( Game1.currentLocation is Farm == false ) {
                return;
            }

            if( gateToClose != null ) {
                
                int deltaX = Math.Abs( Game1.player.getTileX() - gateX );
                int deltaY = Math.Abs( Game1.player.getTileY() - gateY );

                if( deltaX > 1 || deltaY > 1 ) {

                    if( gateToClose.gatePosition != 0 ) {
                        Game1.playSound( "doorClose" );
                        gateToClose.gatePosition = 0;

                    }

                    gateToClose = null;

                }

            } else {

                foreach( var groundObject in Game1.currentLocation.objects ) {

                    if( groundObject.Value is Fence && ( groundObject.Value as Fence ).isGate ) {

                        List<Vector2> adjacentTiles = Utility.getAdjacentTileLocations(Game1.player.getTileLocation());

                        foreach( var item in adjacentTiles ) {

                            if( groundObject.Value.tileLocation.Equals( new Vector2( item.X, item.Y ) ) ) {

                                int gatePositionValue = ( groundObject.Value as Fence ).gatePosition;

                                // If gate is closed
                                if( gatePositionValue == 0 ) {
                                    ( groundObject.Value as Fence ).gatePosition = 88;
                                    gateToClose = ( Fence ) groundObject.Value;
                                    Game1.playSound( "doorClose" );
                                    gateX = ( int ) item.X;
                                    gateY = ( int ) item.Y;
                                }

                            }

                        }

                    }

                }

            }

        }

    }
}