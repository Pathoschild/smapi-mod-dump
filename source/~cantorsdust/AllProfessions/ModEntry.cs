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

        /// <summary>Professions to gain for each level. Each entry represents the skill, level requirement, and profession IDs.</summary>
        private ModDataProfessions[] ProfessionsToGain;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // read data
            this.ProfessionsToGain = this.GetProfessionsToGain(this.Helper.ReadJsonFile<ModData>("data.json")).ToArray();
            if (!this.ProfessionsToGain.Any())
            {
                this.Monitor.Log("The data.json file is missing or invalid; try reinstalling the mod.", LogLevel.Error);
                return;
            }

            // log if data.json is customised
            string dataPath = Path.Combine(this.Helper.DirectoryPath, "data.json");
            if (File.Exists(dataPath))
            {
                string hash = this.GetFileHash(dataPath);
                if (hash != this.DataFileHash)
                    this.Monitor.Log($"Using a custom data.json file (MD5 hash: {hash}).", LogLevel.Trace);
            }

            // hook event
            TimeEvents.AfterDayStarted += this.ReceiveAfterDayStarted;
        }


        /*********
        ** Private methods
        *********/
        /****
        ** Event handlers
        ****/
        /// <summary>The method called after a new day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ReceiveAfterDayStarted(object sender, EventArgs e)
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
            using (FileStream stream = File.OpenRead(absolutePath))
            using (MD5 md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        /// <summary>Get the profession levels to gain for each skill level.</summary>
        /// <param name="data">The underlying mod data.</param>
        private IEnumerable<ModDataProfessions> GetProfessionsToGain(ModData data)
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
            foreach (ModDataProfessions entry in this.ProfessionsToGain)
            {
                if (Game1.player.getEffectiveSkillLevel((int)entry.Skill) >= entry.Level)
                    expectedProfessions.AddRange(entry.Professions);
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
