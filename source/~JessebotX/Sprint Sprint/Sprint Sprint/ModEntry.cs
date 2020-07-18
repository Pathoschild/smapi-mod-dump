using Sprint_Sprint.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Sprint_Sprint
{
    class ModEntry : Mod
    {
        #region Fields & Properties

        /// <summary> Access mod integrations </summary>
        private Integrations ModIntegration;
        /// <summary> Access the mod's configuration settings </summary>
        private ModConfig Config;

        /// <summary> Check if the player is sprinting </summary>
        private bool PlayerSprinting = false;

        #endregion

        #region Methods

        /// <summary> The mod's entry point </summary>
        /// <param name="helper"> Provides simplified APIs </param>
        public override void Entry(IModHelper helper)
        {
            // read config
            this.Config = helper.ReadConfig<ModConfig>();

            // pass stuff to other classes
            this.ModIntegration = new Integrations(this.Helper, this.Monitor, this.ModManifest, this.Config);

            /* Events */
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;
            helper.Events.Input.ButtonPressed += this.ButtonPressed;
        }

        /// <summary> 
        /// Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). 
        /// All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.  
        /// </summary>
        /// <param name="sender"> The object's sender </param>
        /// <param name="e"> The event arguments </param>
        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.ModIntegration.SprintSprintSprintWarning(); // alert to player that you have an outdated version installed
            this.ModIntegration.GenericModConfigMenuApi(); // add a generic mod config menu if the framework exists
        }

        /// <summary> Raised before/after the game state is updated (≈60 times per second) </summary>
        /// <param name="sender"> The object's sender </param>
        /// <param name="e"> The event arguments </param>
        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree || !Context.IsWorldReady || Game1.paused)
                return;

            this.CheckIfHoldDownToSprint();
            this.CheckIfPlayerCanSprint();
            this.Sprint(e);
        }

        /// <summary> Raised after the player pressed a keyboard, mouse, or controller button. This includes mouse clicks </summary>
        /// <param name="sender"> The object sender </param>
        /// <param name="e"> The event arguments </param>
        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            this.CheckIfToggleToSprint(e);
        }

        /// <summary> Make player sprint </summary>
        /// <param name="e"> The event arguments </param>
        private void Sprint(UpdateTickedEventArgs e)
        {
            if (this.CheckIfPlayerCanSprint())
            {
                Game1.player.addedSpeed = this.Config.SprintSpeed;
                this.DepleteStamina(e);
            } else
                Game1.player.addedSpeed = 0;
        }

        private bool CheckIfPlayerCanSprint()
        {
            if (this.PlayerSprinting)
            {
                if (this.Config.NoSprintIfTooTired.Enabled)
                {
                    if (Game1.player.Stamina > this.Config.NoSprintIfTooTired.TiredStamina)
                        return true;
                    else
                        return false;
                } else
                    return true;
            } else
                return false;
        }

        /// <summary> Check if the player is holding down sprint key to sprint </summary>
        private void CheckIfHoldDownToSprint()
        {
            if (this.Config.HoldToSprint)
            {
                if (this.Helper.Input.IsDown(this.Config.SprintKey))
                    this.PlayerSprinting = true;
                else
                    this.PlayerSprinting = false;
            }
        }

        /// <summary> Check if the player has toggle to sprint on</summary>
        /// <param name="e"> The event arguments </param>
        private void CheckIfToggleToSprint(ButtonPressedEventArgs e)
        {
            if (!this.Config.HoldToSprint && e.Button == this.Config.SprintKey)
            {
                if (this.PlayerSprinting)
                    this.PlayerSprinting = false;
                else
                    this.PlayerSprinting = true;
            }
        }

        /// <summary> Deplete player stamina</summary>
        private void DepleteStamina(UpdateTickedEventArgs e)
        {
            if (this.Config.StaminaDrain.Enabled && e.IsMultipleOf(60))
                Game1.player.Stamina -= this.Config.StaminaDrain.StaminaCost;
        }

        #endregion
    }
}
