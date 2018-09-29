using System;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Characters;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using StardewValley.Monsters;

namespace Demiacle.ImprovedQualityOfLife {
    internal class SummonHorseAnywhere {

        private Vector2 positionToRunTo;
        private Horse runningHorse;
        private float currentRunSpeed;

        private float initialRunSpeed = 30;
        private float speedToSlowPerRender = 0.9f;

        private ShadowShaman poofAnimator = new ShadowShaman( Vector2.Zero );

        public SummonHorseAnywhere() {
            ControlEvents.KeyPressed += callHorseOnButtonPress;
            //GraphicsEvents.OnPostRenderEvent += animateHorse;
        }

        /// <summary>
        /// Animates a horse running from the left of the screen to the designated position
        /// </summary>
        [Obsolete( "Never Fires" )]
        private void animateHorse( object sender, EventArgs e ) {

            // Only fires if callHorseOnZPress found a valid horse
            if( runningHorse != null ) {

                // Stop if player jumped on horse before animation
                if( runningHorse.rider != null ) {
                    runningHorse = null;
                    return;
                }

                // Stop if horse reached destination boundingbox is more accurate
                if( runningHorse.GetBoundingBox().X >= positionToRunTo.X ) {

                    // If horse position is at a fraction the game draws glitches
                    runningHorse.position.X = ( float ) Math.Floor( runningHorse.position.X );

                    // Mini animation for flavor!
                    runningHorse.sprite.setCurrentAnimation( new List<FarmerSprite.AnimationFrame>() {
                        new FarmerSprite.AnimationFrame(21, 600),
                        new FarmerSprite.AnimationFrame(22, 700),
                        new FarmerSprite.AnimationFrame(21, 5600)
                    } );

                    var test1 = Game1.player.GetBoundingBox();
                    var horsetest = runningHorse.GetBoundingBox();

                    var horseBound = runningHorse.position.X;

                    runningHorse = null;
                    return;

                }

                // Move horse
                runningHorse.position.X += Math.Max( 1, currentRunSpeed );
                runningHorse.SetMovingOnlyRight();

                // Slow horse down if closer to destination
                if( runningHorse.position.X + 600 > positionToRunTo.X && currentRunSpeed > 3 ) {
                    currentRunSpeed -= speedToSlowPerRender;
                }

                // Animates the horse
                if( runningHorse.sprite.currentAnimation == null ) {

                    runningHorse.sprite.setCurrentAnimation( new List<FarmerSprite.AnimationFrame>() {

                    // These animate left
                    // new FarmerSprite.AnimationFrame(8, 70, false, true, null, false),
                    // new FarmerSprite.AnimationFrame(9, 70, false, true, new AnimatedSprite.endOfAnimationBehavior(FarmerSprite.checkForFootstep), false),
                    // new FarmerSprite.AnimationFrame(10, 70, false, true, new AnimatedSprite.endOfAnimationBehavior(FarmerSprite.checkForFootstep), false),
                    // new FarmerSprite.AnimationFrame(11, 70, false, true, new AnimatedSprite.endOfAnimationBehavior(FarmerSprite.checkForFootstep), false),
                    // new FarmerSprite.AnimationFrame(12, 70, false, true, null, false),
                    // new FarmerSprite.AnimationFrame(13, 70, false, true, null, false),

                    // These animate right
                    new FarmerSprite.AnimationFrame(8, 70),
                    new FarmerSprite.AnimationFrame(9, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(FarmerSprite.checkForFootstep), false),
                    new FarmerSprite.AnimationFrame(10, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(FarmerSprite.checkForFootstep), false),
                    new FarmerSprite.AnimationFrame(11, 70, false, false, new AnimatedSprite.endOfAnimationBehavior(FarmerSprite.checkForFootstep), false),
                    new FarmerSprite.AnimationFrame(12, 70),
                    new FarmerSprite.AnimationFrame(13, 70)
                    } );
                }
            }
        }

        /// <summary>
        /// Removes your horse from its current location and places it next to the player
        /// </summary>
        private void callHorseOnButtonPress( object sender, EventArgsKeyPressed e ) {

            // Do nothing 
            // If riding horse
            // If player is indoors
            // If horse is already being called
            // If event up
            // If menu up
            // If keypress is not configured key ( defaults to z )
            if( Game1.player.isRidingHorse() || Game1.currentLocation.isOutdoors == false || runningHorse != null || Game1.eventUp || Game1.activeClickableMenu != null || e.KeyPressed.ToString() != ModEntry.modConfig.summonHorseKey ) {
                return;
            }

            Horse tempHorse = null;
            GameLocation horseLocation = null;

            // Find horse
            foreach( var location in Game1.locations ) {
                foreach( var npc in location.characters ) {

                    // Only call horse if horse is in a different location
                    if( location == Game1.currentLocation ) {
                        //continue;
                    }

                    if( npc is Horse ) {
                        if( tempHorse == null ) {
                            tempHorse = ( Horse ) npc;
                            horseLocation = location;
                        } else {
                            ModEntry.Log( $"There are multiple horses! Calling {tempHorse.name} from {tempHorse.currentLocation.name}" );
                        }
                    }
                }
            }
            
            // No horse found
            if( tempHorse == null ) {
                return;
            }

            // Do nothing if horse is close to farmer
            if( Utility.distance( tempHorse.position.X, Game1.player.position.X, tempHorse.position.Y, Game1.player.position.Y ) < 600 ) {
                ModEntry.Log( "Your horse is right next to you... don't be lazy, go walk up to it!" );
                return;
            }

            // Change horse location
            if( Game1.currentLocation.characters.Contains( tempHorse ) == false && horseLocation.characters.Remove( tempHorse ) ) {
                Game1.currentLocation.characters.Add( tempHorse );
                tempHorse.currentLocation = Game1.currentLocation;
            }

            runningHorse = tempHorse;

            // Set horse start position
            int positionXToRunTo = Game1.player.getTileX() * Game1.tileSize;
            int positionYToRunTo = Game1.player.getTileY() * Game1.tileSize;

            positionToRunTo = new Vector2( positionXToRunTo, positionYToRunTo );

            //Game1.currentLocation.tile

            /*
            int differenceBetweenPlayerAndHorse = ( Game1.player.sprite.spriteWidth - runningHorse.sprite.spriteWidth ) / 2 * Game1.pixelZoom + 6;
            positionToRunTo = Game1.player.position;
            positionToRunTo.X += differenceBetweenPlayerAndHorse;
            runningHorse.position.Y = positionToRunTo.Y;
            runningHorse.position.X = Game1.viewport.X - runningHorse.sprite.getWidth() * Game1.pixelZoom;
            */

            float boundingBoxOffset = runningHorse.position.X - runningHorse.GetBoundingBox().X;

            runningHorse.position.X = positionToRunTo.X + boundingBoxOffset;
            //runningHorse.position.X = Game1.viewport.X - runningHorse.sprite.getWidth() * Game1.pixelZoom;
            runningHorse.position.Y = positionToRunTo.Y;

            // Set horse moving right
            runningHorse.facingDirection = 1;

            // Set starting variables
            currentRunSpeed = 30;
            speedToSlowPerRender = 1;
            runningHorse.sprite.currentAnimation = null;
            poofAnimator.position = runningHorse.position;
            poofAnimator.deathAnimation();
            runningHorse = null;
        }

    }
}