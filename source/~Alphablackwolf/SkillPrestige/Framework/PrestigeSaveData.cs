/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SkillPrestige.Framework.JsonNet.PrivateSettersContractResolvers;
using SkillPrestige.Logging;
using StardewValley;

namespace SkillPrestige.Framework
{
    /// <summary>Represents the save file data for the Skill Prestige Mod.</summary>
    [Serializable]
    internal class PrestigeSaveData
    {
        /*********
        ** Fields
        *********/
        private const string DataFileName = @"Data.json";
        private static readonly string DataFilePath = Path.Combine(ModEntry.ModPath, DataFileName);
        public static PrestigeSet CurrentlyLoadedPrestigeSet => Instance.PrestigeSaveFiles[CurrentlyLoadedSaveFileUniqueId];
        private static ulong CurrentlyLoadedSaveFileUniqueId { get; set; }

        // ReSharper disable once InconsistentNaming
        private static PrestigeSaveData _instance;


        /*********
        ** Accessors
        *********/
        /// <summary>Set of prestige data saved per save file unique ID.</summary>
        // ReSharper disable once MemberCanBePrivate.Global - no, it can't be made private or it won't be serialized.
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global - setter used by deserializer.
        public IDictionary<ulong, PrestigeSet> PrestigeSaveFiles { get; set; }


        /*********
        ** Public methods
        *********/
        // ReSharper disable once MemberCanBePrivate.Global - used publically, resharper is wrong.
        public static PrestigeSaveData Instance
        {
            get => _instance ??= new PrestigeSaveData();
            // ReSharper disable once UnusedMember.Global - used by deseralizer.
            set => _instance = value;
        }

        // ReSharper disable once MemberCanBeMadeStatic.Global - removing this removes lazy load in accessor for the instance.
        public void Save()
        {
            Logger.LogInformation("Writing prestige save data to disk...");
            File.WriteAllLines(DataFilePath, new[] { JsonConvert.SerializeObject(Instance) });
            Logger.LogInformation("Prestige save data written to disk.");
        }

        public void Read()
        {
            if (!File.Exists(DataFilePath))
                this.SetupDataFile();
            Logger.LogInformation("Deserializing prestige save data...");
            var settings = new JsonSerializerSettings { ContractResolver = new PrivateSetterContractResolver() };
            _instance = JsonConvert.DeserializeObject<PrestigeSaveData>(File.ReadAllText(DataFilePath), settings);
            Logger.LogInformation("Prestige save data loaded.");
        }

        public void UpdateCurrentSaveFileInformation()
        {
            if (CurrentlyLoadedSaveFileUniqueId == Game1.uniqueIDForThisGame)
                return;
            Logger.LogInformation("Save file change detected.");
            if (!Instance.PrestigeSaveFiles.ContainsKey(Game1.uniqueIDForThisGame))
            {
                Instance.PrestigeSaveFiles.Add(Game1.uniqueIDForThisGame, PrestigeSet.CompleteEmptyPrestigeSet);
                this.Save();
                Logger.LogInformation($"Save file not found in list, adding save file to prestige data. Id = {Game1.uniqueIDForThisGame}");
            }
            CurrentlyLoadedSaveFileUniqueId = Game1.uniqueIDForThisGame;
            this.UpdatePrestigeSkillsForCurrentFile();
            this.Read();
        }


        /*********
        ** Private methods
        *********/
        private PrestigeSaveData()
        {
            this.PrestigeSaveFiles = new Dictionary<ulong, PrestigeSet>();
            Logger.LogInformation("Created new prestige save data instance.");
        }

        private void UpdatePrestigeSkillsForCurrentFile()
        {
            Logger.LogVerbose("Checking for missing prestige data...");
            var missingPrestiges = PrestigeSet.CompleteEmptyPrestigeSet.Prestiges.Where(x => !CurrentlyLoadedPrestigeSet.Prestiges.Select(y => y.SkillType).Contains(x.SkillType)).ToList();
            if (!missingPrestiges.Any())
                return;
            Logger.LogInformation("Missing Prestige data found. Loading new prestige data...");
            var prestiges = new List<Prestige>(CurrentlyLoadedPrestigeSet.Prestiges);
            prestiges.AddRange(missingPrestiges);
            CurrentlyLoadedPrestigeSet.Prestiges = prestiges;
            this.Save();
            Logger.LogInformation("Missing Prestige data loaded.");
        }

        private void SetupDataFile()
        {
            Logger.LogInformation("Creating new data file...");
            try
            {
                this.Save();
            }
            catch (Exception exception)
            {
                Logger.LogCritical($"An error occured while attempting to create a data file. details: {Environment.NewLine} {exception}");
                throw;
            }
            Logger.LogInformation("Successfully created new data file.");
        }
    }
}
