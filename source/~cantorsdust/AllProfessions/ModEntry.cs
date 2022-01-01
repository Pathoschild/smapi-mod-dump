/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cantorsdust/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using AllProfessions.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace AllProfessions
{
    /// <summary>The entry class called by SMAPI.</summary>
    internal class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The MD5 hash of the data.json, used to detect when the file is edited.</summary>
        private readonly string DataFileHash = "a3b6882bf1d9026055423b73cbe05e50";

        /// <summary>The professions by skill and level requirement.</summary>
        private ModDataProfessions[] ProfessionMap;

        /// <summary>The mod configuration.</summary>
        private ModConfig Config;


        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);

            // read config
            this.Config = helper.ReadConfig<ModConfig>();
            if (this.Config.Normalize(this.Monitor))
                helper.WriteConfig(this.Config);

            // read data
            this.ProfessionMap = this.GetProfessionMap(this.Helper.Data.ReadJsonFile<ModData>("assets/data.json")).ToArray();
            if (!this.ProfessionMap.Any())
            {
                this.Monitor.Log("The data.json file is missing or invalid; try reinstalling the mod.", LogLevel.Error);
                return;
            }

            // log if data.json is customized
            string dataPath = Path.Combine(this.Helper.DirectoryPath, "assets", "data.json");
            if (File.Exists(dataPath))
            {
                string hash = this.GetFileHash(dataPath);
                if (hash != this.DataFileHash)
                    this.Monitor.Log($"Using a custom data.json file (MD5 hash: {hash}).");
            }

            // hook event
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }


        /*********
        ** Private methods
        *********/
        /****
        ** Event handlers
        ****/
        /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            GenericModConfigMenuIntegration.Register(this.ModManifest, this.Helper.ModRegistry, this.Monitor, this.ProfessionMap,
                getConfig: () => this.Config,
                reset: () => this.Config = new(),
                save: () =>
                {
                    this.Config.Normalize(this.Monitor);
                    this.Helper.WriteConfig(this.Config);
                }
            );
        }

        /// <inheritdoc cref="IGameLoopEvents.DayStarted"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // When the player loads a saved game, or after the overnight level screen,
            // add any professions the player should have but doesn't.
            this.AddMissingProfessions();
        }

        /****
        ** Methods
        ****/
        /// <summary>Get the MD5 hash for a file.</summary>
        /// <param name="absolutePath">The absolute file path.</param>
        private string GetFileHash(string absolutePath)
        {
            using FileStream stream = File.OpenRead(absolutePath);
            using MD5 md5 = MD5.Create();

            byte[] hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>Get the professions by skill and level requirement.</summary>
        /// <param name="data">The underlying mod data.</param>
        private IEnumerable<ModDataProfessions> GetProfessionMap(ModData data)
        {
            if (data?.ProfessionsToGain == null)
                yield break;

            foreach (ModDataProfessions set in data.ProfessionsToGain)
            {
                if (set.Professions != null && set.Professions.Any())
                    yield return set;
            }
        }

        /// <summary>Add all missing professions.</summary>
        private void AddMissingProfessions()
        {
            // get missing professions
            List<Profession> expectedProfessions = new List<Profession>();
            foreach (ModDataProfessions entry in this.ProfessionMap)
            {
                if (Game1.player.getEffectiveSkillLevel((int)entry.Skill) >= entry.Level)
                {
                    expectedProfessions.AddRange(
                        entry.Professions.Where(profession => !this.Config.ShouldIgnore(profession))
                    );
                }
            }

            // add professions
            foreach (int professionID in expectedProfessions.Select(p => (int)p).Distinct().Except(Game1.player.professions))
            {
                // add profession
                Game1.player.professions.Add(professionID);

                // add health bonuses that are a special case of LevelUpMenu.getImmediateProfessionPerk
                switch (professionID)
                {
                    // fighter
                    case 24:
                        Game1.player.maxHealth += 15;
                        break;

                    // defender
                    case 27:
                        Game1.player.maxHealth += 25;
                        break;
                }
            }
        }
    }
}
