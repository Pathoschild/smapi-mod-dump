/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JessebotX/StardewValleyMods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;

namespace Sprint
{
    class ModEntry : Mod
    {
        //reference ModConfig class
        private ModConfig Config;

        private bool playerSprinting = false;
        private int SprintSpeed;

        public override void Entry(IModHelper helper)
        {
            /* Read Config */
            this.Config = helper.ReadConfig<ModConfig>();

            if (this.Config.SprintKey == SButton.LeftShift)
            {
                this.SprintSpeed = this.Config.Sprint.SprintSpeed + this.Config.Sprint.LeftShiftKeybindExtraSpeed;
            }
            else
            {
                this.SprintSpeed = this.Config.Sprint.SprintSpeed;
            }
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

            if (!Context.IsPlayerFree)
            {
                return;
            }

            else
            {
                if (isSprintKey)
                {
                    playerSprinting = true;
                    Game1.player.Speed = this.SprintSpeed;
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
    }
}
