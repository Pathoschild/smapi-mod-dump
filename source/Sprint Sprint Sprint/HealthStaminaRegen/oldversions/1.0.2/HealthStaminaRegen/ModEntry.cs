using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace HealthStaminaRegen
{
    class ModEntry : Mod
    {
        private ModConfig Config;

        private int secondsUntilHealthRegen = 0;
        private int secondsUntilStaminaRegen = 0;

        private int lastHealth;
        private float lastStamina;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.OneSecondUpdateTicked += this.oneSecond;

            /* Read Config */
            this.Config = helper.ReadConfig<ModConfig>();
        }

        private void oneSecond(object sender, EventArgs e)
        {
            /* Variables */
            var player = Game1.player;

            if (!Context.IsPlayerFree || Game1.paused)
            {
                return;
            }

            else
            {   /****************
                **Health Regen**
                ****************/
                // if player took damage
                if (player.health < this.lastHealth)
                {
                    this.secondsUntilHealthRegen = this.Config.SecondsUntilHealthRegen;
                }
                //timer
                else if (this.secondsUntilHealthRegen > 0)
                {
                    this.secondsUntilHealthRegen--;
                }
                //regen
                else if (this.secondsUntilHealthRegen <= 0)
                {
                    if (player.health < player.maxHealth)
                    {
                        player.health = Math.Min(player.maxHealth, player.health + this.Config.HealthRegenRate);
                    }
                }

                /***************
                 *Stamina Regen*
                 ***************/
                // if player used stamina
                if (player.Stamina < this.lastStamina)
                {
                    this.secondsUntilStaminaRegen = this.Config.SecondsUntilStaminaRegen;
                }
                //timer
                else if (this.secondsUntilStaminaRegen > 0)
                {
                    this.secondsUntilStaminaRegen--;
                }
                // regen
                else if (this.secondsUntilStaminaRegen <= 0)
                {
                    if (player.Stamina < player.MaxStamina)
                    {
                        player.Stamina = Math.Min(player.MaxStamina, player.Stamina + this.Config.StaminaRegenRate);
                    }
                }

                this.lastHealth = player.health;
                this.lastStamina = player.Stamina;
            }
        }
    }
}
