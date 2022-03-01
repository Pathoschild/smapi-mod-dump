/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/OhWellMikell/Starksouls
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewModdingAPI.Utilities;
using Starksouls.Config;

namespace Starksouls
{
    public class ModEntry : Mod
    {
        // Mod Variables
        private ModConfig Config;
        private bool DebugLogging;

        private PerScreen<int> Health = new PerScreen<int>();
        private PerScreen<float> Stamina = new PerScreen<float>();

        // === On Mod Load === //
        public override void Entry(IModHelper helper)
        {
            var moddata = this.Helper.ReadConfig<ModConfig>();

            /* Read Data Config */
            this.Config = helper.ReadConfig<ModConfig>();

            /* Event Hooks */
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
            helper.Events.GameLoop.SaveLoaded += this.OnLoadedSave;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTick;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.DayEnding += this.OnDayEnded;

            /* Console Commands */
            this.Helper.ConsoleCommands.Add("open_debuglog", "spams your smapi log with debug info used to debug things.\n" +
            "Note that you have to restart in order to stop the debug info spamming on your console, " +
            "this command is for when you have found a bug and want to report it " +
            "(including the parsed log(see https://log.smapi.io))",
            this.DebugLoggingCommand);
        }


        // === Game Loop Events === //
        private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
        {
            // Retrieve Generic Mod Config Menu API
            var genConfigMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if (genConfigMenu == null)
            {
                this.Monitor.Log("Generic Mod Config Menu not installed. No integration needed", LogLevel.Info);
                return;
            }

            genConfigMenu.RegisterModConfig(this.ModManifest, () => this.Config = new ModConfig(), () => Helper.WriteConfig(this.Config));
            this.HealthConfigImplementation(genConfigMenu);
            this.StaminaConfigImplementation(genConfigMenu);
        }
        private void OnLoadedSave(object sender, SaveLoadedEventArgs e)
        {
            this.Monitor.Log("Save Loaded!", LogLevel.Debug);
            this.SetLoadValues(e);
        }
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            this.Monitor.Log("Day Started!", LogLevel.Debug);
            this.SetHealth(e);
            this.SetStamina(e);
        }
        private void OnUpdateTick(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (!Context.IsPlayerFree) return;

            CheckIfDead(e);
        }
        private void OnDayEnded(object sender, DayEndingEventArgs e)
        {
            this.Monitor.Log("Day Ended!", LogLevel.Debug);
            this.WriteHealth(e);
            this.WriteStamina(e);
        }
        

