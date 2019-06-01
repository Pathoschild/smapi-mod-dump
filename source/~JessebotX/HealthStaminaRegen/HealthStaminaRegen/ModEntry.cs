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
using HealthStaminaRegen.Config;

namespace HealthStaminaRegen
{
    class ModEntry : Mod
    {
        private ModConfig Config;
        private bool DebugLogging;

        private int SecondsUntilHealthRegen = 0;
        private int SecondsUntilStaminaRegen = 0;

        private int LastHealth;
        private float LastStamina;

        public override void Entry(IModHelper helper)
        {
            this.Monitor.Log("Note: Since Health & Stamina Regeneration v2.0.0, a config overhaul has been done and your config.json has been reset, " +
                "if you had custom values set before v2.0.0, you will need to go into the config.json and re add the values again, " +
                "if you never configured the mod and think that the default values are fine, you can safely ignore this. \n",LogLevel.Alert
            );

            /* Read config */
            this.Config = helper.ReadConfig<ModConfig>();

            /* Hook events */
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;

            /* Console Commands */
            this.Helper.ConsoleCommands.Add("healthstaminaregen_confighelp", "shows config.json document", this.ConfigHelpCommand);
            this.Helper.ConsoleCommands.Add("healthstaminaregen_debuglogging", "spams your smapi log with debug info used to debug things.\n" +
                "Note that you have to restart in order to stop the debug info spamming on your console, this command is for when you have found a bug and want to report it (including the parsed log(see https://log.smapi.io))", 
                this.DebugLoggingCommand);
        }

        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree || Game1.paused)
                return;

            /********
            ** Health
            ********/
            if (this.Config.Health.Enabled)
            {
                if (e.IsMultipleOf(this.Config.Health.RegenRateInSeconds * 60))
                {
                    if (!this.Config.Health.DontCheckConditions)
                    {
                        // if player took damage
                        if (Game1.player.health < this.LastHealth)
                            this.SecondsUntilHealthRegen = this.Config.Health.SecondsUntilRegenWhenTakenDamage;
                        //timer
                        else if (this.SecondsUntilHealthRegen > 0)
                            this.SecondsUntilHealthRegen--;
                        //regen
                        else if (this.SecondsUntilHealthRegen <= 0)
                            if (Game1.player.health < Game1.player.maxHealth)
                                Game1.player.health = Math.Min(Game1.player.maxHealth, Game1.player.health + this.Config.Health.HealthPerRegenRate);

                        this.LastHealth = Game1.player.health;

                        if (this.DebugLogging)
                        {
                            this.Monitor.Log("Health Updated", LogLevel.Debug);
                            this.Monitor.Log($"Last Health: {LastHealth.ToString()} ");
                        }
                    }

                    else
                    {
                        Game1.player.health += this.Config.Health.HealthPerRegenRate;

                        if (this.DebugLogging)
                            this.Monitor.Log("Health Updated (No Regen Delay)", LogLevel.Debug);
                    }
                }
            }

            /********
            ** Stamina
            *********/
            if (this.Config.Stamina.Enabled)
            {
                if (e.IsMultipleOf(this.Config.Stamina.RegenRateInSeconds * 60))
                {
                    if (!this.Config.Stamina.DontCheckConditions)
                    {
                        // if player used stamina
                        if (Game1.player.Stamina < this.LastStamina)
                            this.SecondsUntilStaminaRegen = this.Config.Stamina.SecondsUntilRegenWhenUsedStamina;
                        //timer
                        else if (this.SecondsUntilStaminaRegen > 0)
                            this.SecondsUntilStaminaRegen--;
                        // regen
                        else if (this.SecondsUntilStaminaRegen <= 0)
                            if (Game1.player.Stamina < Game1.player.MaxStamina)
                                Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + this.Config.Stamina.StaminaPerRegenRate);

                        this.LastStamina = Game1.player.Stamina;

                        if (this.DebugLogging)
                        {
                            this.Monitor.Log("Stamina Updated", LogLevel.Debug);
                            this.Monitor.Log($"Last Stamina: {LastStamina.ToString()}");
                        }
                    }

                    else
                    {
                        Game1.player.Stamina += this.Config.Stamina.StaminaPerRegenRate;

                        if (this.DebugLogging)
                            this.Monitor.Log("Stamina Updated (No Regen Delay)", LogLevel.Debug);
                    }
                }
            }

            if (e.IsMultipleOf(60) && this.DebugLogging == true)
                this.Monitor.Log("1 second has passed", LogLevel.Debug);
        }

        private void ConfigHelpCommand(string command, string[] args)
        {
            this.Monitor.Log(
                "[NOTE: 2.0 breaks old configs] \n\n" +
                "CONFIG DOCUMENTATION\n" +
                "-----------------------------\n" +
                "-> Enabled: default true, if you want to enable/disable either health or stamina regenerating" +
                "-> HealthPerRegenRate/StaminaPerRegenRate: the amount you regenerate every {RegenRate} seconds. \n" +
                "-> RegenRateInSeconds: the rate in seconds you regen.\n" +
                "-> SecondsUntilRegenWhenUsedStamina/SecondsUntilRegenWhenTakenDamage: the regen cooldown when you take damage or used stamina.\n" +
                "-> DontCheckConditions: default false, if true, makes it so that it ignores SecondsUntilRegen and just keeps regenerating, ignoring max health/stamina " +
                "(recommended for people who are using the mod to degenerate like a hunger mod)\n" +
                "-----------------------------\n" +
                "See https://github.com/JessebotX/StardewMods/tree/master/HealthStaminaRegen#configure for the full config.json documentation\n\n" +
                "(If you dont see the config.json in the HealthStaminaRegen folder, you have to run the game once with this mod installed for it to generate)",
                LogLevel.Info
            );
        }

        private void DebugLoggingCommand(string command, string[] args)
        {
            this.DebugLogging = true;
        }
    }
}

