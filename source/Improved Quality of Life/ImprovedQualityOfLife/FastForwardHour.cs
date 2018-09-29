using System;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using SFarmer = StardewValley.Farmer;

namespace Demiacle.ImprovedQualityOfLife {
    internal class FastForwardHour {

        /// <summary>
        /// This mod fast forwards the game clock by performing 6 update 10 minute ticks when a key is pressed
        /// </summary>
        public FastForwardHour() {
            ControlEvents.KeyPressed += displayFastForwardDialogureOnPressX;
        }

        /// <summary>
        /// Opens a question dialogue when the appropriate key is pressed
        /// </summary>
        private void displayFastForwardDialogureOnPressX( object sender, EventArgsKeyPressed e ) {

            if( e.KeyPressed.ToString() == ModEntry.modConfig.waitOneHourKey && Game1.activeClickableMenu == null && Game1.eventUp == false ) {

                var responses = new List<Response>();

                responses.Add( new Response( "yes", "yes" ) );
                responses.Add( new Response( "no", "no" ) );
                
                Game1.currentLocation.createQuestionDialogue( "Wait an hour?", responses.ToArray(), fastForwardTime );
                Game1.currentLocation.lastQuestionKey = "";
            }
        }

        /// <summary>
        /// The delegate to fire after the question dialogue is answered
        /// </summary>
        /// <param name="who">Unused</param>
        /// <param name="whichAnswer">The answer the player has chosen</param>
        private void fastForwardTime( SFarmer who, string whichAnswer ) {
            if( whichAnswer == "yes" ) {
                Game1.globalFadeToBlack( fadeBackIn );
            }
        }

        /// <summary>
        /// Fires right after fadeToBlack
        /// </summary>
        private void fadeBackIn() {

            for( int i = 0; i < 6; i++ ) {
                Game1.performTenMinuteClockUpdate();
            }

            Game1.globalFadeToClear();
        }

    }
}