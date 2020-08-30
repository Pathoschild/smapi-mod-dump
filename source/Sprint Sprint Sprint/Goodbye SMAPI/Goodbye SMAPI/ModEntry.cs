using Goodbye_SMAPI.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace Goodbye_SMAPI
{
    class ModEntry : Mod
    {
        #region Fields & Properties
        private bool QuitGame;
        private int QuitGameTimer;
        /// <summary> The random number generator. </summary>
        private Random RNG;

        /// <summary> The mod configuration options. </summary>
        private ModConfig Config;

        #endregion

        #region Methods

        /// <summary> The mod's entry point. </summary>
        /// <param name="helper"> Provides simplified APIs for writing mods. </param>
        public override void Entry(IModHelper helper)
        {
            this.RNG = new Random();
            this.Config = this.Helper.ReadConfig<ModConfig>();

            this.Helper.Events.GameLoop.OneSecondUpdateTicked += this.OneSecondUpdateTicked;

            this.AddCommands();
        }

        private void OneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (this.QuitGame)
            {
                if (this.QuitGameTimer > 0)
                {
                    this.QuitGameTimer--;
                    this.Monitor.Log($"Shutting down in {this.QuitGameTimer}...", LogLevel.Info);
                }
                if (this.QuitGameTimer <= 0)
                    Environment.Exit(0);
            }
        }

        private void AddCommands()
        {
            if (this.Config.CommandInput.Length != 0)
            {
                foreach (string cmd in this.Config.CommandInput)
                    this.Helper.ConsoleCommands.Add(cmd, $"Say {cmd.ToLower()} to SMAPI", this.Goodbye);
            } else
            {
                this.Monitor.Log(
                    "There are no command inputs found in the config.json. Please add values to the CommandInput section in the config.json found in Stardew Valley/Mods/Goodbye SMAPI/",
                    LogLevel.Error
                );
            }
        }

        private void Goodbye(string command, string[] args)
        {
            if (!this.QuitGame)
            {
                this.Monitor.Log(this.Config.Responses[this.RNG.Next(this.Config.Responses.Length)], LogLevel.Info);
                this.QuitGame = true;
                this.QuitGameTimer = 5;
            } else
                this.Monitor.Log("You've already said goodbye", LogLevel.Info);
        }

        #endregion
    }
}
