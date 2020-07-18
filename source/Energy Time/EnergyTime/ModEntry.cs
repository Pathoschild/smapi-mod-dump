using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace EnergyTime
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {

        /********* Properties *********/

        private ModConfig Config;
        private bool IsPassingTime = false;
        private int TargetIntervals = 150;
        private float UpdateStaminaDelta;
        private float LastStamina;
        private float StaminaUsed;
        

        /*********
        ** Public methods
        *********/
        public override void Entry(IModHelper helper)
        {
            // read config
            this.Config = helper.ReadConfig<ModConfig>();

            // set initial values
            this.StaminaUsed = 0;

            // initialize helpers
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Input.ButtonReleased += this.OnButtonReleased;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }


        /*********
        ** Private methods
        *********/

        private void OnSaveLoaded(object sender, EventArgs e)
        {
            this.LastStamina = Game1.player.Stamina;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            //this.Monitor.Log($"{this.UpdateStaminaDelta} {Game1.player.MaxStamina} {this.TotalIntervals}", LogLevel.Debug);

            if (e.Button == this.Config.PassTimeKey)
                this.IsPassingTime = true;
        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (e.Button == this.Config.PassTimeKey)
                this.IsPassingTime = false;
        }

        private int currentTickInterval()
        {
            return 7000 + (Game1.currentLocation?.getExtraMillisecondsPerInGameMinuteForThisLocation() ?? 0);
        }


        private void OnUpdateTicked(object sender, EventArgs e)
        {
            // skip if save not loaded yet
            if (!Context.IsWorldReady)
                return;

            int tickInterval = this.currentTickInterval();
            if (this.IsPassingTime)
            {
                int ffTimeInterval = Convert.ToInt32(tickInterval * 0.1);
                Game1.gameTimeInterval += ffTimeInterval;
                return;
            }
            else
                Game1.gameTimeInterval = 0;

            float currentStamina = Game1.player.Stamina;
            if (currentStamina == this.LastStamina)
                return;

            float usedStamina = this.StaminaUsed + this.LastStamina - currentStamina;
            //this.Monitor.Log($"{this.LastStamina} {currentStamina} | Used: {usedStamina}", LogLevel.Debug);

            this.LastStamina = currentStamina;

            float requirement = this.UpdateStaminaDelta * this.Config.EnergyRequirementMultiplier;
            if (usedStamina > requirement)
            {
                //int multiplier = (int) Math.Floor(usedStamina / requirement);
                Game1.gameTimeInterval = tickInterval;
                this.StaminaUsed = 0;
            }
            else
                this.StaminaUsed = usedStamina;
        } 

        private float determineUpdateDelta()
        {
            return Game1.player.MaxStamina / this.TargetIntervals;
        }

        private void OnDayStarted(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            this.UpdateStaminaDelta = determineUpdateDelta();
            this.StaminaUsed = 0;
        }
    }
}