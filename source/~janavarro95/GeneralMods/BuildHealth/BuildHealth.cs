using System;
using System.IO;
using System.Linq;
using Omegasis.BuildHealth.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Omegasis.BuildHealth
{
    /// <summary>The mod entry point.</summary>
    public class BuildHealth : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The relative path for the current player's data file.</summary>
        private string DataFilePath => Path.Combine("data", $"{Constants.SaveFolderName}.json");

        /// <summary>The absolute path for the current player's legacy data file.</summary>
        private string LegacyDataFilePath => Path.Combine(this.Helper.DirectoryPath, "PlayerData", $"BuildHealth_data_{Game1.player.Name}.txt");

        /// <summary>The mod settings and player data.</summary>
        private ModConfig Config;

        /// <summary>The data for the current player.</summary>
        private PlayerData PlayerData;

        /// <summary>Whether the player recently gained XP for tool use.</summary>
        private bool HasRecentToolExp;

        /// <summary>Whether the player was eating last time we checked.</summary>
        private bool WasEating;

        /// <summary>The player's health last time we checked it.</summary>
        private int LastHealth;

        /// <summary>Whether the player has collapsed today.</summary>
        private bool WasCollapsed;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
            GameEvents.OneSecondTick += this.GameEvents_OneSecondTick;
            TimeEvents.AfterDayStarted += this.SaveEvents_BeforeSave;
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoaded;

            this.Config = helper.ReadConfig<ModConfig>();
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked once per second during a game update.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void GameEvents_OneSecondTick(object sender, EventArgs e)
        {
            // nerf how quickly tool xp is gained (I hope)
            if (this.HasRecentToolExp)
                this.HasRecentToolExp = false;
        }

        /// <summary>The method invoked when the game updates (roughly 60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // give XP when player finishes eating
            if (Game1.player.isEating)
                this.WasEating = true;
            else if (this.WasEating)
            {
                this.PlayerData.CurrentExp += this.Config.ExpForEating;
                this.WasEating = false;
            }

            // give XP when player uses tool
            if (!this.HasRecentToolExp && Game1.player.UsingTool)
            {
                this.PlayerData.CurrentExp += this.Config.ExpForToolUse;
                this.HasRecentToolExp = true;
            }

            // give XP for taking damage
            var player = Game1.player;
            if (this.LastHealth > player.health)
            {
                this.PlayerData.CurrentExp += this.LastHealth - player.health;
                this.LastHealth = player.health;
            }
            else if (this.LastHealth < player.health)
                this.LastHealth = player.health;

            // give XP when player stays up too late or collapses
            if (!this.WasCollapsed && Game1.farmerShouldPassOut)
            {
                this.PlayerData.CurrentExp += this.Config.ExpForCollapsing;
                this.WasCollapsed = true;
                this.Monitor.Log("The player has collapsed!");
            }
        }

        /// <summary>The method invoked after the player loads a save.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void SaveEvents_AfterLoaded(object sender, EventArgs e)
        {
            // reset state
            this.HasRecentToolExp = false;
            this.WasEating = false;
            this.LastHealth = Game1.player.health;
            this.WasCollapsed = false;

            // load player data
            this.MigrateLegacyData();
            this.PlayerData = this.Helper.ReadJsonFile<PlayerData>(this.DataFilePath) ?? new PlayerData();
            if (this.PlayerData.OriginalMaxHealth == 0)
                this.PlayerData.OriginalMaxHealth = Game1.player.maxHealth;

            // reset if needed
            if (this.PlayerData.ClearModEffects)
            {
                Game1.player.maxHealth = this.PlayerData.OriginalMaxHealth;
                this.PlayerData.ExpToNextLevel = this.Config.ExpToNextLevel;
                this.PlayerData.CurrentExp = this.Config.CurrentExp;
                this.PlayerData.CurrentLevelHealthBonus = 0;
                this.PlayerData.OriginalMaxHealth = Game1.player.maxHealth;
                this.PlayerData.BaseHealthBonus = 0;
                this.PlayerData.CurrentLevel = 0;
                this.PlayerData.ClearModEffects = false;
            }

            // else apply health bonus
            else
                Game1.player.maxHealth = this.PlayerData.BaseHealthBonus + this.PlayerData.CurrentLevelHealthBonus + this.PlayerData.OriginalMaxHealth;
        }

        /// <summary>The method invoked just before the game saves.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            // reset data
            this.LastHealth = Game1.player.maxHealth;
            this.WasCollapsed = false;

            // update settings
            var player = Game1.player;
            this.PlayerData.CurrentExp += this.Config.ExpForSleeping;
            if (this.PlayerData.OriginalMaxHealth == 0)
                this.PlayerData.OriginalMaxHealth = player.maxHealth; //grab the initial Health value

            if (this.PlayerData.CurrentLevel < this.Config.MaxLevel)
            {
                while (this.PlayerData.CurrentExp >= this.PlayerData.ExpToNextLevel)
                {
                    this.PlayerData.CurrentLevel += 1;
                    this.PlayerData.CurrentExp = this.PlayerData.CurrentExp - this.PlayerData.ExpToNextLevel;
                    this.PlayerData.ExpToNextLevel =
                        (this.Config.ExpCurve * this.PlayerData.ExpToNextLevel);
                    player.maxHealth += this.Config.HealthIncreasePerLevel;
                    this.PlayerData.CurrentLevelHealthBonus += this.Config.HealthIncreasePerLevel;
                }
            }

            // save data
            this.Helper.WriteJsonFile(this.DataFilePath, this.PlayerData);
        }

        /// <summary>Migrate the legacy settings for the current player.</summary>
        private void MigrateLegacyData()
        {
            // skip if no legacy data or new data already exists
            if (!File.Exists(this.LegacyDataFilePath) || File.Exists(this.DataFilePath))
                return;

            // migrate to new file
            try
            {
                string[] text = File.ReadAllLines(this.LegacyDataFilePath);
                this.Helper.WriteJsonFile(this.DataFilePath, new PlayerData
                {
                    CurrentLevel = Convert.ToInt32(text[3]),
                    CurrentExp = Convert.ToDouble(text[5]),
                    ExpToNextLevel = Convert.ToDouble(text[7]),
                    BaseHealthBonus = Convert.ToInt32(text[9]),
                    CurrentLevelHealthBonus = Convert.ToInt32(text[11]),
                    ClearModEffects = Convert.ToBoolean(text[14]),
                    OriginalMaxHealth = Convert.ToInt32(text[16])
                });

                FileInfo file = new FileInfo(this.LegacyDataFilePath);
                file.Delete();
                if (!file.Directory.EnumerateFiles().Any())
                    file.Directory.Delete();
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Error migrating data from the legacy 'PlayerData' folder for the current player. Technical details:\n {ex}", LogLevel.Error);
            }
        }
    }
}
