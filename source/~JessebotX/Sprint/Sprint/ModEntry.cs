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
        private Buff SprintingBuff;
        private Buff SprintingBuff2;

        public override void Entry(IModHelper helper)
        {
            /* Read Config */
            this.Config = helper.ReadConfig<ModConfig>();

            SprintingBuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, Config.Sprint.SprintSpeed, 0, 0, 1, "Sprint Sprint Sprint", "Sprint Sprint Sprint");
            SprintingBuff2 = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, Config.Sprint.SprintSpeed + Config.Sprint.LeftShiftKeybindExtraSpeed, 0, 0, 1, "Sprint Sprint Sprint", "Sprint Sprint Sprint");

            /* Event Handlers */
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;
            helper.Events.GameLoop.OneSecondUpdateTicked += this.OneSecond;
        }

        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            bool isSprintKey = this.Helper.Input.IsDown(this.Config.SprintKey); //check if sprint key is pressed
            if (this.Config.SprintKey == SButton.LeftShoulder || this.Config.SprintKey == SButton.RightShoulder)
            {
                this.Helper.Input.Suppress(this.Config.SprintKey);
            }

            SprintingBuff.millisecondsDuration = 5000;
            SprintingBuff2.millisecondsDuration = 5000;
            if (!Context.IsPlayerFree)
            {
                return;
            }

            else
            {
                /* only create a `sprintingBuff` if it does not already exist */ 
                if (isSprintKey && !SprintBuffExists() && this.Config.SprintKey != SButton.LeftShift)
                {
                    playerSprinting = true;

                    Game1.buffsDisplay.addOtherBuff(SprintingBuff);
                }

                else if (isSprintKey && !SprintBuff2Exists() && this.Config.SprintKey == SButton.LeftShift)
                {
                    //left shift only
                    playerSprinting = true;

                    Game1.buffsDisplay.addOtherBuff(SprintingBuff2);
                }

                else
                {
                    playerSprinting = false;

                    //remove buffs
                    Game1.buffsDisplay.otherBuffs.Remove(SprintingBuff);
                    Game1.buffsDisplay.otherBuffs.Remove(SprintingBuff2);
                    SprintingBuff.removeBuff();
                    SprintingBuff2.removeBuff();
                    Game1.buffsDisplay.syncIcons();
                }
            }
        }

        /* stamina drain */
        private void OneSecond(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Config.StaminaDrain.DrainStamina)
                if (playerSprinting && !Game1.paused && Game1.player.isMoving())
                    Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina - this.Config.StaminaDrain.StaminaDrainCost);
        }

        /* Check if sprinting buff exists */
        private bool SprintBuffExists()
        {
            if (SprintingBuff == null)
            {
                return false;
            }

            return Game1.buffsDisplay.otherBuffs.Contains(SprintingBuff);
        }

        /* check if sprinting buff 2 (when leftshift is the keybind) exists */
        private bool SprintBuff2Exists()
        {
            if (SprintingBuff2 == null)
            {
                return false;
            }

            return Game1.buffsDisplay.otherBuffs.Contains(SprintingBuff2);
        }
    }
}
