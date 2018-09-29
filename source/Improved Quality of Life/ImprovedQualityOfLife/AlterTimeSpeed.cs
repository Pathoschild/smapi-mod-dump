using System;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;

namespace Demiacle.ImprovedQualityOfLife {
    internal class AlterTimeSpeed {

        private int amountOfTimeToAlterPerTenMinutes = 0;
        private int timePassedPerTenMinuteUpdate;
        private int timeOfDayToAlter;
        private List<int> optionTable = new List<int>();

        //System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        /// <summary>
        /// Alters the time of a 10 minute increment
        /// </summary>
        public AlterTimeSpeed() {

            LocationEvents.CurrentLocationChanged += adjustIndoorOutdoorTimer;
            MenuEvents.MenuClosed += onTimeMenuRemove;
            GameEvents.UpdateTick += addOrRemoveTime;
            //timer.Start();

            //TODO write test to check vs option
            optionTable.Add( -6000 );
            optionTable.Add( -5000 );
            optionTable.Add( -4000 );
            optionTable.Add( -3000 );
            optionTable.Add( -2000 );
            optionTable.Add( -1000 );
            optionTable.Add( 0000 );
            optionTable.Add( 1000 );
            optionTable.Add( 2000 );
            optionTable.Add( 3000 );
            optionTable.Add( 4000 );
            optionTable.Add( 5000 );
            optionTable.Add( 6000 );
            optionTable.Add( 7000 );
            optionTable.Add( 8000 );
            optionTable.Add( 9000 );
            optionTable.Add( 10000 );
            optionTable.Add( 11000 );
            optionTable.Add( 12000 );
            optionTable.Add( 13000 );
            optionTable.Add( 14000 );
            optionTable.Add( 15000 );
            optionTable.Add( 16000 );
            optionTable.Add( 17000 );
            optionTable.Add( 18000 );
            optionTable.Add( 19000 );
            optionTable.Add( 20000 );
            optionTable.Add( 30000 );
            optionTable.Add( 40000 );
            optionTable.Add( 50000 );
            optionTable.Add( 60000 );
        }

        /// <summary>
        /// Reset the timer if menu is closed and last menu this mods option menu
        /// </summary>
        private void onTimeMenuRemove( object sender, EventArgsClickableMenuClosed e ) {
            if( e.PriorMenu is QualityOfLifeModOptions ) {
                resetTimer( false );
            }
        }

        /// <summary>
        /// Handles a change timespeed when moving between locations
        /// </summary>
        private void adjustIndoorOutdoorTimer( object sender, EventArgsCurrentLocationChanged e ) {

            int option = e.NewLocation.isOutdoors ? ModEntry.modData.intOptions[ QualityOfLifeModOptions.TIME_PER_TEN_MINUTE_OPTION ] : ModEntry.modData.intOptions[ QualityOfLifeModOptions.TIME_PER_TEN_MINUTE_INSIDE_OPTION ];
            amountOfTimeToAlterPerTenMinutes = optionTable[ option ];

            int timePerTenMinute = optionTable[ option ];
            int totalTimeNeededForUpdatedTick = timePerTenMinute + 7000;
            int timeStillNeededToCompleteUpdatedTick = totalTimeNeededForUpdatedTick - timePassedPerTenMinuteUpdate;

            // Time will be added no matter what so only reduce to 0
            Game1.gameTimeInterval = Math.Max( 7000 - timeStillNeededToCompleteUpdatedTick, 0 );
        }

        /// <summary>
        /// Adds a total number of milliseconds to the Game1.gameTimeInterval which when 7000 is hit a 10 minute time increment is done
        /// </summary>
        private void addOrRemoveTime( object sender, EventArgs e ) {

            // Don't count time if any option menu is open
            if( Game1.activeClickableMenu != null ) {
                return;
            }

            // Reset counter every 10 minutes
            if( timeOfDayToAlter != Game1.timeOfDay ) {
                resetTimer( true );
            }

            // If slowed enough time do nothing
            if( timePassedPerTenMinuteUpdate > amountOfTimeToAlterPerTenMinutes ) {
                return;
            }

            // Slow down time passed
            int timePassedThisTick = Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            timePassedPerTenMinuteUpdate += Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            Game1.gameTimeInterval -= timePassedThisTick;
        }

        /// <summary>
        /// Resets vars and reduces the time needed for a 10 minute tick if necessary.
        /// </summary>
        private void resetTimer( bool logTimeTaken ) {

            // Force reset
            Game1.gameTimeInterval = 0;

            int option = Game1.currentLocation.isOutdoors ? ModEntry.modData.intOptions[ QualityOfLifeModOptions.TIME_PER_TEN_MINUTE_OPTION ] : ModEntry.modData.intOptions[ QualityOfLifeModOptions.TIME_PER_TEN_MINUTE_INSIDE_OPTION ];
            amountOfTimeToAlterPerTenMinutes = optionTable[ option ];

            //if( logTimeTaken ) {
                //ModEntry.Log( $"10 minute length took {timer.ElapsedMilliseconds}" );
                //timer.Reset();
                //timer.Start();
            //}
            
            timeOfDayToAlter = Game1.timeOfDay;

            timePassedPerTenMinuteUpdate = 0;

            // Fast forward time if we are speeding up the clock
            if( amountOfTimeToAlterPerTenMinutes < 0 ) {
                Game1.gameTimeInterval -= amountOfTimeToAlterPerTenMinutes;
            }
        }

    }
}