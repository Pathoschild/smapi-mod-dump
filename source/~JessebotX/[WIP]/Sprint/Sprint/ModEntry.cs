using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewModdingAPI.Framework;
using StardewModdingAPI.Utilities;

namespace Sprint
{
    class ModEntry : Mod
    {
        /************
         ***Fields***
         ************/
        private ModConfig Config;
        /* sprint activated bool */
        private bool playerSprinting = false;
        /* Realistic Sprint Speed Timer*/
        private int secondsUntilIncreaseSpeed = 0;
        //speed
        private int sprintSpeed;

        public override void Entry(IModHelper helper)
        {
            /* Event Handlers */
            this.Helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            this.Helper.Events.GameLoop.OneSecondUpdateTicked += this.OneSecond;

            /* Read Config */
            this.Config = helper.ReadConfig<ModConfig>();
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            //if player isn't free to act in the world, do nothing
            if (!Context.IsPlayerFree)
            {
                return;
            }

            else
            {
                //suppress game keybinds depending on config values
                this.Helper.Input.Suppress(this.Config.PrimarySprintKey);
                this.Helper.Input.Suppress(this.Config.SecondarySprintKey);
                this.Helper.Input.Suppress(this.Config.ControllerSprintButton);
                this.Helper.Input.Suppress(this.Config.SlowDownKey);

                // is the key/button being pressed
                bool isPrimarySprintKeyPressed = this.Helper.Input.IsDown(this.Config.PrimarySprintKey);
                bool isSecondarySprintKeyPressed = this.Helper.Input.IsDown(this.Config.SecondarySprintKey);
                bool isControllerSprintButtonPressed = this.Helper.Input.IsDown(this.Config.ControllerSprintButton);

                if (isPrimarySprintKeyPressed || isSecondarySprintKeyPressed || isControllerSprintButtonPressed && Game1.player.isMoving())
                {
                    playerSprinting = true;
                    secondsUntilIncreaseSpeed = 5;
                    Game1.player.addedSpeed += this.sprintSpeed;
                }
            }
        }

        private void OneSecond(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree)
            {
                return;
            }

            if (playerSprinting == true)
            {
                if (secondsUntilIncreaseSpeed > 0)
                {
                    secondsUntilIncreaseSpeed--;
                }

                if (secondsUntilIncreaseSpeed <= 5)
                {
                    sprintSpeed = 1;
                }
                else if (secondsUntilIncreaseSpeed <= 3)
                {
                    sprintSpeed = 2;
                }
                else if (secondsUntilIncreaseSpeed <= 0)
                {
                    sprintSpeed = 3;
                }
            }
        }
    }
}
