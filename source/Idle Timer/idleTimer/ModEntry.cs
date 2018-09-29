using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace IdleTimer
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private bool isIdle;
        private SButton lastPressed;
        private int lastPressedTime;
        int idleTimer;
        public 
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        override void Entry(IModHelper helper)
        {
            idleTimer = 300;
            InputEvents.ButtonPressed += this.InputEvents_ButtonPressed;
            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
            TimeEvents.TimeOfDayChanged += this.TimeOfDayChanged;
        }

        private void TimeOfDayChanged(object sender, EventArgsIntChanged e)
        {

            if (isIdle)
            {
                Game1.pauseThenMessage(200, "You are now idle", false);
                this.Monitor.Log($"{Game1.player.name} idle at {Game1.timeOfDay}");
            }
            if (Game1.timeOfDay > lastPressedTime + idleTimer)
                isIdle = true;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player presses a controller, keyboard, or mouse button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            isIdle = false;
            lastPressedTime = Game1.timeOfDay;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked after the game updates (roughly 60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        /// <remarks>
        /// All times are shown in the game's internal format, which is essentially military time with support for
        /// times past midnight (e.g. 2400 is midnight, 2600 is 2am).
        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

        }
    }
}