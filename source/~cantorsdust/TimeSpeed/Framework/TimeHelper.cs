using System;
using StardewModdingAPI.Events;
using StardewValley;

namespace TimeSpeed.Framework
{
    /// <summary>Provides helper methods for tracking time flow.</summary>
    internal class TimeHelper
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The game's default tick interval in milliseconds for the current location.</summary>
        public int CurrentDefaultTickInterval => 7000 + (Game1.currentLocation?.getExtraMillisecondsPerInGameMinuteForThisLocation() ?? 0);

        /// <summary>The percentage of the <see cref="CurrentDefaultTickInterval"/> that's elapsed since the last tick.</summary>
        public double TickProgress
        {
            get => (double)Game1.gameTimeInterval / this.CurrentDefaultTickInterval;
            set => Game1.gameTimeInterval = (int)(value * this.CurrentDefaultTickInterval);
        }


        /*********
        ** Public methods
        *********/
        /// <summary>Register an event handler to notify when the <see cref="TickProgress"/> changes.</summary>
        /// <param name="handler">The event handler to notify.</param>
        /// <returns>Returns an action which unregisters the handler.</returns>
        public Action WhenTickProgressChanged(Action<TickProgressChangedEventArgs> handler)
        {
            double previousProgress = 0;

            void Wrapper(object sender, EventArgs args)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator - intended
                if (previousProgress != this.TickProgress)
                    handler(new TickProgressChangedEventArgs(previousProgress, this.TickProgress));
                previousProgress = this.TickProgress;
            }

            GameEvents.UpdateTick += Wrapper;
            return () => GameEvents.UpdateTick -= Wrapper;
        }
    }
}
