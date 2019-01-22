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
        //reference ModConfig class
        private ModConfig Config;

        private bool playerSprinting = false;

        //reference buff
        private Buff sprintingBuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 1, "Sprinting", "Sprinting");

        public override void Entry(IModHelper helper)
        {
            /* Event Handlers */
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;
            helper.Events.GameLoop.OneSecondUpdateTicked += this.OneSecond;

            /* Read Config */
            this.Config = helper.ReadConfig<ModConfig>();
        }

        /* Check if sprinting buff exists */
        private bool SprintBuffExists()
        {
            if (sprintingBuff == null)
            {
                return false;
            }

            return Game1.buffsDisplay.otherBuffs.Contains(sprintingBuff);
        }

        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            this.Helper.Input.Suppress(this.Config.SprintKey); //suppresses game keybind so that you can use the sprint key
            bool isSprintKey = this.Helper.Input.IsDown(this.Config.SprintKey); //check if sprint key is pressed

            sprintingBuff.millisecondsDuration = 5000;
            if (!Context.IsPlayerFree)
            {
                return;
            }

            else
            {
                /* only create a `sprintingBuff` if it does not already exist */ 
                if (isSprintKey && !SprintBuffExists())
                {
                    playerSprinting = true;

                    Game1.buffsDisplay.addOtherBuff(sprintingBuff);
                }

                else
                {
                    Game1.buffsDisplay.otherBuffs.Remove(sprintingBuff);
                    sprintingBuff.removeBuff();
                    Game1.buffsDisplay.syncIcons();
                }
            }
        }

        private void OneSecond(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (playerSprinting && !Game1.paused && Game1.player.isMoving())
            {
                Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina - 0.5f);
            }
        }
    }
}