        // ===  Mod Functions === //
        private void SetLoadValues(SaveLoadedEventArgs e)
        {
            this.Monitor.Log("SetLoadValues Called!", LogLevel.Debug);
            if (this.Config.Health.Enabled)
            {
                this.Health.Value = Game1.player.health;
                this.Monitor.Log($"Loaded Health: {this.Health.Value}", LogLevel.Debug);
            }
            if (this.Config.Stamina.Enabled)
            {
                this.Stamina.Value = Game1.player.stamina;
                this.Monitor.Log($"Loaded Stamina: {this.Stamina.Value}", LogLevel.Debug);
            }
        }
        private void SetHealth(DayStartedEventArgs e)
        {
            this.Monitor.Log("SetHealth Called!", LogLevel.Debug);
            if (this.Config.Health.Enabled)
            {
                if (this.Health.Value <= 0)
                {
                    Game1.player.health = Game1.player.maxHealth ;
                    this.Monitor.Log("The Player Started Day with 0 Health. Defaulting to Max Health.", LogLevel.Debug);
                }
                else
                {
                    Game1.player.health = this.Health.Value + Convert.ToInt32(Math.Floor(this.Config.Health.HealthRegenPercent / 100.0 * (Game1.player.maxHealth - this.Health.Value)));
                    this.Monitor.Log($"Last Recorded Health: {this.Health.Value}", LogLevel.Debug);
                }
                this.Monitor.Log($"Setting Health to: {Game1.player.health}", LogLevel.Debug);
            }
        }
        private void SetStamina(DayStartedEventArgs e)
        {
            this.Monitor.Log("SetStamina Called!", LogLevel.Debug);
            if (this.Config.Stamina.Enabled)
            {
                if (this.Stamina.Value <= 0)
                {
                    this.Monitor.Log("The Player Exhausted Theirself. Updating Stamina...", LogLevel.Debug);
                    this.Monitor.Log($"Max Stamina: {Game1.player.maxStamina}", LogLevel.Debug);
                    Game1.player.stamina = Convert.ToInt32(Math.Floor(this.Config.Stamina.StaminaAfterPassOutPercent / 100.0 * Game1.player.maxStamina));
                }
                else
                {
                    this.Monitor.Log($"Last Recorded Stamina: {this.Stamina.Value}", LogLevel.Debug);
                    Game1.player.stamina = this.Stamina.Value + Convert.ToInt32(Math.Floor(this.Config.Stamina.StaminaRegenPercent / 100.0 * (Game1.player.maxStamina - this.Stamina.Value)));
                }
                this.Monitor.Log($"Setting Stamina to: {Game1.player.stamina}", LogLevel.Debug);
            }
        }
        private void CheckIfDead(UpdateTickedEventArgs e)
        {
            if (Game1.player.health > 0) return;
            if (this.Config.Health.Enabled)
            {
                if (e.IsMultipleOf(60 * 5))
                {
                    this.Monitor.Log("Player Died. Updating Health...", LogLevel.Debug);
                    this.Monitor.Log($"Max Health: {Game1.player.maxHealth}", LogLevel.Debug);
                    Game1.player.health = Convert.ToInt32(Math.Floor(this.Config.Health.HealthAfterDeathPercent / 100.0 * Game1.player.maxHealth));
                    this.Monitor.Log($"Setting Health to: {Game1.player.health}", LogLevel.Debug);
                }
            }
        }
        private void WriteHealth(DayEndingEventArgs e)
        {
            this.Monitor.Log("WriteHealth Called!", LogLevel.Debug);
            if (this.Config.Health.Enabled)
            {
                this.Health.Value = Game1.player.health;
                this.Monitor.Log($"Recorded Health: {this.Health.Value}", LogLevel.Debug);
            }
        }
        private void WriteStamina(DayEndingEventArgs e)
        {
            this.Monitor.Log("WriteStamina Called!", LogLevel.Debug);
            if (this.Config.Stamina.Enabled)
            {
                this.Stamina.Value = Game1.player.stamina;
                this.Monitor.Log($"Recorded Stamina: {this.Stamina.Value}", LogLevel.Debug);
            }
        }

        // === Utility Functions == //
        private void DebugLoggingCommand(string command, string[] args)
        {
            this.DebugLogging = true;
        }
        private void HealthConfigImplementation(IGenericModConfigMenuAPI api)
        {
            api.RegisterSimpleOption(this.ModManifest, "Enable Health Debuff", "Instead of vanilla regen, the farmer wakes up with an X% of missing health restored.",
                () => this.Config.Health.Enabled, (bool val) => this.Config.Health.Enabled = val);
            api.RegisterSimpleOption(this.ModManifest, "Missing Health % Restored", "The percentage of missing health you regenerate each morning. A number between 0 - 100.",
                () => this.Config.Health.HealthRegenPercent, (int val) => this.Config.Health.HealthRegenPercent = val);
            api.RegisterSimpleOption(this.ModManifest, "Health % After Pass Out (Coming Soon)", "The percentage of your max health you wake up with after falling to 0 HP. A number between 1 - 100.",
                () => this.Config.Health.HealthAfterDeathPercent, (int val) => this.Config.Health.HealthAfterDeathPercent = val);
        }
        private void StaminaConfigImplementation(IGenericModConfigMenuAPI api)
        {
            api.RegisterSimpleOption(this.ModManifest, "Enable Stamina Debuff", "Intead of vanilla regen, the farmer wakes up with an X% of missing stamina restored.",
                () => this.Config.Stamina.Enabled, (bool val) => this.Config.Stamina.Enabled = val);
            api.RegisterSimpleOption(this.ModManifest, "Missing Stamina % Restored", "The percentage of missing stamina you regenerate each morning. A number between 0 - 100.",
                () => this.Config.Stamina.StaminaRegenPercent, (int val) => this.Config.Stamina.StaminaRegenPercent = val);
            api.RegisterSimpleOption(this.ModManifest, "Stamina % After Pass Out (Coming Soon)", "The percentage of your max stamina you wake up with after passing out. A number between 1 - 100.",
                () => this.Config.Stamina.StaminaAfterPassOutPercent, (int val) => this.Config.Stamina.StaminaAfterPassOutPercent = val);
        }
    }
}
