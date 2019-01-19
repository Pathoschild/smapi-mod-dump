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

        private int addedSpeed = 0;
        private int secondsUntilSpeedIncrement = 4;
        
        /*-Buffs-*/
        private Buff sprintBuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 1, "Sprint", "Sprint");
        private Buff sprintBuff2 = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 1, "Sprint", "Sprint");
        //-------//

        private bool playerSprinting = false;

        public override void Entry(IModHelper helper)
        {
            /* Event Handlers */
            this.Helper.Events.Input.ButtonPressed += this.ButtonPressed;
            this.Helper.Events.GameLoop.OneSecondUpdateTicked += this.OneSecond;
            this.Helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;

            /* Read Config */
            this.Config = helper.ReadConfig<ModConfig>();
        }

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
            {
                return;
            }

            this.Helper.Input.Suppress(this.Config.SprintKey);
            this.Helper.Input.Suppress(this.Config.ControllerSprintButton);

            bool sprintKeyPressed = this.Helper.Input.IsDown(this.Config.SprintKey | this.Config.ControllerSprintButton);

            if (sprintKeyPressed && Game1.player.isMoving())
            {
                playerSprinting = true;
                if (secondsUntilSpeedIncrement <= 4 && secondsUntilSpeedIncrement > 2)
                {
                    Game1.buffsDisplay.addOtherBuff(sprintBuff);
                }
                else if (secondsUntilSpeedIncrement <= 2)
                {
                    Game1.buffsDisplay.addOtherBuff(sprintBuff2);
                }
            }
        }

        private void OneSecond(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (playerSprinting)
            {
                if (secondsUntilSpeedIncrement > 0)
                {
                    secondsUntilSpeedIncrement--;
                }
            }
        }

        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (playerSprinting)
            {
                if (secondsUntilSpeedIncrement <= 4 && secondsUntilSpeedIncrement > 2)
                {
                    sprintBuff.millisecondsDuration = 5000;
                }
                else if (secondsUntilSpeedIncrement <= 2)
                {
                    sprintBuff.millisecondsDuration = 5000;
                }
            }
        }
    }
}
