using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using SFarmer = StardewValley.Farmer;

namespace Demiacle.ImprovedQualityOfLife {

    /*
    //currently 10 exist
    switch( flooring.whichFloor ) {

    case Flooring.wood: //Wood floor
    case Flooring.stone: //Stone floor
    case Flooring.colored_cobblestone: //crystal path      
    case Flooring.iceTile: //Crystal flooring
    case Flooring.boardwalk: //Wood path
    case Flooring.ghost: //Weathered floor
    case Flooring.straw: //straw
    case Flooring.gravel: //gravel                    
    case Flooring.cobblestone: //cobblestone
    case Flooring.steppingStone: //steppingStone
    default:
    */

    internal class SpeedModOnRoads {

        private const int fastSpeed = 1;
        private const int fasterSpeed = 2;
        private const int fastestSpeed = 3;

        Buff speedBuff = new Buff( 22 );

        /// <summary>
        /// This mod adds a speed buff when on built roads. The buff is hacked from the item Savage Ring
        /// </summary>
        public SpeedModOnRoads() {
            GameEvents.FourthUpdateTick += checkPlayerTileForRoad;
            speedBuff.description = "Roads make you speedy!";
            speedBuff.glow = Color.White;
        }

        /// <summary>
        /// Checks the current tile the player and if it is a flooring will increase movement speed
        /// </summary>
        internal void checkPlayerTileForRoad( object sender, EventArgs e ) {

            if( Game1.currentLocation == null ) {
                return;
            }

            SFarmer player = Game1.player;
            Point tileAtLocation = player.getTileLocationPoint();

            if( Game1.currentLocation.terrainFeatures.ContainsKey( new Vector2( tileAtLocation.X, tileAtLocation.Y) ) &&
                Game1.currentLocation.terrainFeatures[ new Vector2( tileAtLocation.X, tileAtLocation.Y ) ] is Flooring  ) {
                
                var flooring = ( Flooring ) Game1.currentLocation.terrainFeatures[ new Vector2( tileAtLocation.X, tileAtLocation.Y ) ];


                if( Game1.buffsDisplay.hasBuff( 22 ) == false ) {
                    
                    Game1.buffsDisplay.addOtherBuff( speedBuff );
                }

                speedBuff.millisecondsDuration = 300;
            }
        }

    }
}

