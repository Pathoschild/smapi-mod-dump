using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
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
            /* Read config */
            this.Config = helper.ReadConfig<ModConfig>();

            /* Hook events */
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;

            /* Console Commands */
            this.Helper.ConsoleCommands.Add("healthstaminaregen_confighelp", "shows config.json document", this.ConfigHelpCommand);
            this.Helper.ConsoleCommands.Add("healthstaminaregen_debuglogging", "spams your smapi log with debug info used to debug things.\n" +
                "Note that you have to restart in order to stop the debug info spamming on your console, " +
                "this command is for when you have found a bug and want to report it " +
                "(including the parsed log(see https://log.smapi.io))", 
                this.DebugLoggingCommand);
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // integration with Generic Mod Config Menu
            var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                this.Monitor.Log("Generic Mod Config Menu not installed. No integration needed", LogLevel.Info);
                return;
            }

            api.RegisterModConfig(this.ModManifest, () => this.Config = new ModConfig(), () => Helper.WriteConfig(this.Config));
            this.HealthConfigImplementation(api);
            this.StaminaConfigImplementation(api);

            api.RegisterSimpleOption(this.ModManifest, "Regen Even If Game Is Paused",
                "Even if the game is paused (i.e. opening a menu in singleplayer, cutscene playing, etc.),\n this option will allow you to keep regenerating both Health and Stamina (if they're enabled)",
                () => this.Config.IgnorePause, (bool val) => this.Config.IgnorePause = val);
        }

        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (!Context.IsPlayerFree && !this.Config.IgnorePause)
                return;

            this.HealthRegen(e);
            this.StaminaRegen(e);

            if (e.IsMultipleOf(60) && this.DebugLogging == true)
                this.Monitor.Log("1 second has passed", LogLevel.Trace);
        }

        private void HealthRegen(UpdateTickedEventArgs e)
        {
            if (this.Config.Health.Enabled)
            {
                if (e.IsMultipleOf((uint)this.Config.Health.RegenRateInSeconds * 60))
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
        }

        private void StaminaRegen(UpdateTickedEventArgs e)
        {
            if (this.Config.Stamina.Enabled)
            {
                if (e.IsMultipleOf((uint)this.Config.Stamina.RegenRateInSeconds * 60))
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
        }

        private void ConfigHelpCommand(string command, string[] args)
        {
            this.Monitor.Log(
                "See https://github.com/JessebotX/StardewMods/tree/master/HealthStaminaRegen#configure for the full config.json documentation.\n\n" +
                "(If you dont see the config.json in the HealthStaminaRegen folder, you have to run the game once with this mod installed for it to generate)",
                LogLevel.Info
            );
        }

        private void DebugLoggingCommand(string command, string[] args)
        {
            this.DebugLogging = true;
        }

        private void HealthConfigImplementation(IGenericModConfigMenuAPI api)
        {
            api.RegisterSimpleOption(this.ModManifest, "Enable Health Regeneration", "Allows your health to be modified by HealthPerRegenRate",
                () => this.Config.Health.Enabled, (bool val) => this.Config.Health.Enabled = val);
            api.RegisterSimpleOption(this.ModManifest, "Health Per Regen Rate", "The amount of health you get every <Regen Rate> amount of seconds. Must not contain any decimal values",
                () => this.Config.Health.HealthPerRegenRate, (int val) => this.Config.Health.HealthPerRegenRate = val);
            api.RegisterSimpleOption(this.ModManifest, "Health Regen Rate", "The seconds in between regeneration. Number must be greater than 0 and must not contain decimal values",
                () => this.Config.Health.RegenRateInSeconds, (int val) => this.Config.Health.RegenRateInSeconds = val);
            api.RegisterSimpleOption(this.ModManifest, "Seconds Until Health Regen After Taking Damage",
                "the cooldown for regen to start again after taking damage, set it to 0 if you don't want a regen cooldown",
                () => this.Config.Health.SecondsUntilRegenWhenTakenDamage, (int val) => this.Config.Health.SecondsUntilRegenWhenTakenDamage = val);
            api.RegisterSimpleOption(this.ModManifest, "Don't Check Health Regen Conditions",
                "Keep regenerating regardless if it goes past max health, ignores SecondsUntilRegen... etc. " +
                "\n(eg. this allows you to be able to create some sort of hunger mod where you have a negative number set for Health Per Regen Rate; " +
                "therefore forces you to eat or you may die)",
                () => this.Config.Health.DontCheckConditions, (bool val) => this.Config.Health.DontCheckConditions = val);
        }

        private void StaminaConfigImplementation(IGenericModConfigMenuAPI api)
        {
            api.RegisterSimpleOption(this.ModManifest, "Enable Stamina Regeneration", "Allows your stamina to be modified by StaminaPerRegenRate",
                () => this.Config.Stamina.Enabled, (bool val) => this.Config.Stamina.Enabled = val);
            api.RegisterSimpleOption(this.ModManifest, "Stamina Per Regen Rate", "The amount of stamina you get every <Regen Rate> seconds. Decimal values accepted",
                () => this.Config.Stamina.StaminaPerRegenRate, (float val) => this.Config.Stamina.StaminaPerRegenRate = val);
            api.RegisterSimpleOption(this.ModManifest, "Stamina Regen Rate", "The seconds in between regeneration. Number must be greater than 0 and must not contain decimal values",
                () => this.Config.Stamina.RegenRateInSeconds, (int val) => this.Config.Stamina.RegenRateInSeconds = val);
            api.RegisterSimpleOption(this.ModManifest, "Seconds Until Stamina Regen After Using Stamina",
                "the cooldown for regen to start again after using stamina, set it to 0 if you don't want a regen cooldown",
                () => this.Config.Stamina.SecondsUntilRegenWhenUsedStamina, (int val) => this.Config.Stamina.SecondsUntilRegenWhenUsedStamina = val);
            api.RegisterSimpleOption(this.ModManifest, "Don't Check Stamina Regen Conditions",
                "Keep regenerating regardless if it goes past max stamina, ignores SecondsUntilRegen... etc. " +
                "\n(eg. this allows you to be able to create some sort of hunger mod where you have a negative number set for Stamina Per Regen Rate; " +
                "therefore forces you to eat or you will run out of stamina and get over-exertion)",
                () => this.Config.Stamina.DontCheckConditions, (bool val) => this.Config.Stamina.DontCheckConditions = val);
        }
    }
}

