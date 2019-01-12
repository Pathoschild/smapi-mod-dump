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
        private ModConfig Config;

        public override void Entry (IModHelper helper)
        {
            /* Event Handler */
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;

            /* Read Config */
            this.Config = helper.ReadConfig<ModConfig>();
        }

        private void OnButtonPressed (object sender, ButtonPressedEventArgs e)
        {
            // if player isn't free to act in the world, do nothing
            if (!Context.IsPlayerFree)
            {
                return;
            }

            else
            {
                // suppress game keybinds depending on config values
                this.Helper.Input.Suppress(this.Config.PrimarySprintKey);
                this.Helper.Input.Suppress(this.Config.SecondarySprintKey);
                this.Helper.Input.Suppress(this.Config.ControllerSprintButton);
                this.Helper.Input.Suppress(this.Config.SlowDownKey);

                // is the key/button being pressed
                bool isPrimarySprintKeyPressed = this.Helper.Input.IsDown(this.Config.PrimarySprintKey);
                bool isSecondarySprintKeyPressed = this.Helper.Input.IsDown(this.Config.SecondarySprintKey);
                bool isControllerSprintButtonPressed = this.Helper.Input.IsDown(this.Config.ControllerSprintButton);

                if (isPrimarySprintKeyPressed || isSecondarySprintKeyPressed || isControllerSprintButtonPressed)
                {
                    Game1.player.addedSpeed = 3;
                }
            }
        }
    }
}
